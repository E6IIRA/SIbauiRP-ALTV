using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Resources.Chat.Api;
using GangRP_Server.Core;
using GangRP_Server.Events;
using GangRP_Server.Handlers.Inventory;
using GangRP_Server.Handlers.Logger;
using GangRP_Server.Handlers.Player;
using GangRP_Server.Handlers.Vehicle;
using GangRP_Server.Models;
using GangRP_Server.Modules.Interior;
using GangRP_Server.Modules.Inventory;
using GangRP_Server.Utilities;
using GangRP_Server.Utilities.Blip;
using GangRP_Server.Utilities.StorageRoom;
using GangRP_Server.Utilities.Team;
using GangRP_Server.Utilities.Vehicle;
using GangRP_Server.Utilities.VehicleShop;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Vehicle = AltV.Net.Elements.Entities.Vehicle;
/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Modules.StorageRoom
{
    public sealed class StorageRoomModule : ModuleBase, ILoadEvent, IPlayerConnectEvent, IPressedEEvent, IPressedLEvent, IEntityColshapeHitEvent
    {
        private readonly RPContext _rpContext;
        private readonly InteriorModule _interiorModule;
        private readonly IInventoryHandler _inventoryHandler;

        public Dictionary<int, Storageroom> _storagerooms;
        public List<Marker> _marker = new List<Marker>();
        public List<PlayerLabel> _labels = new List<PlayerLabel>();
        public List<BlipData> _blipDataList = new List<BlipData>();

        public Dictionary<int, List<InteriorPositionData>> _upgradeInteriorPositionDataTypeIds = new Dictionary<int, List<InteriorPositionData>>();

        public StorageRoomModule(RPContext rpContext, InteriorModule interiorModule, IInventoryHandler inventoryHandler)
        {
            _rpContext = rpContext;
            _interiorModule = interiorModule;
            _inventoryHandler = inventoryHandler;
        }

        public void OnLoad()
        { 
            _storagerooms = AddTableLoadEvent<Storageroom>(_rpContext.Storageroom.Include(d => d.StorageroomData).Include(d => d.StorageroomInteriorPosition).ThenInclude(d => d.InteriorPositionData), OnItemLoad).ToDictionary(sr => sr.Id);

            AddClientEvent<int, int>("UpgradeStorage", UpgradeStorageroom);
            AddClientEvent<int>("UpgradeStorageroomType", UpgradeStorageroomType);
            //_upgradeInteriorPositionDataTypeIds.Add(0, _interiorModule._interiorPositionDatas.Values.ToList().FindAll(d => d.InteriorDataId == 2 && d.InteriorPositionDataTypeId == 4));
            //_upgradeInteriorPositionDataTypeIds.Add(1, _interiorModule._interiorPositionDatas.Values.ToList().FindAll(d => d.InteriorDataId == 3 && d.InteriorPositionDataTypeId == 4));
            //_upgradeInteriorPositionDataTypeIds.Add(2, _interiorModule._interiorPositionDatas.Values.ToList().FindAll(d => d.InteriorDataId == 4 && d.InteriorPositionDataTypeId == 4));
        }
        public async void OnItemLoad(Storageroom storageroom)
        {
            IColShape colShape = Alt.CreateColShapeSphere(storageroom.StorageroomData.Position, 2.0f);
            colShape.SetData("storageroomId", storageroom.Id);
            foreach (var storageroomInteriorPosition in storageroom.StorageroomInteriorPosition)
            {
                LocalInventory localInventory = await _inventoryHandler.LoadInventory(storageroomInteriorPosition.InventoryId,
                    storageroomInteriorPosition.Id);
                _marker.Add(MarkerStreamer.Create(MarkerTypes.MarkerTypeVerticalCylinder, storageroomInteriorPosition.InteriorPositionData.Position, new Vector3(1), color: new Rgba(255,255,0,255), dimension: storageroomInteriorPosition.Storageroom.Id));
                _labels.Add(TextLabelStreamer.Create($"a: {storageroomInteriorPosition.Id},b: {storageroomInteriorPosition.InteriorPositionData.Id}", storageroomInteriorPosition.InteriorPositionData.Position, color: new Rgba(255, 0, 0, 255), dimension: storageroomInteriorPosition.Storageroom.Id));
            }
#if DEBUG
            _marker.Add(MarkerStreamer.Create(MarkerTypes.MarkerTypeVerticalCylinder,
                storageroom.StorageroomData.Position, new Vector3(1), color: new Rgba(255, 0, 0, 255)));
            _blipDataList.Add(new BlipData(storageroom.StorageroomData.Position, $"Lagerhalle"));
            _labels.Add(TextLabelStreamer.Create($"Id: {storageroom.Id}", storageroom.StorageroomData.Position, color: new Rgba(255, 0, 0, 255)));
#endif
        }
        
#if DEBUG
        public void OnPlayerConnect(IPlayer player, string reason)
        {
            player.Emit("SetTempPlayerBlips", new BlipDataWriter(_blipDataList, 615, 3));
        }
#endif

        public void OnEntityColshapeHit(IColShape shape, IEntity entity, bool state)
        {
            if (state == false)
            {
                if (entity is IPlayer player)
                {
                    RPPlayer rpPlayer = (RPPlayer) player;
                    rpPlayer.DeleteData("storageroomId");
                }
                return;
            }
            if (shape.GetData("storageroomId", out int storageroomId))
            {
                if (entity is IPlayer player)
                {
                    RPPlayer rpPlayer = (RPPlayer) player;
                    rpPlayer.SetData("storageroomId", storageroomId);
                }
            }
        }

        public bool OnPressedL(IPlayer player)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            if (rpPlayer.DimensionType == DimensionType.STORAGEROOM)
            {
                //INSIDE
                if (!_storagerooms.TryGetValue(rpPlayer.Dimension, out Storageroom storageroom)) return false;
                ToggleStorageroomLock(rpPlayer, storageroom);
            }
            else if (rpPlayer.DimensionType == DimensionType.WORLD)
            {
                //OUTSIDE
                if (!rpPlayer.GetData("storageroomId", out int storageroomId)) return false;
                ToggleStorageroomLock(rpPlayer, storageroomId);
            }
            return false;
        }


        public Task<bool> OnPressedE(IPlayer player)
        {
            RPPlayer rpPlayer = (RPPlayer) player;

            if (rpPlayer.DimensionType == DimensionType.STORAGEROOM)
            {
                //INSIDE
                if (_storagerooms.TryGetValue(rpPlayer.Dimension, out Storageroom storageroom))
                {
                    InteriorData? interiorData = _interiorModule.GetInteriorDataById(storageroom.InteriorDataId);
                    if (interiorData == null) return Task.FromResult(false);

                    if (interiorData.Position.Distance(player.Position) < 1.0f)
                    {
                        LeaveStorageroom(rpPlayer, storageroom);
                    }
                    else if (1 == 1) //Abfrage, ob bei Laptop Position
                    {
                        OpenLaptop(rpPlayer, storageroom);
                    }
                }
            }
            else
            {
                //OUTSIDE
                if (!rpPlayer.GetData("storageroomId", out int storageroomId)) return Task.FromResult(false);
                if (!_storagerooms.TryGetValue(storageroomId, out Storageroom storageroom)) return Task.FromResult(false);
                EnterStorageroom(rpPlayer, storageroom);
            }
            return Task.FromResult(true);
        }

        public void OpenLaptop(RPPlayer rpPlayer, Storageroom storageroom)
        {
            int storageControlLevel = rpPlayer.GetStorageroomControlLevel(storageroom);
            int typeUpgradeable = 0;
            int crateUpgradeable = 0;
            if ((storageroom.InteriorDataId == 2 && storageroom.Crates == 6) ||
                (storageroom.InteriorDataId == 3 && storageroom.Crates == 42))
                typeUpgradeable = 1;
            if ((storageroom.InteriorDataId == 2 && storageroom.Crates < 6) ||
                (storageroom.InteriorDataId == 3 && storageroom.Crates < 42) ||
                storageroom.InteriorDataId == 4 && storageroom.Crates < 111)
                crateUpgradeable = 1;
            rpPlayer.Emit("ShowIF", "StorageLaptop", new StorageLaptopWriter(storageroom.Id, storageControlLevel, storageroom.Crates, typeUpgradeable, crateUpgradeable));
        }
        
        async void EnterStorageroom(RPPlayer rpPlayer, Storageroom storageroom)
        {
            if (storageroom == null || storageroom.Locked) return;

            rpPlayer.Dimension = storageroom.Id;
            rpPlayer.DimensionType = DimensionType.STORAGEROOM;
            rpPlayer.Emit("LoadStorageroom",  storageroom.Type, storageroom.Crates);
            await rpPlayer.SavePosition(rpPlayer.Position);
            await rpPlayer.SetPositionAsync(storageroom.InteriorData.Position);
        }

        async void LeaveStorageroom(RPPlayer rpPlayer, Storageroom storageroom)
        {
            if (storageroom == null || storageroom.Locked) return;

            rpPlayer.Dimension = 0;
            rpPlayer.DimensionType = DimensionType.WORLD;
            rpPlayer.Emit("UnloadStorageroom");
            await rpPlayer.SavePosition(storageroom.StorageroomData.Position);
            await rpPlayer.SetPositionAsync(storageroom.StorageroomData.Position);
        }

        public async void UpgradeStorageroom(IPlayer player, int storageroomId, int crates)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            if (!_storagerooms.TryGetValue(storageroomId, out Storageroom storageroom)) return;
            if (storageroom == null) return;
            if (!rpPlayer.CanControlStorageroom(storageroom)) return;
            if (storageroom.InteriorDataId == 2 && storageroom.Crates == 6 ||
                storageroom.InteriorDataId == 3 && storageroom.Crates == 42 ||
                storageroom.InteriorDataId == 4 && storageroom.Crates == 111) return;

            if (!HasStorageroomNeededItems(storageroom)) return;
                /*
                 * TODO: ITEMS ZUM UPGRADEN AUS DEM KISTEN / UPGRADEINVENTAR? ZIEHEN
                 */
            storageroom = await UpgradeStorageroomCrateAmount(storageroom);
            await using RPContext rpContext = new RPContext();
            rpContext.Storageroom.Update(storageroom);
            await rpContext.SaveChangesAsync();
            Console.WriteLine("storageroomType: " + storageroom.Type);
            rpPlayer.Emit("ReloadStorageroom", storageroom.Type, storageroom.Crates);
        }

        private async void CreateNewInteriorPosition(Storageroom storageroom, int type)
        {
            var inventory = await _inventoryHandler.CreateInventory(type);
            new StorageroomInteriorPosition()
            {
                StorageroomId = storageroom.Id,
                Storageroom = storageroom,
                InteriorPositionDataId = _upgradeInteriorPositionDataTypeIds[storageroom.Type]
                    .ElementAtOrDefault(storageroom.Crates / (storageroom.Type + 1) + 1).Id
            };
        }

        private async Task<Storageroom> UpgradeStorageroomCrateAmount(Storageroom storageroom)
        {
            return null;
        }

        public bool HasStorageroomNeededItems(Storageroom storageroom)
        {

            return true;
        }

        public async void UpgradeStorageroomType(IPlayer player, int storageroomId)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            if (!_storagerooms.TryGetValue(storageroomId, out Storageroom storageroom)) return;
            if (storageroom == null) return;
            if (!rpPlayer.CanControlStorageroom(storageroom)) return;
            if (storageroom.InteriorDataId == 4) return;
            if ((storageroom.InteriorDataId == 2 && storageroom.Crates != 6) ||
                (storageroom.InteriorDataId == 3 && storageroom.Crates != 42)) return;
            /*
             * TODO: ITEMS ZUM UPGRADEN AUS DEM KISTEN / UPGRADEINVENTAR? ZIEHEN
             */
            storageroom.InteriorDataId++;
            _interiorModule._interiorDatas.TryGetValue(storageroom.InteriorDataId, out InteriorData interiorData);
            if (interiorData == null) return;
            storageroom.InteriorData = interiorData;
            
            await using RPContext rpContext = new RPContext();
            var dbstorageroom = rpContext.Storageroom.Find(storageroom.Id);
            dbstorageroom.InteriorDataId++;
            if (dbstorageroom.InteriorDataId == 3)
            {
                dbstorageroom.Crates *= 2;
                storageroom.Crates *= 2;
            }
            else if (dbstorageroom.InteriorDataId == 4)
            {
                dbstorageroom.Crates = (int)((double)dbstorageroom.Crates  *1.5);
                storageroom.Crates = (int)((double)storageroom.Crates  *1.5);
            }
            rpContext.Storageroom.Update(dbstorageroom);
            await rpContext.SaveChangesAsync();
            rpPlayer.Emit("ReloadStorageroom", storageroom.InteriorDataId - 2, storageroom.Crates);
            await rpPlayer.SetPositionAsync(storageroom.InteriorData.Position);
        }

        void ToggleStorageroomLock(RPPlayer rpPlayer, int storageroomId)
        {
            if (!_storagerooms.TryGetValue(storageroomId, out Storageroom storageroom)) return;
            ToggleStorageroomLock(rpPlayer, storageroom);
        }

        void ToggleStorageroomLock(RPPlayer rpPlayer, Storageroom storageroom)
        {
            if (!rpPlayer.CanControlStorageroom(storageroom)) return;
            if (storageroom.Locked) rpPlayer.SendNotification("Lagerraum aufgeschlossen", RPPlayer.NotificationType.SUCCESS, $"({storageroom.Id})");
            else rpPlayer.SendNotification("Lagerraum abgeschlossen", RPPlayer.NotificationType.SUCCESS, $"({storageroom.Id})");
            storageroom.Locked = !storageroom.Locked;
        }

        public async void CreateStorageroom(RPPlayer rpPlayer)
        {
            Storageroom storageroom =
                _storagerooms.Values.ToList()
                    .FirstOrDefault(sr => sr.StorageroomData.Position.Distance(rpPlayer.Position) < 2.0f);
            if (storageroom != null)
            {
                rpPlayer.SendNotification($"Existierende Lagerhalle zu nah! Id: {storageroom.Id}", RPPlayer.NotificationType.ERROR);
                return;
            }
            StorageroomData storageroomData = new StorageroomData()
            {
                PositionX = rpPlayer.Position.X,
                PositionY = rpPlayer.Position.Y,
                PositionZ = rpPlayer.Position.Z - 1.0f
            };

            await using RPContext rpContext = new RPContext();
            await rpContext.StorageroomData.AddAsync(storageroomData);
            await rpContext.SaveChangesAsync();

            storageroomData = await rpContext.StorageroomData.OrderByDescending(d => d.Id).FirstOrDefaultAsync();

            storageroom = new Storageroom()
            {
                StorageroomDataId = storageroomData.Id,
                InteriorDataId = 2,
                Crates = 0
            };

            await rpContext.Storageroom.AddAsync(storageroom);
            await rpContext.SaveChangesAsync();

            storageroom = await rpContext.Storageroom.Where(sr => sr.StorageroomDataId == storageroomData.Id)
                .FirstOrDefaultAsync();
            _storagerooms.Add(storageroom.Id, storageroom);
            _labels.Add(TextLabelStreamer.Create($"Id: {storageroom.Id}", storageroom.StorageroomData.Position,
                color: new Rgba(255, 0, 0, 255)));
            _marker.Add(MarkerStreamer.Create(MarkerTypes.MarkerTypeVerticalCylinder,
                storageroom.StorageroomData.Position, new Vector3(1), color: new Rgba(255, 0, 0, 255)));
            _blipDataList.Add(new BlipData(storageroom.StorageroomData.Position, $"Lagerhalle"));
            foreach (var p in Alt.Server.GetPlayers())
            {
                p.Emit("SetTempPlayerBlips", new BlipDataWriter(_blipDataList, 615, 3));
            }
        }

        public Storageroom GetStorageById(int storageroomId)
        {
            if (_storagerooms.TryGetValue(storageroomId, out Storageroom storageroom)) return storageroom;

            return null;
        }
    }
}

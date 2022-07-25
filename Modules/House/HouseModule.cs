using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using GangRP_Server.Core;
using GangRP_Server.Events;
using GangRP_Server.Handlers.Inventory;
using GangRP_Server.Handlers.Logger;
using GangRP_Server.Handlers.Player;
using GangRP_Server.Models;
using GangRP_Server.Modules.Interior;
using GangRP_Server.Modules.Player;
using GangRP_Server.Utilities;
using GangRP_Server.Utilities.Cloth;
using GangRP_Server.Utilities.Confirm;
using GangRP_Server.Utilities.House;
using GangRP_Server.Utilities.Interior;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Vehicle = GangRP_Server.Models.Vehicle;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Modules.House
{
    public sealed class HouseModule : ModuleBase, IPressedEEvent, ILoadEvent, IEntityColshapeHitEvent
    {
        private readonly RPContext _rpContext;
        private readonly IPlayerHandler _playerHandler;
        private readonly IInventoryHandler _inventoryHandler;
        private readonly OfflinePlayerModule _offlinePlayerModule;
        private readonly InteriorModule _interiorModule;
        public Dictionary<int, Models.House> _houses;
        public HouseModule(RPContext rpContext, IPlayerHandler playerHandler, OfflinePlayerModule offlinePlayerModule, InteriorModule interiorModule, IInventoryHandler inventoryHandler)
        {
            _rpContext = rpContext;
            _playerHandler = playerHandler;
            _offlinePlayerModule = offlinePlayerModule;
            _interiorModule = interiorModule;
            _inventoryHandler = inventoryHandler;
        }

        public void OnLoad()
        {
            //_houses = _rpContext.House.Include(d => d.HouseData).Include(d => d.HouseInteriorPosition).ThenInclude(d=> d.InteriorPositionData).ToDictionary(data => data.Id);
            //_houses = _rpContext.House.Include(d => d.HouseData).Include(d => d.HouseInteriorPosition).Select(d => new {d, d.HouseInteriorPosition.Where(h => h.InteriorPositionData.InteriorPositionDataTypeId == 1)}).ThenInclude(d=> d.InteriorPositionData).ToDictionary(data => data.Id);
            _houses = _rpContext.House.Include(d => d.HouseData).Include(d => d.HouseInteriorPosition).ThenInclude(d=> d.InteriorPositionData).ToDictionary(house => house.Id);
            foreach (var kvp in _houses)
            {
                kvp.Value.HouseInteriorPosition.ToList().RemoveAll(d => d.InteriorPositionData.InteriorPositionDataTypeId != 1);
            }

            AddClientEvent<int>("GetHouseRentPlayers", GetHouseRentPlayers);
            AddClientEvent<int>("GetHouseVehicles", GetHouseVehicles);
            AddClientEvent<int>("RemovePlayerRent", RemovePlayerRent);
            AddClientEvent<int>("RemoveHouseVehicle", RemoveHouseVehicle);
            AddClientEvent<int, string>("UpdateDoorBellSign", UpdateDoorBellSign);
            AddClientEvent<int, int>("UpdatePlayerRent", UpdatePlayerRent);
            AddClientEvent<int, bool>("UpdateHouseLock", UpdateHouseLock);
            AddClientEvent<int>("EnterHouse", EnterHouse);
            AddClientEvent<int>("LeaveHouse", LeaveHouse);
            AddClientEvent<int>("HouseChangeHide", StartHouseChangeHide);
            AddClientEvent<int>("GetHouseHideOptions", GetHouseHideOptions);
            AddClientEvent<int, int>("SetHouseHideout", SetHouseHideout);
            AddClientEvent<int, int>("RemoveHouseHideout", RemoveHouseHideout);

            AddClientEvent("BuyHouse", BuyHouse);
            AddClientEvent("RentHouse", RentHouse);


            foreach (var house in _houses.Values)
            {
                IColShape colShape = Alt.CreateColShapeSphere(house.HouseData.Position, 3.0f);
                colShape.SetData("houseId", house.Id);
#if DEBUG
                TextLabelStreamer.Create($"House Id: {house.Id}", house.HouseData.Position, color: new Rgba(255, 255, 0, 255));
#endif

            }
            foreach (var houseInteriorPosition in _rpContext.HouseInteriorPosition)
            {
                _inventoryHandler.LoadInventory(houseInteriorPosition.InventoryId, houseInteriorPosition.HouseId);
            }


        }
        public async Task<bool> OnPressedE(IPlayer player)
        {
            RPPlayer rpPlayer = (RPPlayer) player;

            if (rpPlayer.DimensionType == DimensionType.HOUSE)
            {
                //INSIDE
                if (_houses.TryGetValue(rpPlayer.Dimension, out Models.House house))
                {
                    InteriorData? interiorData = _interiorModule.GetInteriorDataById(house.HouseData.InteriorDataId);
                    if (interiorData == null) return await Task.FromResult(false);

                    if (interiorData.Position.Distance(player.Position) < 1.5)
                    {
                        int houseControlLevel = rpPlayer.GetHouseControlLevel(house);
                        //player is at house door
                        player.Emit("ShowIF", "House", new HouseInfoWriter(house.Id, houseControlLevel, house.DoorbellSign, house.Locked, houseControlLevel > 0 ? house.Money : 0, inside: true));
                    }
                    else
                    {
                        InteriorWarderobeData interiorWarderobeData = interiorData.InteriorWarderobeData.FirstOrDefault();

                        if (interiorWarderobeData != null && interiorWarderobeData.Position.Distance(player.Position) < 1.5)
                        {
                            rpPlayer.OpenWarderobe();
                        }
                    }

                }
            }
            else
            {
                //OUTSIDE

                if (rpPlayer.GetData("houseId", out int houseId))
                {
                    Models.House? house = GetHouseById(houseId);
                    if (house == null) return await Task.FromResult(false);

                    await using RPContext rpContext = new RPContext();
                    PlayerHouseOwned playerHouseOwned = rpContext.PlayerHouseOwned.FirstOrDefault(d => d.HouseId == house.Id);

                    int count = rpContext.PlayerHouseRent.Count(d => d.HouseId == houseId);
                    bool canRent = !rpPlayer.RentHouses.ContainsKey(houseId) && count < house.HouseData.RentalPlaces;

                    int houseControlLevel = rpPlayer.GetHouseControlLevel(house);
                    if (playerHouseOwned != null)
                    {
                        //House is already bought
                        if (houseControlLevel > 0) player.Emit("ShowIF", "House", new HouseInfoWriter(house.Id, houseControlLevel, house.DoorbellSign, house.Locked, houseControlLevel == 2 ? house.Money : 0));
                        else if (canRent)
                        {
                            rpPlayer.Emit("ShowIF", "Confirm", new ConfirmWriter("Einmieten? Preis : 123123$", "Einmieten", "Abbrechen", "RentHouse"));
                        }
                    }
                    else
                    {
                        //House has no owner
                        if (canRent)
                        {
                            rpPlayer.Emit("ShowIF", "Confirm", new ConfirmWriter($"Haus kaufen? oder Einmieten? Preis : ${ house.HouseData.Price}, Miete : 50$", "Haus kaufen", "Einmieten", "BuyHouse", "RentHouse"));
                        }
                        else
                        {
                            rpPlayer.Emit("ShowIF", "Confirm", new ConfirmWriter($"Haus kaufen? Preis : ${house.HouseData.Price} ", "Kaufen", "Abbrechen", "BuyHouse"));
                        }


                    }
                }
            }
            return await Task.FromResult(true);
        }

        async void GetHouseRentPlayers(IPlayer player, int houseId)
        {

            await using RPContext rpContext = new RPContext();

            var temp = await rpContext.PlayerHouseRent.Where(d => d.HouseId == houseId).Include(d => d.Player).ToListAsync();

            player.Emit("UpdateView", "SendHouseRentPlayers", new HouseRentWriter(temp));
        }

        async void GetHouseVehicles(IPlayer player, int houseId)
        {
            Models.House? houseById = GetHouseById(houseId);

            HouseGarageData? houseGarageData = houseById?.HouseData.HouseGarageData.FirstOrDefault(d => d.HouseDataId == houseById.HouseDataId);

            if (houseGarageData == null) return;

            await using RPContext rpContext = new RPContext();

            List<Vehicle> vehicles = await rpContext.Vehicle.Include(d => d.VehicleData).Include(d => d.Player).Where(d => d.GarageDataId == houseGarageData.GarageDataId).ToListAsync();

            player.Emit("UpdateView", "SendHouseVehicles", new HouseVehicleWriter(vehicles));
        }

        async void GetHouseHideOptions(IPlayer player, int houseId)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            if (_houses.TryGetValue(houseId, out Models.House house))
            {
                Dictionary<int, InteriorPositionData> interiorPositionDatas = _interiorModule.GetInteriorPositionDatasByInteriorId(house.HouseData.InteriorDataId);
                player.Emit("UpdateView", "SendInteriorPos", new InteriorPositionWriter(interiorPositionDatas, house.HouseInteriorPosition));
            }
        }

        async void RemovePlayerRent(IPlayer player, int playerRentId)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            await using RPContext rpContext = new RPContext();

            var playerHouseRent = await rpContext.PlayerHouseRent.FindAsync(playerRentId);
            if (playerHouseRent == null) return;

            var targetRentPlayer = _playerHandler.GetOnlineRPPlayerByPlayerId(playerHouseRent.PlayerId);
            if (targetRentPlayer != null)
            {
                targetRentPlayer.RentHouses.Remove(playerHouseRent.Id);
                targetRentPlayer.SendNotification($"Der Mietvertrag des Hauses {playerHouseRent.HouseId} wurde von {rpPlayer.Name} gekündigt.", RPPlayer.NotificationType.ERROR);
                rpPlayer.SendNotification($"Du hast den Mietvertrag mit {targetRentPlayer.Name} gekündigt.", RPPlayer.NotificationType.SUCCESS);
            }
            else
            {
                rpPlayer.SendNotification($"Du hast den Mietvertrag mit {_offlinePlayerModule.GetOfflinePlayerName(playerHouseRent.PlayerId)} gekündigt.", RPPlayer.NotificationType.SUCCESS);
            }
            rpContext.PlayerHouseRent.Remove(playerHouseRent);
            await rpContext.SaveChangesAsync();
        }

        async void RemoveHouseVehicle(IPlayer player, int vehicleId)
        {
            await using RPContext rpContext = new RPContext();
            var vehicle = await rpContext.Vehicle.FindAsync(vehicleId);
            if (vehicle == null) return;
            vehicle.GarageDataId = 1;
            await rpContext.SaveChangesAsync();
        }

        async void RemoveHouseHideout(IPlayer player, int houseId, int positionDataId)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            if (_houses.TryGetValue(houseId, out Models.House house))
            {
                HouseInteriorPosition houseInteriorPosition = house.HouseInteriorPosition.FirstOrDefault(d => d.InteriorPositionDataId == positionDataId);
                if (houseInteriorPosition != null)
                {
                    RPContext rpContext = new RPContext();
                    rpContext.HouseInteriorPosition.Remove(houseInteriorPosition);
                    await rpContext.SaveChangesAsync();
                    await _inventoryHandler.RemoveInventory(houseInteriorPosition.InventoryId);
                    house.HouseInteriorPosition.Remove(houseInteriorPosition);
                    GetHouseHideOptions(player, houseId);
                }
            }
        }

        async void UpdateDoorBellSign(IPlayer player, int houseDataId, string newDoorBellSign)
        {
            //TODO SQL INJECTION VERHINDERN WENN DAS ENTITY FRAMEWORK NOCH NICHT AUTOMATISCH MACHT AMK

            if (_houses.TryGetValue(houseDataId, out Models.House house))
            {
                house.DoorbellSign = newDoorBellSign;
                this._houses[houseDataId] = house;
                await using RPContext rpContext = new RPContext();
                rpContext.House.Update(house);
                await rpContext.SaveChangesAsync();
            }
        }

        async void UpdatePlayerRent(IPlayer player, int playerRentDataId, int newPrice)
        {
            if (newPrice <= 0) return;
            RPPlayer rpPlayer = (RPPlayer)player;
            await using RPContext rpContext = new RPContext();
            var playerHouseRent = await rpContext.PlayerHouseRent.FindAsync(playerRentDataId);
            if (playerHouseRent == null) return;
            playerHouseRent.Cost = newPrice;
            rpContext.PlayerHouseRent.Update(playerHouseRent);
            await rpContext.SaveChangesAsync();
            var targetPlayer = _playerHandler.GetOnlineRPPlayerByPlayerId(playerHouseRent.PlayerId);
            if (targetPlayer == null) return;
            targetPlayer.RentHouses[playerHouseRent.HouseId] = newPrice;
        }

        void UpdateHouseLock(IPlayer player, int houseId, bool status)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            if (_houses.TryGetValue(houseId, out Models.House house))
            {
                if (house.Locked == status) return;
                if (rpPlayer.CanControlHouse(house) && (rpPlayer.Position.Distance(house.HouseData.Position) < 3 || rpPlayer.Position.Distance(house.HouseData.InteriorData.Position) < 3))
                {
                    if (house.Locked) rpPlayer.SendNotification("Haus aufgeschlossen", RPPlayer.NotificationType.SUCCESS, $"({house.Id}) {house.DoorbellSign}");
                    else rpPlayer.SendNotification("Haus abgeschlossen", RPPlayer.NotificationType.SUCCESS, $"({house.Id}) {house.DoorbellSign}");
                    house.Locked = status;
                }
            }
        }

        async void EnterHouse(IPlayer player, int houseId)
        {
            RPPlayer rpPlayer = (RPPlayer)player;
            if (_houses.TryGetValue(houseId, out Models.House house))
            {
                if (house.Locked) return;
                rpPlayer.Dimension = houseId;
                rpPlayer.DimensionType = DimensionType.HOUSE;
                await rpPlayer.SavePosition(player.Position);
                await rpPlayer.SetPositionAsync(house.HouseData.InteriorData.Position);
            }
        }

        async void LeaveHouse(IPlayer player, int houseId)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            if (_houses.TryGetValue(houseId, out Models.House house))
            {
                if (house.Locked) return;
                rpPlayer.Dimension = 0;
                rpPlayer.DimensionType = DimensionType.WORLD;
                await rpPlayer.SavePosition(house.HouseData.Position);
                await rpPlayer.SetPositionAsync(house.HouseData.Position);
                await rpPlayer.SetRotationAsync(house.HouseData.Rotation);
            }
        }

        async void StartHouseChangeHide(IPlayer player, int houseId)
        {
            RPPlayer rpPlayer = (RPPlayer)player;
            if (_houses.TryGetValue(houseId, out Models.House house))
            {
                //is Owner
                if (rpPlayer.GetHouseControlLevel(house) == 2)
                {
                    if (!house.ChangeHideStatus)
                    {
                        //Start Modus
                        Dictionary<int, InteriorPositionData> interiorPositionDatas = _interiorModule.GetInteriorPositionDatasByInteriorId(house.HouseData.InteriorDataId);

                        foreach (var interiorPositionData in interiorPositionDatas.Values)
                        {
                            house.HideLabels.Add(TextLabelStreamer.Create($" Versteck: {interiorPositionData.Id}", interiorPositionData.Position, dimension:house.Id, color: new Rgba(255, 0, 0, 255)));
                        }
                    }
                    else
                    {
                        //End Modus
                        foreach (var playerLabel in house.HideLabels)
                        {
                            TextLabelStreamer.DestroyDynamicTextLabel(playerLabel);
                        }
                        house.HideLabels = new List<PlayerLabel>();

                    }

                    house.ChangeHideStatus = !house.ChangeHideStatus;
                }
            }
        }

        async void SetHouseHideout(IPlayer player, int houseId, int interiorPositionDataId)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            if (_houses.TryGetValue(houseId, out Models.House house))
            {
                ICollection<HouseInteriorPosition> interiorPositions = house.HouseInteriorPosition;

                //ATM only 1 allowed
                if (interiorPositions.Count == 0)
                {
                    Models.Inventory inventory = await _inventoryHandler.CreateInventory(3);
                    InteriorPositionData interiorPositionData = _interiorModule.GetInteriorPositionDataById(interiorPositionDataId);
                    
                    HouseInteriorPosition houseInteriorPosition = new HouseInteriorPosition();
                    houseInteriorPosition.HouseId = house.Id;
                    houseInteriorPosition.InteriorPositionDataId = interiorPositionDataId;
                    houseInteriorPosition.InventoryId = inventory.Id;
                    RPContext rpContext = new RPContext();
                    houseInteriorPosition.InteriorPositionData = rpContext.InteriorPositionData.FirstOrDefault(d => d.Id == interiorPositionDataId);
                    rpContext.HouseInteriorPosition.Add(houseInteriorPosition);
                    house.HouseInteriorPosition.Add(houseInteriorPosition);
                    await rpContext.SaveChangesAsync();
                    GetHouseHideOptions(player, houseId);
                    await _inventoryHandler.LoadInventory(inventory.Id, houseId);
                }
                else
                {
                    rpPlayer.SendNotification("Du hast bereits ein aktives Versteck.", RPPlayer.NotificationType.ERROR, "Versteck");
                }
            }
        }

        async void BuyHouse(IPlayer player)
        {
            RPPlayer rpPlayer = (RPPlayer) player;

            if (rpPlayer.GetData("houseId", out int houseId))
            {
                Models.House? house = GetHouseById(houseId);
                if (house != null)
                {
                    if (rpPlayer.OwnedHouses.Count() != 0)
                    {
                        rpPlayer.SendNotification("Du hast bereits ein Haus gekauft!", RPPlayer.NotificationType.ERROR, "Haus");
                        return;
                    }

                    await using RPContext rpContext = new RPContext();

                    PlayerHouseOwned playerHouseOwned = rpContext.PlayerHouseOwned.FirstOrDefault(d => d.HouseId == house.Id);
                    if (playerHouseOwned == null)
                    {
                        //No one has bought that house!

                        if (!await rpPlayer.TakeBankMoney(house.HouseData.Price))
                        {
                            rpPlayer.SendNotification("Du hast nicht genügend Geld um das Haus zu kaufen!", RPPlayer.NotificationType.ERROR, "Haus");
                            return;
                        }
                        rpPlayer.SendNotification("Du hast das Haus erfolgreich gekauft!", RPPlayer.NotificationType.SUCCESS, "Haus");
                        rpPlayer.OwnedHouses.Add(houseId);
                        await rpContext.PlayerHouseOwned.AddAsync(new PlayerHouseOwned() {HouseId = houseId, PlayerId = rpPlayer.PlayerId});
                        await rpContext.SaveChangesAsync();
                    }
                }
            }
        }

        async void RentHouse(IPlayer player)
        {
            RPPlayer rpPlayer = (RPPlayer)player;

            if (rpPlayer.GetData("houseId", out int houseId))
            {
                Models.House? house = GetHouseById(houseId);
                if (house != null)
                {
                    if (rpPlayer.RentHouses.ContainsKey(houseId))
                    {
                        rpPlayer.SendNotification("Du bist hier bereits eingemietet!", RPPlayer.NotificationType.ERROR, "Haus");
                        return;
                    }

                    await using RPContext rpContext = new RPContext();
                    int count = rpContext.PlayerHouseRent.Count(d => d.HouseId == houseId);

                    if (count < house.HouseData.RentalPlaces)
                    {
                        rpPlayer.SendNotification("Du hast dich erfolgreich eingemietet.", RPPlayer.NotificationType.SUCCESS, "Haus");
                        await rpContext.PlayerHouseRent.AddAsync(new PlayerHouseRent() {HouseId = houseId, PlayerId = rpPlayer.PlayerId, Cost = 100});
                        await rpContext.SaveChangesAsync();
                        return;
                    }
                    else
                    {
                        rpPlayer.SendNotification("Die Mietplätze sind bereits alle belegt.", RPPlayer.NotificationType.ERROR, "Haus");
                        return;
                    }

                }
            }
        }

        public void OnEntityColshapeHit(IColShape shape, IEntity entity, bool state)
        {
            if (shape.GetData("houseId", out int houseId))
            {
                if (entity is IPlayer player)
                {
                    RPPlayer rpPlayer = (RPPlayer)player;
                    if (state)
                    {
                        rpPlayer.SetData("houseId", houseId);
                        rpPlayer.SendNotification($"Haus ({houseId}) - {_houses[houseId].DoorbellSign}", RPPlayer.NotificationType.INFO);
                    }
                    else
                    {
                        rpPlayer.DeleteData("houseId");
                    }
                }
            }
        }

        public Models.House? GetHouseById(int houseId)
        {
            if (_houses.TryGetValue(houseId, out Models.House house)) return house;
            return null;
        }
    }
}

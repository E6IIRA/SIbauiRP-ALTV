using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using AltV.Net.Elements.Entities;
using GangRP_Server.Core;
using GangRP_Server.Events;
using GangRP_Server.Handlers.Logger;
using GangRP_Server.Handlers.Player;
using GangRP_Server.Handlers.Vehicle;
using GangRP_Server.Models;
using GangRP_Server.Modules.Inventory;
using GangRP_Server.Modules.Player;
using GangRP_Server.Modules.VehicleData;
using GangRP_Server.Utilities.TeamKeyStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Vehicle = GangRP_Server.Models.Vehicle;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Modules.VehicleKey
{
    public class VehicleKeyModule : ModuleBase, IPressedEEvent, ILoadEvent
    {
        private readonly ILogger _logger;
        private readonly RPContext _rpContext;
        private readonly IPlayerHandler _playerHandler;
        private readonly VehicleDataModule _vehicleDataModule;
        private readonly OfflinePlayerModule _offlineOfflinePlayerModule;

        private const int MAXIMUM_VEHICLE_KEYS = 3;

        public IEnumerable<TeamKeyStorageData> _teamKeyStorages;


        public static VehicleKeyModule Instance { get; private set; }

        public VehicleKeyModule(ILogger logger, RPContext rpContext, IPlayerHandler playerHandler, VehicleDataModule vehicleDataModule, OfflinePlayerModule offlineOfflinePlayerModule)
        {
            _logger = logger;
            _rpContext = rpContext;
            _playerHandler = playerHandler;
            _vehicleDataModule = vehicleDataModule;
            _offlineOfflinePlayerModule = offlineOfflinePlayerModule;
            Instance = this;
        }


        public void OnLoad()
        {
            _teamKeyStorages = AddTableLoadEvent<TeamKeyStorageData>(_rpContext.TeamKeyStorageData);
            AddClientEvent<int, int>("TakeKeyFromStorage", TakeVehicleKeyOutOfStorage);
            AddClientEvent<int>("PackVehicleKey", PackVehicleKey);
        }

        public async Task ChangeVehicleLock(RPPlayer rpPlayer, int vehicleId)
        {
            await using RPContext rpContext = new RPContext();
            Vehicle vehicle = await rpContext.Vehicle.FindAsync(vehicleId);
            if (vehicle == null) return;
            vehicle.KeyGeneration += 1;

            var temp = rpContext.PlayerVehicleKey.Where(d => d.VehicleId == vehicleId);
            IQueryable<int> queryable = temp.Select(d => d.PlayerId);
            foreach (var playerId in queryable)
            {
                if (_playerHandler.GetOnlineRPPlayerByPlayerId(playerId) != null)
                {
                    rpPlayer.VehicleKeys = rpPlayer.VehicleKeys.Where(keyId => keyId != vehicleId);
                }
            }
            rpContext.PlayerVehicleKey.RemoveRange(temp);
            await rpContext.SaveChangesAsync();
            rpPlayer.SendNotification($"Das Schloss von  ({vehicleId}) {_vehicleDataModule.GetVehicleDataById(vehicle.VehicleDataId).Name } wurde getauscht.", RPPlayer.NotificationType.SUCCESS);
        }


        public async Task<bool> UseVehicleKey(RPPlayer rpPlayer, int vehicleId, int vehicleGeneration)
        {
            await using RPContext rpContext = new RPContext();
            Models.Vehicle vehicle = await rpContext.Vehicle.FindAsync(vehicleId);
            if (vehicle == null) return false;

            if (vehicle.KeyGeneration != vehicleGeneration)
            {
                rpPlayer.SendNotification("Dieser Schlüssel ist unbrauchbar...", RPPlayer.NotificationType.ERROR);
                return false;
            }


            var keyStorage = _teamKeyStorages.FirstOrDefault(s => (s.TeamId == rpPlayer.TeamId) && (s.Position.Distance(rpPlayer.Position) < 3));
            if (keyStorage != null)
            {
                //Player wants to add this key to key store

                TeamKeyStorage teamKeyStorage = new TeamKeyStorage
                {
                    VehicleId = vehicleId,
                    VehicleKeyGeneration = vehicleGeneration,
                    TeamKeyStorageDataId = keyStorage.Id
                };
                await rpContext.TeamKeyStorage.AddAsync(teamKeyStorage);
                await rpContext.SaveChangesAsync();
                rpPlayer.SendNotification($"Du hast den Schlüssel für ({vehicleId}) {_vehicleDataModule.GetVehicleDataById(vehicle.VehicleDataId).Name} an den Schlüsselkasten gehängt.", RPPlayer.NotificationType.SUCCESS);
                return true;
            }
            else
            {
                //Player wants to use key and add it to own keys
                IQueryable<PlayerVehicleKey> playerVehicleKeys = rpContext.PlayerVehicleKey.Where(d => d.VehicleId == vehicleId);
                //Check how many keys for this vehicle are already in use
                if (playerVehicleKeys.Count() == MAXIMUM_VEHICLE_KEYS)
                {
                    //already too many keys bro...
                    rpPlayer.SendNotification($"Es sind bereits {MAXIMUM_VEHICLE_KEYS} Schlüssel auf dieses Fahrzeug registriert!", RPPlayer.NotificationType.ERROR);
                    return false;
                }
                //check if player has this key already
                if (playerVehicleKeys.FirstOrDefault(d => d.PlayerId == rpPlayer.PlayerId) != null)
                {
                    //player has this key already
                    rpPlayer.SendNotification($"Für dieses Fahrzeug hast du bereits einen Schlüssel am Schlüsselbund!", RPPlayer.NotificationType.ERROR);
                    return false;
                }

                PlayerVehicleKey playerVehicleKey = new PlayerVehicleKey
                {
                    VehicleId = vehicleId,
                    PlayerId = rpPlayer.PlayerId
                };
                await rpContext.PlayerVehicleKey.AddAsync(playerVehicleKey);
                await rpContext.SaveChangesAsync();
                rpPlayer.SendNotification($"Du hast den Schlüssel für ({vehicleId}) {_vehicleDataModule.GetVehicleDataById(vehicle.VehicleDataId).Name} an deinen Schlüsselbund gemacht.", RPPlayer.NotificationType.SUCCESS);
                return true;
            }
        }




        public async void TakeVehicleKeyOutOfStorage(IPlayer player, int teamDataStorageId, int teamKeyStorageId)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            await using RPContext rpContext = new RPContext();
            TeamKeyStorage teamKeyStorage = await rpContext.TeamKeyStorage.Include(d => d.Vehicle).SingleOrDefaultAsync(d => d.VehicleId == teamKeyStorageId);
            if (teamKeyStorage == null) return;

            string vehicleHash = _vehicleDataModule.GetVehicleDataById(teamKeyStorage.Vehicle.VehicleDataId).Name;

            InventoryModule.Instance.AddItem(rpPlayer.Inventory, 4, 1, new[] {$"{vehicleHash}", $"{teamKeyStorage.VehicleId}", $"{teamKeyStorage.Vehicle.KeyGeneration}"});
            rpContext.TeamKeyStorage.Remove(teamKeyStorage);
            await rpContext.SaveChangesAsync();
            rpPlayer.SendNotification($"Du hast den Schlüssel für ({teamKeyStorage.Vehicle.Id}) {vehicleHash} dem Schlüsselkasten entnommen.", RPPlayer.NotificationType.SUCCESS);
        }

        public async void PackVehicleKey(IPlayer player, int vehicleId)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            if (rpPlayer.VehicleKeys.Contains(vehicleId))
            {
                await using RPContext rpContext = new RPContext();

                PlayerVehicleKey playerVehicleKey = await rpContext.PlayerVehicleKey.Include(d => d.Vehicle).FirstOrDefaultAsync(d => (d.VehicleId == vehicleId) && (d.PlayerId == rpPlayer.PlayerId));
                if (playerVehicleKey != null)
                {
                    string vehicleHash = _vehicleDataModule.GetVehicleDataById(playerVehicleKey.Vehicle.VehicleDataId).Name;

                    rpContext.PlayerVehicleKey.Remove(playerVehicleKey);
                    await rpContext.SaveChangesAsync();
                    rpPlayer.SendNotification($"Du hast den Schlüssel für ({playerVehicleKey.VehicleId}) {vehicleHash} vom Schlüsselbund genommen.", RPPlayer.NotificationType.SUCCESS);
                    InventoryModule.Instance.AddItem(rpPlayer.Inventory, 4, 1, new[] { $"{vehicleHash}", $"{playerVehicleKey.VehicleId}", $"{playerVehicleKey.Vehicle.KeyGeneration}" });
                }
            }
        }


        public async Task ShowVehicleKeyStorageAsync(RPPlayer rpPlayer, TeamKeyStorageData teamKeyStorageData)
        {
            await using RPContext rpContext = new RPContext();
            List<VehicleKeyInfo> vehicleKeyInfos = new List<VehicleKeyInfo>();
            foreach (var info in await rpContext.TeamKeyStorage.Where(d => d.TeamKeyStorageDataId == teamKeyStorageData.Id).Include(d => d.Vehicle).ToListAsync())
            {
                _logger.Debug(_vehicleDataModule.GetVehicleDataById(info.Vehicle.VehicleDataId).Name);
                _logger.Debug(_offlineOfflinePlayerModule.GetOfflinePlayerName(info.Vehicle.PlayerId));
                vehicleKeyInfos.Add(new VehicleKeyInfo(info.Id, info.VehicleId, _offlineOfflinePlayerModule.GetOfflinePlayerName(info.Vehicle.PlayerId), _vehicleDataModule.GetVehicleDataById(info.Vehicle.VehicleDataId).Name, info.CreationDate));
            }
            rpPlayer.Emit("ShowIF", "TeamKeyStorage", new TeamKeyStorageWriter(teamKeyStorageData.Id, teamKeyStorageData.Name,vehicleKeyInfos));
        }


        public async Task ShowVehicleKeyOverview(RPPlayer rpPlayer)
        {
            await using RPContext rpContext = new RPContext();
            var include =await rpContext.PlayerVehicleKey.Include(d => d.Vehicle).Where(d => d.PlayerId == rpPlayer.PlayerId).ToListAsync();

            List<VehicleKeyInfo> vehicleKeyInfos = new List<VehicleKeyInfo>();
            foreach (PlayerVehicleKey playerVehicleKey in include)
            {
                vehicleKeyInfos.Add(new VehicleKeyInfo(playerVehicleKey.Id, playerVehicleKey.VehicleId, _offlineOfflinePlayerModule.GetOfflinePlayerName(playerVehicleKey.PlayerId),_vehicleDataModule.GetVehicleDataById(playerVehicleKey.Vehicle.VehicleDataId).Name, playerVehicleKey.CreationDate));
            }
            rpPlayer.Emit("ShowIF", "TeamKeyStorage", new TeamKeyStorageWriter(-1, rpPlayer.Name, vehicleKeyInfos));
        }



        public async Task<bool> OnPressedE(IPlayer player)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            var keyStorage = _teamKeyStorages.FirstOrDefault(s => (s.TeamId == rpPlayer.TeamId) && (s.Position.Distance(player.Position) < 3));
            if (keyStorage == null) return false;
            _logger.Info(player.Name + " opened TeamVehicleKeyStorage " + keyStorage.Id);
            await ShowVehicleKeyStorageAsync(rpPlayer, keyStorage);
            return true;
        }


    }
}

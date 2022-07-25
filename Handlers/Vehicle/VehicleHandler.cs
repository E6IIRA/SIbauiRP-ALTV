using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using AltV.Net.Resources.Chat.Api;
using GangRP_Server.Core;
using GangRP_Server.Events;
using GangRP_Server.Handlers.Inventory;
using GangRP_Server.Handlers.Logger;
using GangRP_Server.Handlers.Player;
using GangRP_Server.Models;
using GangRP_Server.Modules.Garage;
using GangRP_Server.Modules.VehicleData;
using Microsoft.EntityFrameworkCore;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Handlers.Vehicle
{
    public class VehicleHandler : IVehicleHandler, ILoadEvent, IPressedLEvent, IPressedMEvent, IPressedKEvent
    {
        private readonly ILogger _logger;
        private readonly VehicleDataModule _vehicleDataModule;
        private readonly IInventoryHandler _inventoryHandler;

        public readonly Dictionary<int, RPVehicle> RPVehicles = new Dictionary<int, RPVehicle>();


        public VehicleHandler(ILogger logger, VehicleDataModule vehicleDataModule, IInventoryHandler inventoryHandler)
        {
            _logger = logger;
            _vehicleDataModule = vehicleDataModule;
            _inventoryHandler = inventoryHandler;
        }

        public async Task<RPVehicle> CreateVehicle(string model, Position position, Rotation rotation)
        {
            RPVehicle vehicle = (RPVehicle)await AltAsync.CreateVehicle(model, position, rotation);
            _logger.Info("Created vehicle");

            return vehicle;
        }

        public async Task<RPVehicle> CreateVehicle(uint model, Position position, Rotation rotation)
        {
            RPVehicle vehicle = (RPVehicle)await AltAsync.CreateVehicle(model, position, rotation);
            _logger.Info("Created vehicle");

            return vehicle;
        }

        public async Task<RPVehicle> CreateVehicle(VehicleModel model, Position position, Rotation rotation)
        {
            RPVehicle vehicle = (RPVehicle)await AltAsync.CreateVehicle(model, position, rotation);
            _logger.Info("Created vehicle");

            return vehicle;
        }

        public async Task<RPVehicle> CreateVehicleFromDatabaseAtPosition(Models.Vehicle vehicle, Position position, Rotation rotation)
        {
            RPVehicle rpVehicle = await CreateVehicle(vehicle.VehicleData.Hash, position, rotation);
            return await VehicleSetup(rpVehicle, vehicle);
        }

        public async Task<RPVehicle> CreateVehicleFromDatabase(Models.Vehicle vehicle)
        {
            RPVehicle rpVehicle = await CreateVehicle(vehicle.VehicleData.Hash, vehicle.Position, vehicle.Rotation);
            return await VehicleSetup(rpVehicle, vehicle);
        }

        public async Task<RPVehicle> VehicleSetup(RPVehicle rpVehicle, Models.Vehicle vehicle)
        {
            //TODO use AltAsync.Do() ONLY AltV APIS
            if (RPVehicles.ContainsKey(vehicle.Id)) return null;
            RPVehicles.Add(vehicle.Id, rpVehicle);
            rpVehicle.VehicleId = vehicle.Id;
            rpVehicle.OwnerId = vehicle.PlayerId;
            rpVehicle.TeamId = vehicle.TeamDataId;
            rpVehicle.VehicleTunings = vehicle.VehicleTuning;
            rpVehicle.VehicleDataId = vehicle.VehicleDataId;
            rpVehicle.InventoryId = vehicle.InventoryId;
            rpVehicle.Fuel = vehicle.Fuel;

            rpVehicle.Inventory = await _inventoryHandler.LoadInventory(rpVehicle.InventoryId, rpVehicle.VehicleId);

            await AltAsync.Do(() =>
            {
                rpVehicle.SetPrimaryColorAsync(vehicle.ColorPrimary);
                rpVehicle.SetSecondaryColorAsync(vehicle.ColorSecondary);
                rpVehicle.SetNumberplateTextAsync(vehicle.NumberPlate);
                rpVehicle.SetPearlColorAsync(vehicle.ColorPearl);

                if (vehicle.ColorNeonR != 0 || vehicle.ColorNeonG != 0 || vehicle.ColorNeonB != 0 || vehicle.ColorNeonA != 0)
                {
                    rpVehicle.SetNeonColorAsync(new Rgba(vehicle.ColorNeonR, vehicle.ColorNeonG, vehicle.ColorNeonB, vehicle.ColorNeonA));
                    rpVehicle.SetNeonActiveAsync(true, true, true, true);
                }
                rpVehicle.SetBodyHealthAsync(vehicle.BodyHealth);
                rpVehicle.SetEngineHealthAsync(vehicle.EngineHealth);
                rpVehicle.SetPetrolTankHealthAsync(vehicle.PetrolTankHealth);



                rpVehicle.SetModKitAsync(1);
                foreach (VehicleTuning vehicleTuning in rpVehicle.VehicleTunings)
                {
                    rpVehicle.SetModAsync(vehicleTuning.VehicleTuningDataId, vehicleTuning.Value);
                }
                rpVehicle.SetStreamSyncedMetaData("speed", vehicle.VehicleData.Multiplier);
            });



            return rpVehicle;
        }


        public async Task SaveVehicle(RPVehicle rpVehicle)
        {
            await using RPContext rpContext = new RPContext();
            Models.Vehicle veh = rpContext.Vehicle.Find(rpVehicle.VehicleId);
            if (veh != null)
            {
                veh.PositionX = rpVehicle.Position.X;
                veh.PositionY = rpVehicle.Position.Y;
                veh.PositionZ = rpVehicle.Position.Z;

                veh.RotationPitch = rpVehicle.Rotation.Pitch;
                veh.RotationRoll = rpVehicle.Rotation.Roll;
                veh.RotationYaw = rpVehicle.Rotation.Yaw;
                veh.EngineHealth = rpVehicle.EngineHealth;
                veh.BodyHealth = rpVehicle.BodyHealth;
                veh.PetrolTankHealth = rpVehicle.PetrolTankHealth;
                veh.Fuel = rpVehicle.Fuel;
            }

            await rpContext.SaveChangesAsync();
        }



        public async Task SaveAllVehiclesToDb()
        {
            await using RPContext rpContext = new RPContext();
            foreach (var rpVehicle in RPVehicles.Values)
            {
                if (rpVehicle.VehicleId == 0) continue;

                lock (rpVehicle)
                {
                    Models.Vehicle veh = rpContext.Vehicle.Find(rpVehicle.VehicleId);
                    if (veh == null) continue;
                    veh.PositionX = rpVehicle.Position.X;
                    veh.PositionY = rpVehicle.Position.Y;
                    veh.PositionZ = rpVehicle.Position.Z;

                    veh.RotationPitch = rpVehicle.Rotation.Pitch;
                    veh.RotationRoll = rpVehicle.Rotation.Roll;
                    veh.RotationYaw = rpVehicle.Rotation.Yaw;
                    veh.EngineHealth = rpVehicle.EngineHealth;
                    veh.BodyHealth = rpVehicle.BodyHealth;
                    veh.PetrolTankHealth = rpVehicle.PetrolTankHealth;
                    veh.Fuel = rpVehicle.Fuel;
                }
            }
            await rpContext.SaveChangesAsync();
        }


        public async void OnLoad()
        {
            await using RPContext rpContext = new RPContext();

            foreach (var vehicle in await rpContext.Vehicle.Where(v => !v.InGarage)
                .Include(data => data.VehicleData)
                .Include(t => t.VehicleTuning).ThenInclude(d => d.VehicleTuningData).ToListAsync())
            {
                await CreateVehicleFromDatabase(vehicle);
            }
        }

        public async Task<Models.Vehicle> AddVehicleToDatabase(int rpPlayerId, int vehicleDataId, Position position)
        {
            Models.Vehicle vehicle = new Models.Vehicle
            {
                PlayerId = rpPlayerId,
                VehicleDataId = vehicleDataId,
                GarageDataId = 1,
                ColorPrimary = 1,
                ColorSecondary = 1,
                NumberPlate = "",
                InGarage = false
            };

            await using RPContext rpContext = new RPContext();
            await rpContext.Vehicle.AddAsync(vehicle);
            await rpContext.SaveChangesAsync();

            var temp = await rpContext.Vehicle
                .Include(data => data.VehicleData).Where(p => p.PlayerId == rpPlayerId)
                .Include(t => t.VehicleTuning).ThenInclude(d => d.VehicleTuningData).OrderByDescending(d => d.Id).FirstOrDefaultAsync();

            await CreateVehicleFromDatabaseAtPosition(temp, position, new Rotation());
            return null;
        }


        public async Task<List<Models.Vehicle>> GetVehiclesInGarage(RPPlayer rpPlayer, GarageData garageData)
        {
            await using RPContext rpContext = new RPContext();

            List<Models.Vehicle> vehicles = await rpContext.Vehicle.Where(p => (p.PlayerId == rpPlayer.PlayerId || (p.TeamDataId == rpPlayer.TeamId && rpPlayer.TeamId != 1)) && (p.InGarage) && (p.GarageDataId == garageData.Id))
                .Include(v => v.VehicleData).ToListAsync();
            List<PlayerVehicleKey> vehicleKeys = await rpContext.PlayerVehicleKey.Where(p => (p.PlayerId == rpPlayer.PlayerId) && (p.Vehicle.InGarage) && (p.Vehicle.GarageDataId == garageData.Id))
                .Include(d => d.Vehicle).ThenInclude(d => d.VehicleData)
                .ToListAsync();
            vehicles.AddRange(vehicleKeys.Select(vehicleKey => vehicleKey.Vehicle));
            return vehicles;
        }


        public async void ParkVehicleIntoGarage(RPPlayer rpPlayer, int vehicleId, GarageData garageData)
        {
            RPVehicle rpVehicle = GetRpVehicle(vehicleId);
            if (rpVehicle == null) return;

            if (garageData.VehicleClassificationHashSet.Contains(_vehicleDataModule.GetVehicleDataById(rpVehicle.VehicleDataId).ClassificationId) && rpPlayer.CanControlVehicle(rpVehicle))
            {
                await rpVehicle.SetIntoGarage(garageData);
                await SaveVehicle(rpVehicle);
                _inventoryHandler.UnloadInventory(rpVehicle.InventoryId);
                RemoveRpVehicle(vehicleId);
                rpPlayer.SendNotification($"Du hast den {_vehicleDataModule.GetVehicleDataById(rpVehicle.VehicleDataId).Name } ({rpVehicle.VehicleId}) in die {garageData.Name} Garage eingeparkt", RPPlayer.NotificationType.SUCCESS);
            }
        }

        public async Task TakeVehicleOutOfGarage(RPPlayer rpPlayer, int vehicleId, GarageData garageData)
        {
            await using RPContext rpContext = new RPContext();
            
            Models.Vehicle vehicle = await rpContext.Vehicle.FindAsync(vehicleId);
            if (vehicle == null) return;
            if (!vehicle.InGarage) return;

            rpContext.Entry(vehicle).Reference("VehicleData").Load();

            //TODO Check if player is able to control vehicle

            var garagespawnData = GetFreeGarageSpawnData(garageData);

            if (garagespawnData != null)
            {
                RPVehicle rpVehicle = await CreateVehicleFromDatabaseAtPosition(vehicle, garagespawnData.Position, garagespawnData.Rotation);
                if (rpVehicle == null) return;
                rpPlayer.SendNotification($"Du hast den {_vehicleDataModule.GetVehicleDataById(rpVehicle.VehicleDataId).Name } ({rpVehicle.VehicleId}) aus der  {garageData.Name} Garage ausgeparkt", RPPlayer.NotificationType.SUCCESS);
                vehicle.InGarage = false;
                vehicle.PositionX = garagespawnData.PositionX;
                vehicle.PositionY = garagespawnData.PositionY;
                vehicle.PositionZ = garagespawnData.PositionZ;
                await rpContext.SaveChangesAsync();
            }
            else
            {
                //Kein freier Ausparkpunkt frei
                rpPlayer.SendNotification("Es ist kein freier Ausparkpunkt mehr frei!", RPPlayer.NotificationType.ERROR);
                return;
            }
        }

        private GaragespawnData GetFreeGarageSpawnData(GarageData garageData)
        {
            foreach (var garagespawnData in garageData.GaragespawnData)
            {
                bool found = false;
                foreach (var vehicle in Alt.Server.GetVehicles())
                {
                    if (vehicle?.Position.Distance(garagespawnData.Position) <= 2.0f)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    return garagespawnData;
                }
            }
            return null;
        }



        public bool OnPressedL(IPlayer player)
        {
            RPVehicle rpVehicle;

            if (player.IsInVehicle)
            {
                rpVehicle = (RPVehicle) player.Vehicle;
            }
            else
            {
                rpVehicle = GetClosestRpVehicle(player.Position);
            }

            if (rpVehicle == null) return false;
            RPPlayer rpPlayer = (RPPlayer) player;

            rpPlayer.SendNotification($"{rpVehicle.BodyHealth}, {rpVehicle.EngineHealth}, {rpVehicle.PetrolTankHealth}", RPPlayer.NotificationType.SUCCESS, "");

            //Autos aus der Datenbank
            if (rpPlayer.CanControlVehicle(rpVehicle))
            {
                //Spieler darf Fahrzeug bedienen

                if (rpVehicle.Locked)
                    rpPlayer.SendNotification($"Fahrzeug aufgeschlossen", RPPlayer.NotificationType.SUCCESS, $"({rpVehicle.VehicleId}) - {_vehicleDataModule.GetVehicleDataById(rpVehicle.VehicleDataId).Name}");
                else rpPlayer.SendNotification($"Fahrzeug abgeschlossen", RPPlayer.NotificationType.ERROR, $"({rpVehicle.VehicleId}) - {_vehicleDataModule.GetVehicleDataById(rpVehicle.VehicleDataId).Name}");

                rpVehicle.Locked = !rpVehicle.Locked;

                return true;
            }
            return false;
        }

        public bool OnPressedK(IPlayer player)
        {
            RPVehicle rpVehicle;

            if (player.IsInVehicle)
            {
                rpVehicle = (RPVehicle)player.Vehicle;
            }
            else
            {
                rpVehicle = GetClosestRpVehicle(player.Position);
            }

            if (rpVehicle == null) return false;
            RPPlayer rpPlayer = (RPPlayer)player;

            if (!rpVehicle.Locked)
            {
                if (rpVehicle.TrunkStatus)
                    rpPlayer.SendNotification($"Kofferraum geschlossen", RPPlayer.NotificationType.ERROR, $"({rpVehicle.VehicleId}) - {_vehicleDataModule.GetVehicleDataById(rpVehicle.VehicleDataId).Name}");
                else rpPlayer.SendNotification($"Kofferraum geöffnet", RPPlayer.NotificationType.SUCCESS, $"({rpVehicle.VehicleId}) - {_vehicleDataModule.GetVehicleDataById(rpVehicle.VehicleDataId).Name}");

                rpVehicle.TrunkStatus = !rpVehicle.TrunkStatus;

                return true;
            }
            return false;
        }




        public bool OnPressedM(IPlayer player)
        {
            if (!player.IsInVehicle) return false;
            RPVehicle rpVehicle = (RPVehicle) player.Vehicle;
            RPPlayer rpPlayer = (RPPlayer) player;

            if (rpVehicle.Engine)
                rpPlayer.SendNotification($"Motor ausgeschalten", RPPlayer.NotificationType.ERROR, $"({rpVehicle.VehicleId}) - {_vehicleDataModule.GetVehicleDataById(rpVehicle.VehicleDataId).Name}");
            else rpPlayer.SendNotification($"Motor gestartet", RPPlayer.NotificationType.SUCCESS, $"({rpVehicle.VehicleId}) - {_vehicleDataModule.GetVehicleDataById(rpVehicle.VehicleDataId).Name}");
            rpVehicle.Engine = !rpVehicle.Engine;
            return false;
        }

        public RPVehicle? GetRpVehicle(int vehicleId)
        {
            if (RPVehicles.TryGetValue(vehicleId, out RPVehicle rpVehicle))
            {
                return rpVehicle;
            }
            return null;
        }

        public void RemoveRpVehicle(int vehicleId)
        {
            if (RPVehicles.ContainsKey(vehicleId))
            {
                RPVehicle rpVehicle = RPVehicles[vehicleId];
                rpVehicle.Remove();
                RPVehicles.Remove(vehicleId);
            }
        }

        public Dictionary<int, RPVehicle> GetVehicles()
        {
            return this.RPVehicles;
        }

        public RPVehicle GetClosestRpVehicle(Position position, int distance = 2)
        {
            return this.RPVehicles.Values.FirstOrDefault(d => d.Position.Distance(position) < distance);
        }

        public RPVehicle GetClosestTeamRpVehicle(Position position, int teamId, int distance = 2)
        {
            return this.RPVehicles.Values.FirstOrDefault(d => (d.Position.Distance(position) < distance) && (d.TeamId == teamId));
        }

        public IEnumerable<RPVehicle> GetRpVehiclesInRange(Position position, int range = 2)
        {
            return this.RPVehicles.Values.Where(d => d.Position.Distance(position) < range);
        }
    }
}

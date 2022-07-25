using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using GangRP_Server.Core;
using GangRP_Server.Events;
using GangRP_Server.Handlers.Logger;
using GangRP_Server.Models;
using GangRP_Server.Modules.Bank;
using GangRP_Server.Modules.Cloth;
using GangRP_Server.Modules.Drug;
using GangRP_Server.Modules.Farming;
using GangRP_Server.Modules.Fuelstation;
using GangRP_Server.Modules.Garage;
using GangRP_Server.Modules.House;
using GangRP_Server.Modules.Inventory;
using GangRP_Server.Modules.ServerScenario;
using GangRP_Server.Modules.Shop;
using GangRP_Server.Modules.StorageRoom;
using GangRP_Server.Modules.Tuning;
using GangRP_Server.Modules.WareExport;
using GangRP_Server.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Modules.Command
{
    public sealed class CommandModule : ModuleBase, ILoadEvent, IConsoleCommandEvent
    {
        private readonly ILogger _logger;
        private readonly InventoryModule _inventoryModule;
        private readonly FarmingModule _farmingModule;
        private readonly HouseModule _houseModule;
        private readonly GarageDataModule _garageDataModule;
        private readonly BankModule _bankModule;
        private readonly ShopModule _shopModule;
        private readonly PlantModule _plantModule;
        private readonly WareExportModule _wareExportModule;
        private readonly FuelstationDataModule _fuelstationDataModule;
        private readonly StorageRoomModule _storageRoomModule;
        private readonly ServerScenarioModule _serverScenarioModule;
        private readonly DrugCamperModule _drugCamperModule;
        private readonly TuningModule _tuningModule;
        public CommandModule(ILogger logger, InventoryModule inventoryModule, FarmingModule farmingModule, HouseModule houseModule, GarageDataModule garageDataModule, BankModule bankModule, ShopModule shopModule, PlantModule plantModule, WareExportModule wareExportModule, StorageRoomModule storageRoomModule, FuelstationDataModule fuelstationDataModule, ServerScenarioModule serverScenarioModule, DrugCamperModule drugCamperModule, TuningModule tuningModule)
        {
            _logger = logger;
            _inventoryModule = inventoryModule;
            _farmingModule = farmingModule;
            _houseModule = houseModule;
            _garageDataModule = garageDataModule;
            _bankModule = bankModule;
            _shopModule = shopModule;
            _plantModule = plantModule;
            _wareExportModule = wareExportModule;
            _fuelstationDataModule = fuelstationDataModule;
            _serverScenarioModule = serverScenarioModule;
            _drugCamperModule = drugCamperModule;
            _tuningModule = tuningModule;
            _storageRoomModule = storageRoomModule;
        }

        public void OnLoad()
        {
            AddClientEvent<string>("chat:message", ChatMessage);
        }

        async void ChatMessage(IPlayer player, string message)
        {
            _logger.Info(player.Name + " typed " + message);

            var args = message.Split(" ");
            if (args.Length < 1) return;


            if (args[0].StartsWith("/"))
            {
                var command = args[0].Substring(1);
                args = args.Skip(1).ToArray();

                await OnPlayerCommand(player, command, args);
            }
        }

        private async Task OnPlayerCommand(IPlayer player, string command, string[] args)
        {
            var rpPlayer = (RPPlayer)player;

            if (command.Equals("view"))
            {
                if (args.Length == 2)
                {
                    rpPlayer.UpdateView(args[0], args[1]);
                }

                return;
            }
            else if (command.Equals("ooc"))
            {
                foreach (var p in Alt.Server.GetPlayers())
                {
                    if (p.Position.Distance(player.Position) < 30)
                    {
                        RPPlayer pPlayer = (RPPlayer) p;
                        pPlayer.SendNotification(String.Join(" ", args), RPPlayer.NotificationType.OOC, $"{rpPlayer.Name}");
                    }
                }
            }

            else if (command.Equals("giveitem"))
            {
                ItemData? itemData;
                int amount = 0;
                if (args.Length == 1)
                {
                    itemData = _inventoryModule.GetItemDataByName(args[0]);
                    amount = 1;
                }
                else if (args.Length == 2)
                {
                    itemData = _inventoryModule.GetItemDataByName(args[0]);
                    if (!int.TryParse(args[1], out amount)) return;
                }
                else if (args.Length == 3)
                {
                    rpPlayer = (RPPlayer)Alt.Server.GetPlayers().FirstOrDefault(p => p.Name.ToLower().Contains(args[0].ToLower()));
                    itemData = _inventoryModule.GetItemDataByName(args[1]);
                    if (!int.TryParse(args[2], out amount)) return;
                }
                else
                {
                    return;
                }

                if (itemData == null) return;

                await _inventoryModule.AddItem(rpPlayer.Inventory, itemData.Id, amount);

                return;
            }
            else if (command.Equals("kick"))
            {
                RPPlayer targetRpPlayer = (RPPlayer)Alt.Server.GetPlayers().FirstOrDefault(p => p.Name.ToLower().Contains(args[0].ToLower()));

                if (targetRpPlayer == null) return;
                targetRpPlayer.Kick("bye");
            }
            
            else if (command.Equals("plant_seed"))
            {
                int plantTypeId = 1;
                if (args.Length == 1)
                    if (!int.TryParse(args[0], out plantTypeId)) return;
                _plantModule.CreatePlant(rpPlayer, plantTypeId);
                return;
            }

            else if (command.Equals("plant_water"))
            {
                _plantModule.WaterPlant(rpPlayer);
                return;
            }

            else if (command.Equals("plant_fertilizer"))
            {
                _plantModule.FertilizePlant(rpPlayer);
                return;
            }

            else if (command.Equals("plant_harvest"))
            {
                await _plantModule.HarvestPlant(rpPlayer);
                return;
            }

            else if (command.Equals("plant_save"))
            {
                await _plantModule.SavePlants();
                return;
            }

            else if (command.Equals("plant_grow"))
            {
                Plant plant = _plantModule.GetPlant(player);
                if (plant == null) return;
                _plantModule.GrowPlant(plant);
                return;
            }

            else if (command.Equals("camper_step"))
            {
                _drugCamperModule.ProcessCommand();
                return;
            }

            else if (command.Equals("camper_test"))
            {
                foreach (var keyValuePair in _drugCamperModule.ProcessingVehicles)
                {
                    _logger.Info(keyValuePair.Value.NeededItems.Count.ToString());
                }
                return;
            }

            else if (command.Equals("tuning_tune"))
            {
                _tuningModule.CommandTune(rpPlayer);
                return;
            }

            else if (command.Equals("export_setpoint"))
            {
                if (args.Length == 3)
                {
                    if (!int.TryParse(args[0], out int itemId)) return;
                    if (!int.TryParse(args[1], out int price)) return;
                    _wareExportModule.SetNextSetpoint(player, itemId, price, args[2]);
                }
            }

            else if (command.Equals("export_evaluate"))
            {
                _wareExportModule.EvaluatePrices(player);
            }
            else if (command.Equals("gotoclothshop"))
            {
                if (args.Length == 1)
                {
                    if (int.TryParse(args[0], out int clothShopId))
                    {
                        ClothShopData shopData = ClothModule.Instance._clothShops.FirstOrDefault(d => d.Id == clothShopId);
                        if (shopData != null)
                        {
                            await rpPlayer.SetPositionAsync(shopData.Position);
                        }
                    }
                }
            }

            else if (command.Equals("gotohouse"))
            {
                if (args.Length == 1)
                {
                    if (int.TryParse(args[0], out int houseId))
                    {
                        Models.House house = _houseModule.GetHouseById(houseId);
                        if (house != null)
                        {
                            await rpPlayer.SetPositionAsync(house.HouseData.Position);
                        }
                        
                    }
                }
            }
            else if (command.Equals("createhouse"))
            { 
                await using RPContext rpContext = new RPContext();

                HouseData houseData = new HouseData();
                houseData.PositionX = rpPlayer.Position.X;
                houseData.PositionY = rpPlayer.Position.Y;
                houseData.PositionZ = rpPlayer.Position.Z;

                houseData.RotationRoll = rpPlayer.Rotation.Roll;
                houseData.RotationPitch = rpPlayer.Rotation.Pitch;
                houseData.RotationYaw = rpPlayer.Rotation.Yaw;

                houseData.RentalPlaces = 0;
                houseData.Price = 0;
                houseData.InteriorDataId = 1;
                houseData.HouseAreaDataId = 1;
                houseData.HouseAppearanceDataId = 1;
                houseData.HouseSizeDataId = 1;

                await rpContext.HouseData.AddAsync(houseData);
                await rpContext.SaveChangesAsync();

                Models.House house = new Models.House() {HouseDataId = houseData.Id, DoorbellSign = "Haus", Money = 0};
                await rpContext.House.AddAsync(house);
                await rpContext.SaveChangesAsync();

                house.DoorbellSign = $"Haus {house.Id}";
                await rpContext.SaveChangesAsync();

                TextLabelStreamer.Create($"RESTART REQUIRED House Id: {house.Id}", house.HouseData.Position, color: new Rgba(255, 255, 0, 255));

            }
            else if (command.Equals("creategarage"))
            {
                if (args.Length == 1)
                {
                    if (int.TryParse(args[0], out int houseId))
                    {
                        await using RPContext rpContext = new RPContext();
                        Models.House house = rpContext.House.Find(houseId);
                        if (house != null)
                        {
                            GarageData garageData = new GarageData();
                            garageData.PositionX = rpPlayer.Position.X;
                            garageData.PositionY = rpPlayer.Position.Y;
                            garageData.PositionZ = rpPlayer.Position.Z;
                            garageData.Name = $"Hausgarage {houseId}";
                            garageData.Rotation = 0;
                            garageData.Type = 1;
                            garageData.HasMarker = false;
                            garageData.PedHash = "";
                            garageData.Radius = 25;
                            garageData.VehicleClassifications = "";

                            await rpContext.GarageData.AddAsync(garageData);
                            await rpContext.SaveChangesAsync();
                            TextLabelStreamer.Create($"RESTART REQUIRED Garage Id: {garageData.Id}", garageData.Position, color: new Rgba(255, 255, 0, 255));

                            await rpContext.HouseGarageData.AddAsync(new HouseGarageData() {GarageDataId = garageData.Id, HouseDataId = house.HouseDataId});
                            await rpContext.SaveChangesAsync();
                            rpPlayer.SendNotification("Garage erfolgreich erstellt.", RPPlayer.NotificationType.SUCCESS, "Garage");
                        }
                        else
                        {
                            rpPlayer.SendNotification("Das Haus gibts net alla", RPPlayer.NotificationType.ERROR, "Error");
                        }
                    }
                }
            }
            else if (command.Equals("creategaragespawn"))
            {
                if (args.Length == 1)
                {
                    if (int.TryParse(args[0], out int garageId))
                    {
                        if (rpPlayer.IsInVehicle)
                        {
                            await using RPContext rpContext = new RPContext();

                            
                            GarageData garageData = rpContext.GarageData.Find(garageId);
                            if (garageData != null)
                            {
                                GaragespawnData garagespawnData = new GaragespawnData();
                                garagespawnData.GarageId = garageId;
                                garagespawnData.PositionX = rpPlayer.Position.X;
                                garagespawnData.PositionY = rpPlayer.Position.Y;
                                garagespawnData.PositionZ = rpPlayer.Position.Z;
                                garagespawnData.RotationX = rpPlayer.Vehicle.Rotation.Roll;
                                garagespawnData.RotationY = rpPlayer.Vehicle.Rotation.Pitch;
                                garagespawnData.RotationZ = rpPlayer.Vehicle.Rotation.Yaw;

                                await rpContext.GaragespawnData.AddAsync(garagespawnData);
                                await rpContext.SaveChangesAsync();
                                rpPlayer.SendNotification("Garagen Ausparkpunkt erfolgreich erstellt.", RPPlayer.NotificationType.SUCCESS, "Garage");
                                TextLabelStreamer.Create($"RESTART REQUIRED GarageSpawn Id: {garagespawnData.Id}", garagespawnData.Position, color: new Rgba(255, 255, 0, 255));
                                Alt.Server.CreateVehicle(2891838741, garagespawnData.Position, garagespawnData.Rotation);
                            }
                            else
                            {
                                rpPlayer.SendNotification("Die Garage gibts net alla", RPPlayer.NotificationType.ERROR, "Error");
                            }
                        }
                        else
                        {
                            rpPlayer.SendNotification("Du musst im Fahrzeug sitzen alla, so wie das Fahrzeug spawnen soll!", RPPlayer.NotificationType.ERROR, "Error blyat");
                        }
                    }
                }
            }

            else if (command.Equals("gotogarage"))
            {
                if (args.Length == 1)
                {
                    GarageData garageData = _garageDataModule._garages.Values.FirstOrDefault(d => d.Name.ToLower().Contains(args[0].ToLower()));


                    if (garageData != null)
                    {
                        await rpPlayer.SetPositionAsync(garageData.Position);
                    }


                }
            }
            else if (command.Equals("gotobank"))
            {
                if (args.Length == 1)
                {
                    if (int.TryParse(args[0], out int bankId))
                    {
                        BankData bank = _bankModule.GetBankById(bankId);
                        if (bank != null)
                        {
                            await rpPlayer.SetPositionAsync(bank.Position);
                        }

                    }
                }
            }
            else if (command.Equals("gotoshop"))
            {
                if (args.Length == 1)
                {
                    if (int.TryParse(args[0], out int shopId))
                    {
                        Models.ShopData shopById = _shopModule.GetShopById(shopId);
                        if (shopById != null)
                        {
                            await rpPlayer.SetPositionAsync(shopById.Position);
                        }

                    }
                }
            }
            else if (command.Equals("gotostorage"))
            {
                if (args.Length == 1)
                {
                    if (int.TryParse(args[0], out int storageId))
                    {
                        Storageroom storageroom = _storageRoomModule.GetStorageById(storageId);
                        if (storageroom != null && storageroom.StorageroomData != null)
                        {
                            await rpPlayer.SetPositionAsync(storageroom.StorageroomData.Position);
                        }

                    }
                }
            }
            else if (command.Equals("storage_create"))
            {
                _storageRoomModule.CreateStorageroom(rpPlayer);
            }
            else if (command.Equals("storage_upgrade"))
            {
                Storageroom storageroom = _storageRoomModule.GetStorageById(rpPlayer.Dimension);
                if (storageroom != null)
                {
                    if (int.TryParse(args[0], out int crates))
                    {
                        _storageRoomModule.UpgradeStorageroom(rpPlayer, storageroom.Id, crates);
                    }
                }
            }
            else if (command.Equals("scenario_start"))
            {
                if (args.Length == 1)
                {
                    if (int.TryParse(args[0], out int serverScenarioDataId))
                    {
                        ServerScenarioData serverScenarioData =
                            _serverScenarioModule.GetServerScenarioById(serverScenarioDataId);
                        if (serverScenarioData != null && !serverScenarioData.IsActive)
                        {
                            _serverScenarioModule.SpawnScenario(serverScenarioData);
                        }
                    }
                }
            }
            else if (command.Equals("scenario_stop"))
            {
                if (args.Length == 1)
                {
                    if (int.TryParse(args[0], out int serverScenarioDataId))
                    {
                        ServerScenarioData serverScenarioData =
                            _serverScenarioModule.GetServerScenarioById(serverScenarioDataId);
                        if (serverScenarioData != null && serverScenarioData.IsActive)
                        {
                            _serverScenarioModule.DespawnScenario(serverScenarioData);
                        }
                    }
                }
            }
            else if (command.Equals("gotofuel"))
            {
                if (args.Length == 1)
                {
                    if (int.TryParse(args[0], out int fuelstationId))
                    {
                        Models.FuelstationData fuelstatioById = _fuelstationDataModule.GetFuelstationDataById(fuelstationId);
                        if (fuelstatioById != null)
                        {
                            await rpPlayer.SetPositionAsync(fuelstatioById.Position);
                        }

                    }
                }
            }

            /*else if (command.Equals("showfarmprops"))
            {
                _farmingModule.ShowAllFarmFieldProps();

                return;
            }*/

            return;
        }

        public void OnConsoleCommand(string name, string[] args)
        {
            switch (name)
            {
                case "count":
                    _logger.Info("Spieler online: " + Alt.GetAllPlayers().Count);
                    break;
                default:
                    break;
            }
        }
    }
}

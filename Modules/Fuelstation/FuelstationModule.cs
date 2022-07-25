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
using GangRP_Server.Handlers.Vehicle;
using GangRP_Server.Models;
using GangRP_Server.Modules.Interior;
using GangRP_Server.Modules.Player;
using GangRP_Server.Modules.VehicleData;
using GangRP_Server.Utilities;
using GangRP_Server.Utilities.Cloth;
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
namespace GangRP_Server.Modules.Fuelstation
{
    public sealed class FuelstationModule : ModuleBase, ILoadEvent, IEntityColshapeHitEvent
    {
        private readonly RPContext _rpContext;
        private readonly IPlayerHandler _playerHandler;
        private readonly FuelstationDataModule _fuelstationDataModule;
        private readonly IVehicleHandler _vehicleHandler;
        private readonly VehicleDataModule _vehicleDataModule;

        public FuelstationModule(RPContext rpContext, IPlayerHandler playerHandler, FuelstationDataModule fuelstationDataModule, IVehicleHandler vehicleHandler, VehicleDataModule vehicleDataModule)
        {
            _rpContext = rpContext;
            _playerHandler = playerHandler;
            _fuelstationDataModule = fuelstationDataModule;
            _vehicleHandler = vehicleHandler;
            _vehicleDataModule = vehicleDataModule;
        }

        public void OnLoad()
        {
            AddClientEvent<int,int, int, int>("DoRefuel", RefuelVehicle);
        }

        public void OnEntityColshapeHit(IColShape shape, IEntity entity, bool state)
        {
            if (shape.GetData("fuelstationId", out int fuelstationId))
            {
                if (entity is IPlayer player)
                {
                    RPPlayer rpPlayer = (RPPlayer)player;
                    if (state)
                    {
                        rpPlayer.SetData("fuelstationId", fuelstationId);
                        FuelstationData fuelstationData = _fuelstationDataModule.GetFuelstationDataById(fuelstationId);
                        rpPlayer.SendNotification($"Fuelstation ({fuelstationId}) $ {fuelstationData.Price}/Liter - {fuelstationData.Name}", RPPlayer.NotificationType.INFO);
                    }
                    else
                    {
                        rpPlayer.DeleteData("fuelstationId");
                    }
                }
            }
        }

        async void RefuelVehicle(IPlayer player, int fuelStationId, int gaspumpDataId, int fuel, int vehicleId)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            if (_fuelstationDataModule._fuelstationDatas.TryGetValue(fuelStationId, out var tuple))
            {
                if (tuple.Item2.TryGetValue(gaspumpDataId, out var fuelstationGaspumpData))
                {
                    bool status = await rpPlayer.StartTask(5000);
                    if (status)
                    {
                        RPVehicle? rpVehicle = _vehicleHandler.GetRpVehicle(vehicleId);

                        if (rpVehicle != null)
                        {
                            if (rpVehicle.Position.Distance(fuelstationGaspumpData.Position) < 5)
                            {
                                int maxFuel = _vehicleDataModule.GetVehicleDataById(rpVehicle.VehicleDataId).MaxFuel;

                                if (rpVehicle.Fuel + fuel >= maxFuel)
                                {
                                    fuel = (int) (maxFuel - rpVehicle.Fuel);
                                }

                                int price = fuel * tuple.Item1.Price;
                                if (await rpPlayer.TakeBankMoney(price))
                                {
                                    rpPlayer.SendNotification($"Fahrzeug getankt : +{fuel} Liter", RPPlayer.NotificationType.SUCCESS, $"{tuple.Item1.Name}");
                                    rpVehicle.Fuel += fuel;
                                }
                                else
                                {
                                    rpPlayer.SendNotification($"Du hast nicht genügend Geld. Benötigt: {price}", RPPlayer.NotificationType.ERROR, $"{tuple.Item1.Name}");
                                    return;
                                }

                            }
                        }
                        else
                        {
                            rpPlayer.SendNotification($"Tankvorgang konnte nicht abgeschlossen werden.", RPPlayer.NotificationType.ERROR, $"{tuple.Item1.Name}");
                            return;
                        }
                    }
                    else
                    {
                        rpPlayer.SendNotification("Tankvorgang abgebrochen", RPPlayer.NotificationType.ERROR, $"{tuple.Item1.Name}");
                        return;
                    }
                }
            }
        }
    }
}

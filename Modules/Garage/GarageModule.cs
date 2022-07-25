using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using GangRP_Server.Core;
using GangRP_Server.Events;
using GangRP_Server.Handlers.Logger;
using GangRP_Server.Handlers.Player;
using GangRP_Server.Handlers.Vehicle;
using GangRP_Server.Models;
using GangRP_Server.Modules.House;
using GangRP_Server.Modules.VehicleData;
using GangRP_Server.Utilities;
using GangRP_Server.Utilities.Blip;
using GangRP_Server.Utilities.Vehicle;
using Microsoft.EntityFrameworkCore;
using Vehicle = AltV.Net.Elements.Entities.Vehicle;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Modules.Garage
{
    public sealed class GarageModule : ModuleBase, ILoadEvent, IPressedEEvent, IPlayerConnectEvent
    {
        private readonly ILogger _logger;
        private readonly RPContext _rpContext;
        private readonly IVehicleHandler _vehicleHandler;
        private readonly VehicleDataModule _vehicleDataModule;
        private readonly HouseModule _houseModule;
        private readonly GarageDataModule _garageDataModule;

        public static GarageModule Instance { get; private set; }

        public GarageModule(ILogger logger, RPContext rpContext, IVehicleHandler vehicleHandler, VehicleDataModule vehicleDataModule, HouseModule houseModule, GarageDataModule garageDataModule)
        {
            _logger = logger;
            _rpContext = rpContext;
            _vehicleHandler = vehicleHandler;
            _vehicleDataModule = vehicleDataModule;
            _garageDataModule = garageDataModule;
            _houseModule = houseModule;
            Instance = this;
        }

        public void OnLoad()
        {
            AddClientEvent<int, int>("ParkOut", ParkOutVehicle);
            AddClientEvent<int, int>("ParkIn", ParkInVehicle);
            AddClientEvent<int>("GetInparkVehicles", GetInparkVehicles);
        }

        void GetInparkVehicles(IPlayer player, int garageId)
        {
            RPPlayer rpPlayer = (RPPlayer)player;

            GarageData garage = _garageDataModule.GetGarageDataById(garageId);
            if (garage == null) return;

            List<GarageVehicle> garageVehicles = new List<GarageVehicle>();
            foreach (var rpVehicle in _vehicleHandler.GetRpVehiclesInRange(garage.Position, garage.Radius))
            {
                if (rpVehicle.VehicleId != 0 && garage.VehicleClassificationHashSet.Contains(_vehicleDataModule.GetVehicleDataById(rpVehicle.VehicleDataId).ClassificationId) && rpPlayer.CanControlVehicle(rpVehicle))
                {
                    garageVehicles.Add(new GarageVehicle(rpVehicle.VehicleId, _vehicleDataModule.GetVehicleDataById(rpVehicle.VehicleDataId).Name));
                }
            }

            player.Emit("UpdateView", "SendInparkVehicles", new GarageVehicleWriter(garageVehicles, garage.Id, garage.Name));


        }

        async void ParkOutVehicle(IPlayer player, int vehicleId, int garageId)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            GarageData garage = _garageDataModule.GetGarageDataById(garageId);
            if (garage == null) return;
            await _vehicleHandler.TakeVehicleOutOfGarage(rpPlayer, vehicleId, garage);
        }


        public void ParkInVehicle(IPlayer player, IVehicle vehicle)
        {
            GarageData garage = _garageDataModule._garages.Values.FirstOrDefault(g => g.Position.Distance(vehicle.Position) < g.Radius);
            if (garage == null) return;
            RPVehicle rpVehicle = (RPVehicle) vehicle;
            ParkInVehicle(player, rpVehicle.VehicleId, garage.Id);
        }


        void ParkInVehicle(IPlayer player, int vehicleId, int garageId)
        {
            RPPlayer rpPlayer = (RPPlayer)player;
            GarageData garage = _garageDataModule.GetGarageDataById(garageId);
            if (garage == null) return;

            RPVehicle rpVehicle = _vehicleHandler.GetRpVehicle(vehicleId);
            if (rpVehicle == null) return;

            _vehicleHandler.ParkVehicleIntoGarage(rpPlayer, vehicleId, garage);
        }


        public async Task<bool> OnPressedE(IPlayer player)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            var garage = _garageDataModule._garages.Values.FirstOrDefault(g => player.Position.Distance(g.Position) < 3);
            if (garage == null) return false;
            _logger.Info(player.Name + " opened Garage " + garage.Id);
            List<Models.Vehicle> vehiclesInGarage = await _vehicleHandler.GetVehiclesInGarage(rpPlayer, garage);

            List<GarageVehicle> garageVehicles = new List<GarageVehicle>();
            foreach (var vehicle in vehiclesInGarage)
            {
                garageVehicles.Add(new GarageVehicle(vehicle.Id, vehicle.VehicleData.Name));
            }

            HouseGarageData? houseGarageData = garage.HouseGarageData.FirstOrDefault(d => d.GarageDataId == garage.Id);

            if (houseGarageData != null)
            {
                //house garage
                bool canParkIn = !(!rpPlayer.OwnedHouses.Contains(houseGarageData.HouseDataId) && !rpPlayer.RentHouses.ContainsKey(houseGarageData.HouseDataId));
                player.Emit("ShowIF", "Garage", new GarageVehicleWriter(garageVehicles, garage.Id, garage.Name, canParkIn));
                return true;
            }

            //public garage
            player.Emit("ShowIF", "Garage", new GarageVehicleWriter(garageVehicles, garage.Id, garage.Name));
            return true;
        }

        public void OnPlayerConnect(IPlayer player, string reason)
        {
            List<BlipData> blipDataList = new List<BlipData>();
            foreach (var shop in _garageDataModule._garages.Values.Where(d => d.HasMarker))
            {
                blipDataList.Add(new BlipData(shop.Position, shop.Name));
            }
            player.Emit("SetPlayerBlips", new BlipDataWriter(blipDataList, 50, 4));
        }
    }
}

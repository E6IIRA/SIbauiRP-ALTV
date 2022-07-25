using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using GangRP_Server.Core;
using GangRP_Server.Events;
using GangRP_Server.Handlers.Logger;
using GangRP_Server.Handlers.Player;
using GangRP_Server.Handlers.Vehicle;
using GangRP_Server.Models;
using GangRP_Server.Modules.Garage;
using GangRP_Server.Modules.VehicleData;
using GangRP_Server.Utilities.VehicleOverview;
using Microsoft.EntityFrameworkCore;
using Vehicle = GangRP_Server.Models.Vehicle;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Modules.VehicleOverview
{

    public class OverviewVehicle
    {
        public int VehicleId { get; set; }

        public String VehicleHash { get; set; }
        public String GarageName { get; set; }
        public Position Position { get; set; }

        public bool InGarage { get; set; }

        public OverviewVehicle()
        {
            InGarage = true;
        }

    }




    public sealed class VehicleOverviewModule : ModuleBase, ILoadEvent
    {
        private readonly ILogger _logger;
        private readonly RPContext _rpContext;

        private readonly VehicleDataModule _vehicleDataModule;
        private readonly GarageDataModule _garageDataModule;
        private readonly IVehicleHandler _vehicleHandler;

        public static VehicleOverviewModule Instance { get; private set; }

        public VehicleOverviewModule(ILogger logger, RPContext rpContext, VehicleDataModule vehicleDataModule, GarageDataModule garageDataModule, IVehicleHandler vehicleHandler)
        {
            _logger = logger;
            _rpContext = rpContext;
            _vehicleDataModule = vehicleDataModule;
            _garageDataModule = garageDataModule;
            _vehicleHandler = vehicleHandler;
            Instance = this;
        }
        public void OnLoad()
        {
            AddClientEvent<int>("GetOverviewVehicles", GetOverviewVehicles);
        }

        public async Task<List<OverviewVehicle>> GetKeyVehicles(RPPlayer rpPlayer)
        {
            await using RPContext rpContext = new RPContext();
            IEnumerable<Vehicle> vehicles = rpContext.PlayerVehicleKey.Where(d => d.PlayerId == rpPlayer.PlayerId).Select(key => key.Vehicle);
            return DbVehiclesToOverviewVehicles(vehicles);
        }

        public async Task<List<OverviewVehicle>> GetOwnVehicles(RPPlayer rpPlayer)
        {
            await using RPContext rpContext = new RPContext();
            IEnumerable<Vehicle> vehicles = rpContext.Vehicle.Where(d => d.PlayerId == rpPlayer.PlayerId);
            return DbVehiclesToOverviewVehicles(vehicles);
        }

        public async Task<List<OverviewVehicle>> GetTeamVehicles(RPPlayer rpPlayer)
        {
            await using RPContext rpContext = new RPContext();
            IEnumerable<Vehicle> vehicles = rpContext.Vehicle.Where(d => d.TeamDataId == rpPlayer.TeamId);
            return DbVehiclesToOverviewVehicles(vehicles);
        }

        public List<OverviewVehicle> DbVehiclesToOverviewVehicles(IEnumerable<Vehicle> vehicles)
        {
            List<OverviewVehicle> overviewVehicles = new List<OverviewVehicle>();

            foreach (var vehicle in vehicles)
            {
                GarageData garageData = _garageDataModule.GetGarageDataById(vehicle.GarageDataId);
                RPVehicle? rpVehicle = _vehicleHandler.GetRpVehicle(vehicle.Id);

                OverviewVehicle overviewVehicle = new OverviewVehicle
                {
                    VehicleId = vehicle.Id,
                    VehicleHash = _vehicleDataModule.GetVehicleDataById(vehicle.VehicleDataId).Name,
                    GarageName = garageData.Name
                };

                if (rpVehicle != null)
                {
                    overviewVehicle.Position = rpVehicle.Position;
                    overviewVehicle.InGarage = false;
                }
                else overviewVehicle.Position = garageData.Position;

                overviewVehicles.Add(overviewVehicle);
            }
            return overviewVehicles;
        }


        public async void GetOverviewVehicles(IPlayer player, int type)
        {
            RPPlayer rpPlayer = (RPPlayer) player;

            List<OverviewVehicle> overviewVehicles; 

            switch (type)
            {
                case 1: 
                    overviewVehicles = await GetKeyVehicles(rpPlayer);
                    break;
                case 2: 
                    overviewVehicles = await GetTeamVehicles(rpPlayer);
                    break;
                default:
                    overviewVehicles = await GetOwnVehicles(rpPlayer);
                    break;
            }

            player.Emit("UpdateView", "SendOverviewVehicles", new VehicleOverviewWriter(overviewVehicles, rpPlayer.TeamId != 1));
        }


        public async Task ShowVehicleOverview(IPlayer player)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            player.Emit("ShowIF", "VehicleOverview", new VehicleOverviewWriter(await GetOwnVehicles(rpPlayer), rpPlayer.TeamId != 1));

        }
    }
}

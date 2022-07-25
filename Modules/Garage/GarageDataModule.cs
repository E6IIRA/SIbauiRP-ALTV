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
    public sealed class GarageDataModule : ModuleBase, ILoadEvent
    {
        private readonly ILogger _logger;
        private readonly RPContext _rpContext;


        public Dictionary<int, GarageData> _garages;

        public static GarageDataModule Instance { get; private set; }

        public int[] Pkws = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 13};

        public GarageDataModule(ILogger logger, RPContext rpContext)
        {
            _logger = logger;
            _rpContext = rpContext;
            Instance = this;
        }

        public void OnLoad()
        {
            _garages = AddTableLoadEvent<GarageData>(_rpContext.GarageData.Include(d => d.GaragespawnData).Include(d => d.HouseGarageData), OnItemLoad).ToDictionary(data => data.Id);
        }


        private void OnItemLoad(GarageData garage)
        {
#if DEBUG
            MarkerStreamer.Create(MarkerTypes.MarkerTypeUpsideDownCone, garage.Position, new Vector3(1), color: new Rgba(255, 0, 0, 255));
            TextLabelStreamer.Create($"Garage Id: {garage.Id}", garage.Position, color: new Rgba(255, 255, 0, 255));

            foreach (GaragespawnData garagespawnData in garage.GaragespawnData)
            {
                MarkerStreamer.Create((MarkerTypes)36, garagespawnData.Position, new Vector3(1), color: new Rgba(255, 0, 0, 255));
                TextLabelStreamer.Create($"GarageSpawn Id: {garagespawnData.Id}", garagespawnData.Position, color: new Rgba(255, 255, 0, 255));

                Alt.Server.CreateVehicle(3884762073, garagespawnData.Position, garagespawnData.Rotation);

            }


            garage.VehicleClassificationHashSet = new HashSet<int>();
            //DEFAULT PKW GARAGE
            if (String.IsNullOrWhiteSpace(garage.VehicleClassifications))
            {
                garage.VehicleClassificationHashSet = new HashSet<int>(Pkws);
            }
            else
            {
                try
                {
                    int[] myInts = Array.ConvertAll(garage.VehicleClassifications.Split(";"), s => int.Parse(s));
                    garage.VehicleClassificationHashSet = new HashSet<int>(myInts);
                }
                catch (Exception e)
                {
                    _logger.Error($"Error Loading GarageData with ID {garage.Id} REASON: Invalid VehicleClassifications");
                    return;
                }
            }


#endif

        }

        public GarageData GetGarageDataById(int garageDataId)
        {
            if (_garages.TryGetValue(garageDataId, out GarageData garageData))
            {
                return garageData;
            }
            return null;
        }


    }
}

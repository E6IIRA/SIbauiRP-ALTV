using System.Collections.Generic;
using System.Linq;
using GangRP_Server.Core;
using GangRP_Server.Events;
using GangRP_Server.Handlers.Logger;
using GangRP_Server.Handlers.Player;
using GangRP_Server.Models;
using Microsoft.EntityFrameworkCore;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Modules.VehicleData
{
    public sealed class VehicleDataModule : ModuleBase, ILoadEvent
    {
        private readonly ILogger _logger;
        private readonly RPContext _rpContext;


        public IEnumerable<Models.VehicleData> _vehicleData;


        public Dictionary<int, Models.VehicleData> _vehicleDataDictionary;

        public static VehicleDataModule Instance { get; private set; }
        public VehicleDataModule(ILogger logger, RPContext rpContext)
        {
            _logger = logger;
            _rpContext = rpContext;
            Instance = this;
        }

        public void OnLoad()
        {
            _vehicleDataDictionary = new Dictionary<int, Models.VehicleData>();
            _vehicleData = AddTableLoadEvent<Models.VehicleData>(_rpContext.VehicleData
                .Include(d => d.Classification), OnItemLoad);

        }

        private void OnItemLoad(Models.VehicleData vehicleData)
        {
            _vehicleDataDictionary.Add(vehicleData.Id, vehicleData);
        }

        public Models.VehicleData GetVehicleDataById(int vehicleDataId)
        {
            if (_vehicleDataDictionary.TryGetValue(vehicleDataId, out Models.VehicleData? vehicleData))
            {
                return vehicleData;
            }
            return _vehicleDataDictionary.First().Value;
        }


    }
}

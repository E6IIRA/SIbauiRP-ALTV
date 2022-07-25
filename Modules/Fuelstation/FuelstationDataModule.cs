using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
    public sealed class FuelstationDataModule : ModuleBase, ILoadEvent
    {
        private readonly RPContext _rpContext;

        public Dictionary<int, Tuple<FuelstationData, Dictionary<int, FuelstationGaspumpData>>> _fuelstationDatas;

        public FuelstationDataModule(RPContext rpContext)
        {
            _rpContext = rpContext;
        }

        public void OnLoad()
        {
            _fuelstationDatas = new Dictionary<int, Tuple<FuelstationData, Dictionary<int, FuelstationGaspumpData>>>();
            foreach (var fuelstationData in _rpContext.FuelstationData.Include(d => d.FuelstationGaspumpData).ToDictionary(data => data.Id).Values)
            {
                fuelstationData.Price = MathUtils.RandomNumber(5, 10);
                _fuelstationDatas.Add(fuelstationData.Id, new Tuple<FuelstationData, Dictionary<int, FuelstationGaspumpData>>(fuelstationData, fuelstationData.FuelstationGaspumpData.ToDictionary(data => data.Id)));
                IColShape colShape = Alt.CreateColShapeSphere(fuelstationData.InfoPosition, fuelstationData.Range);
                colShape.SetData("fuelstationId", fuelstationData.Id);
#if DEBUG
                TextLabelStreamer.Create($"Fuelstation Id: {fuelstationData.Id}", fuelstationData.Position, color: new Rgba(255, 255, 0, 255));
                MarkerStreamer.Create(MarkerTypes.MarkerTypeVerticalCylinder, fuelstationData.InfoPosition, new Vector3(fuelstationData.Range * 2), color: new Rgba(255, 255, 0, 255));

                foreach (var fuelstationGaspumpData in fuelstationData.FuelstationGaspumpData.ToList())
                {
                    TextLabelStreamer.Create($"GasPump Id: {fuelstationGaspumpData.Id}", fuelstationGaspumpData.Position, color: new Rgba(255, 255, 0, 255));
                }
#endif
            }
        }

        public FuelstationData GetFuelstationDataById(int fuelStationId)
        {
            if (_fuelstationDatas.TryGetValue(fuelStationId, out var fuelstationData))
            {
                return fuelstationData.Item1;
            }
            return null;
        }

    }
}

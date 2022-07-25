using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using AltV.Net;
using GangRP_Server.Core;
using GangRP_Server.Models;
using GangRP_Server.Modules;
using GangRP_Server.Modules.Player;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Utilities.FuelStation
{
    public class FuelStationWriter : IWritable
    {
        private readonly int _fuelStationId;
        private readonly string _fuelStationName;
        private readonly int _gasPumpId;
        private readonly int _vehicleId;
        private readonly int _maxFuel;
        private readonly int _price;

        public FuelStationWriter(int fuelStationId, string fuelStationName, int gasPumpId, int vehicleId, int maxFuel, int price)
        {
            this._fuelStationId = fuelStationId;
            this._fuelStationName = fuelStationName;
            this._gasPumpId = gasPumpId;
            this._vehicleId = vehicleId;
            this._maxFuel = maxFuel;
            this._price = price;
        }


        public void OnWrite(IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("i");
            writer.Value(_fuelStationId);
            writer.Name("n");
            writer.Value(_fuelStationName);
            writer.Name("g");
            writer.Value(_gasPumpId);
            writer.Name("v");
            writer.Value(_vehicleId);
            writer.Name("f");
            writer.Value(_maxFuel);
            writer.Name("p");
            writer.Value(_price);
            writer.EndObject();
        }
    }
}

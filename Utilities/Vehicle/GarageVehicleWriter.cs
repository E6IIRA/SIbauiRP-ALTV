using System.Collections.Generic;
using AltV.Net;
using GangRP_Server.Utilities.ClothProp;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Utilities.Vehicle
{
    public class GarageVehicleWriter : IWritable
    {
        private readonly List<GarageVehicle> _garageVehicles;
        private readonly int _garageId;
        private readonly string _garageName;
        private readonly bool _canParkIn;

        public GarageVehicleWriter(List<GarageVehicle> garageVehicles, int garageId, string garageName, bool canParkIn = true)
        {
            this._garageVehicles = garageVehicles;
            this._garageId = garageId;
            this._garageName = garageName;
            this._canParkIn = canParkIn;

        }

        public void OnWrite(IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("i");
            writer.Value(_garageId);
            writer.Name("n");
            writer.Value(_garageName);
            writer.Name("c");
            writer.Value(_canParkIn);
            writer.Name("data");
            writer.BeginArray();
                foreach (var value in _garageVehicles)
                {
                    writer.BeginObject();
                        writer.Name("i");
                        writer.Value(value.VehicleId);
                        writer.Name("n");
                        writer.Value(value.Name);
                    writer.EndObject();
                }
                writer.EndArray();
            writer.EndObject();
        }
    }
}

using System.Collections.Generic;
using AltV.Net;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Utilities.House
{
    public class HouseVehicleWriter : IWritable
    {
        private List<Models.Vehicle> _vehicles;

        public HouseVehicleWriter(List<Models.Vehicle> vehicles)
        {
            this._vehicles = vehicles;
        }


        public void OnWrite(IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("vehicles");
            writer.BeginArray();
            foreach (var value in _vehicles)
            {
                writer.BeginObject();
                writer.Name("i");
                writer.Value(value.Id);
                writer.Name("n");
                writer.Value(value.VehicleData.Name);
                writer.Name("p");
                writer.Value(value.Player.Name);
                writer.EndObject();
            }
            writer.EndArray();
            writer.EndObject();
        }
    }
}

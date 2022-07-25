using System.Collections.Generic;
using AltV.Net;
using GangRP_Server.Utilities.ClothProp;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Utilities.VehicleShop
{
    public class VehicleShopVehicleDataWriter : IWritable
    {
        private readonly List<VehicleShopVehicleData> _shopVehicles;
        private readonly int _vehicleShopId;
        private readonly string _vehicleShopName;

        public VehicleShopVehicleDataWriter(List<VehicleShopVehicleData> garageVehicles, int vehicleShopId, string vehicleShopName)
        {
            this._shopVehicles = garageVehicles;
            this._vehicleShopId = vehicleShopId;
            this._vehicleShopName = vehicleShopName;


        }

        public void OnWrite(IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("i");
            writer.Value(_vehicleShopId);
            writer.Name("n");
            writer.Value(_vehicleShopName);
            writer.Name("data");
            writer.BeginArray();
                foreach (var value in _shopVehicles)
                {
                    writer.BeginObject();
                        writer.Name("i");
                        writer.Value(value.VehicleId);
                        writer.Name("n");
                        writer.Value(value.Name);
                        writer.Name("p");
                        writer.Value(value.Price);
                writer.EndObject();
                }
                writer.EndArray();
            writer.EndObject();
        }
    }
}

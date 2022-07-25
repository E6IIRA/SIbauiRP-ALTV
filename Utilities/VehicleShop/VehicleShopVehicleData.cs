using System.Numerics;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Utilities.VehicleShop
{
    public class VehicleShopVehicleData
    {
        public int VehicleId;
        public string Name;
        public int Price;

        public VehicleShopVehicleData(int vehicleId, string name, int price)
        {
            this.VehicleId = vehicleId;
            this.Name = name;
            this.Price = price;
        }
    }
}

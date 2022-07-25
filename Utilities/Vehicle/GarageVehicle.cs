using System.Numerics;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Utilities.Vehicle
{
    public class GarageVehicle
    {
        public int VehicleId;
        public string Name;

        public GarageVehicle(int vehicleId, string name)
        {
            this.VehicleId = vehicleId;
            this.Name = name;
        }
    }
}

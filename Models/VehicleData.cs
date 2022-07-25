using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class VehicleData
    {
        public VehicleData()
        {
            Vehicle = new HashSet<Vehicle>();
            VehicleShopVehicle = new HashSet<VehicleShopVehicle>();
        }

        public int Id { get; set; }
        public uint Hash { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public int Multiplier { get; set; }
        public int MaxFuel { get; set; }
        public float FuelConsumption { get; set; }
        public int ClassificationId { get; set; }
        public int Tax { get; set; }
        public int InventoryTypeDataId { get; set; }
        public byte Seats { get; set; }

        public virtual VehicleClassificationData Classification { get; set; }
        public virtual InventoryTypeData InventoryTypeData { get; set; }
        public virtual ICollection<Vehicle> Vehicle { get; set; }
        public virtual ICollection<VehicleShopVehicle> VehicleShopVehicle { get; set; }
    }
}

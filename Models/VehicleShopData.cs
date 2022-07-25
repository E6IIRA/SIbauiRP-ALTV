using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class VehicleShopData
    {
        public VehicleShopData()
        {
            VehicleShopVehicle = new HashSet<VehicleShopVehicle>();
        }

        public int Id { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }
        public float SpositionX { get; set; }
        public float SpositionY { get; set; }
        public float SpositionZ { get; set; }
        public bool Activated { get; set; }
        public bool HasMarker { get; set; }
        public string Description { get; set; }
        public string PedHash { get; set; }

        public virtual ICollection<VehicleShopVehicle> VehicleShopVehicle { get; set; }
    }
}

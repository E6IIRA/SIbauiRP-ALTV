using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class VehicleShopVehicle
    {
        public int Id { get; set; }
        public int VehicleShopDataId { get; set; }
        public int VehicleDataId { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }
        public float RotationX { get; set; }
        public float RotationY { get; set; }
        public float RotationZ { get; set; }
        public byte ColorPrimary { get; set; }
        public byte ColorSecondary { get; set; }

        public virtual VehicleData VehicleData { get; set; }
        public virtual VehicleShopData VehicleShopData { get; set; }
    }
}

using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class GarageData
    {
        public GarageData()
        {
            GaragespawnData = new HashSet<GaragespawnData>();
            HouseGarageData = new HashSet<HouseGarageData>();
            Vehicle = new HashSet<Vehicle>();
        }

        public int Id { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }
        public float Rotation { get; set; }
        public int Type { get; set; }
        public bool HasMarker { get; set; }
        public string Name { get; set; }
        public string PedHash { get; set; }
        public int Radius { get; set; }
        public string VehicleClassifications { get; set; }

        public virtual ICollection<GaragespawnData> GaragespawnData { get; set; }
        public virtual ICollection<HouseGarageData> HouseGarageData { get; set; }
        public virtual ICollection<Vehicle> Vehicle { get; set; }
    }
}

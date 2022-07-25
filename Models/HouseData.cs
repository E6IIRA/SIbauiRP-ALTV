using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class HouseData
    {
        public HouseData()
        {
            House = new HashSet<House>();
            HouseGarageData = new HashSet<HouseGarageData>();
        }

        public int Id { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }
        public float RotationRoll { get; set; }
        public float RotationPitch { get; set; }
        public float RotationYaw { get; set; }
        public sbyte RentalPlaces { get; set; }
        public int Price { get; set; }
        public int InteriorDataId { get; set; }
        public int HouseAreaDataId { get; set; }
        public int HouseAppearanceDataId { get; set; }
        public int HouseSizeDataId { get; set; }

        public virtual HouseAppearanceData HouseAppearanceData { get; set; }
        public virtual HouseAreaData HouseAreaData { get; set; }
        public virtual HouseSizeData HouseSizeData { get; set; }
        public virtual InteriorData InteriorData { get; set; }
        public virtual ICollection<House> House { get; set; }
        public virtual ICollection<HouseGarageData> HouseGarageData { get; set; }
    }
}

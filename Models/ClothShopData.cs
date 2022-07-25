using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class ClothShopData
    {
        public ClothShopData()
        {
            ClothData = new HashSet<ClothData>();
        }

        public int Id { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }
        public float WarderobeX { get; set; }
        public float WarderobeY { get; set; }
        public float WarderobeZ { get; set; }
        public string Name { get; set; }
        public int IsActive { get; set; }

        public virtual ICollection<ClothData> ClothData { get; set; }
    }
}

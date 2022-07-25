using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class InteriorData
    {
        public InteriorData()
        {
            HouseData = new HashSet<HouseData>();
            InteriorPositionData = new HashSet<InteriorPositionData>();
            InteriorWarderobeData = new HashSet<InteriorWarderobeData>();
            Storageroom = new HashSet<Storageroom>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }

        public virtual ICollection<HouseData> HouseData { get; set; }
        public virtual ICollection<InteriorPositionData> InteriorPositionData { get; set; }
        public virtual ICollection<InteriorWarderobeData> InteriorWarderobeData { get; set; }
        public virtual ICollection<Storageroom> Storageroom { get; set; }
    }
}

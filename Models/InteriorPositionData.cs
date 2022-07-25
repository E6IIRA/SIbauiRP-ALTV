using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class InteriorPositionData
    {
        public InteriorPositionData()
        {
            HouseInteriorPosition = new HashSet<HouseInteriorPosition>();
            StorageroomInteriorPosition = new HashSet<StorageroomInteriorPosition>();
        }

        public int Id { get; set; }
        public int InteriorDataId { get; set; }
        public int InteriorPositionDataTypeId { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }

        public virtual InteriorData InteriorData { get; set; }
        public virtual InteriorPositionTypeData InteriorPositionDataType { get; set; }
        public virtual ICollection<HouseInteriorPosition> HouseInteriorPosition { get; set; }
        public virtual ICollection<StorageroomInteriorPosition> StorageroomInteriorPosition { get; set; }
    }
}

using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class FarmFieldObjectData
    {
        public int Id { get; set; }
        public int FarmFieldDataId { get; set; }
        public int FarmObjectDataId { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }
        public float RotationRoll { get; set; }
        public float RotationPitch { get; set; }
        public float RotationYaw { get; set; }

        public virtual FarmFieldData FarmFieldData { get; set; }
        public virtual FarmObjectData FarmObjectData { get; set; }
    }
}

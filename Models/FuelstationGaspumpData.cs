using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class FuelstationGaspumpData
    {
        public int Id { get; set; }
        public int FuelstationDataId { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }

        public virtual FuelstationData FuelstationData { get; set; }
    }
}

using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class FuelstationData
    {
        public FuelstationData()
        {
            FuelstationGaspumpData = new HashSet<FuelstationGaspumpData>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }
        public float InfoPositionX { get; set; }
        public float InfoPositionY { get; set; }
        public int InfoPositionZ { get; set; }
        public int Range { get; set; }

        public virtual ICollection<FuelstationGaspumpData> FuelstationGaspumpData { get; set; }
    }
}

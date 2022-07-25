using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class VehicleTuningData
    {
        public VehicleTuningData()
        {
            VehicleTuning = new HashSet<VehicleTuning>();
        }

        public byte Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<VehicleTuning> VehicleTuning { get; set; }
    }
}

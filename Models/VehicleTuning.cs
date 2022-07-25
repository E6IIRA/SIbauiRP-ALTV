using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class VehicleTuning
    {
        public byte Id { get; set; }
        public int VehicleId { get; set; }
        public byte VehicleTuningDataId { get; set; }
        public byte Value { get; set; }

        public virtual Vehicle Vehicle { get; set; }
        public virtual VehicleTuningData VehicleTuningData { get; set; }
    }
}

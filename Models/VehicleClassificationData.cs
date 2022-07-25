using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class VehicleClassificationData
    {
        public VehicleClassificationData()
        {
            VehicleData = new HashSet<VehicleData>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<VehicleData> VehicleData { get; set; }
    }
}

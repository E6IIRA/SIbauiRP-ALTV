using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class DrugCamper
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public int DrugCamperTypeDataId { get; set; }
        public int TeamDataId { get; set; }
        public float Temperature { get; set; }
        public float Humidity { get; set; }
        public int Ventilation { get; set; }
        public int Dildogroeße { get; set; }
        public int SecurityUpgrade { get; set; }

        public virtual DrugCamperTypeData DrugCamperTypeData { get; set; }
        public virtual TeamData TeamData { get; set; }
        public virtual Vehicle Vehicle { get; set; }
    }
}

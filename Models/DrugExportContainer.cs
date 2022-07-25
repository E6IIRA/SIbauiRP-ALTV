using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class DrugExportContainer
    {
        public int Id { get; set; }
        public int DrugExportContainerDataId { get; set; }
        public int TeamId { get; set; }
        public DateTime CallTime { get; set; }
        public DateTime SendTime { get; set; }
        public DateTime EndTime { get; set; }
        public int DrugItemId { get; set; }
        public int DrugAmount { get; set; }
        public int Price { get; set; }
        public int Money { get; set; }

        public virtual DrugExportContainerData DrugExportContainerData { get; set; }
        public virtual ItemData DrugItem { get; set; }
        public virtual TeamData Team { get; set; }
    }
}

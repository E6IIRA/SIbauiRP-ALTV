using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class DrugExportContainerData
    {
        public DrugExportContainerData()
        {
            DrugExportContainer = new HashSet<DrugExportContainer>();
        }

        public int Id { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }

        public virtual ICollection<DrugExportContainer> DrugExportContainer { get; set; }
    }
}

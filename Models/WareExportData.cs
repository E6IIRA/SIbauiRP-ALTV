using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class WareExportData
    {
        public WareExportData()
        {
            WareExportDataHistory = new HashSet<WareExportDataHistory>();
        }

        public int Id { get; set; }
        public int ItemId { get; set; }
        public int MinimumPrice { get; set; }
        public int ActualPrice { get; set; }
        public int MaximumPrice { get; set; }
        public int Setpoint { get; set; }

        public virtual ItemData Item { get; set; }
        public virtual ICollection<WareExportDataHistory> WareExportDataHistory { get; set; }
    }
}

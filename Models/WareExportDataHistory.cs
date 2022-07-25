using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class WareExportDataHistory
    {
        public int Id { get; set; }
        public int WareExportDataId { get; set; }
        public int ActualPrice { get; set; }
        public DateTime Date { get; set; }

        public virtual WareExportData WareExportData { get; set; }
    }
}

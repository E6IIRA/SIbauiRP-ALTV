using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class TeamData
    {
        public TeamData()
        {
            DrugCamper = new HashSet<DrugCamper>();
            DrugExportContainer = new HashSet<DrugExportContainer>();
            Player = new HashSet<Player>();
            TeamKeyStorageData = new HashSet<TeamKeyStorageData>();
            Vehicle = new HashSet<Vehicle>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }

        public virtual TeamTypeData TypeNavigation { get; set; }
        public virtual ICollection<DrugCamper> DrugCamper { get; set; }
        public virtual ICollection<DrugExportContainer> DrugExportContainer { get; set; }
        public virtual ICollection<Player> Player { get; set; }
        public virtual ICollection<TeamKeyStorageData> TeamKeyStorageData { get; set; }
        public virtual ICollection<Vehicle> Vehicle { get; set; }
    }
}

using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class TeamKeyStorageData
    {
        public TeamKeyStorageData()
        {
            TeamKeyStorage = new HashSet<TeamKeyStorage>();
        }

        public int Id { get; set; }
        public int TeamId { get; set; }
        public string Name { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }

        public virtual TeamData Team { get; set; }
        public virtual ICollection<TeamKeyStorage> TeamKeyStorage { get; set; }
    }
}

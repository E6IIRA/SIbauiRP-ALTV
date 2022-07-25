using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class ServerScenarioData
    {
        public ServerScenarioData()
        {
            ServerScenarioLootData = new HashSet<ServerScenarioLootData>();
            ServerScenarioPropData = new HashSet<ServerScenarioPropData>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }

        public virtual ICollection<ServerScenarioLootData> ServerScenarioLootData { get; set; }
        public virtual ICollection<ServerScenarioPropData> ServerScenarioPropData { get; set; }
    }
}

using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class ServerScenarioLootData
    {
        public int Id { get; set; }
        public int ServerScenarioDataId { get; set; }
        public int ItemDataId { get; set; }
        public int MinimumAmount { get; set; }
        public int MaximumAmount { get; set; }
        public float Radius { get; set; }
        public string Propname { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }
        public float RotationX { get; set; }
        public float RotationY { get; set; }
        public float RotationZ { get; set; }

        public virtual ItemData ItemData { get; set; }
        public virtual ServerScenarioData ServerScenarioData { get; set; }
    }
}

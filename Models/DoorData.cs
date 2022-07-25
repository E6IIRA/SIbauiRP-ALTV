using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class DoorData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ModelHash { get; set; }
        public bool Locked { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }
        public int Range { get; set; }
        public string Teams { get; set; }
    }
}

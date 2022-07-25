using System;
using System.Collections.Generic;
using System.Text;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class DoorData
    { 
        public Position Position { get => new Position(PositionX, PositionY, PositionZ);}

        public DateTime LastBreak = DateTime.Now.Add(new TimeSpan(0, -5, 0)); // set lastbreak for load now -5 min
    }
}

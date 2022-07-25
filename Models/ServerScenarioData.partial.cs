using AltV.Net.Data;
using System;
using System.Collections.Generic;
using System.Text;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class ServerScenarioData
    {
        public Position Position => new Position(PositionX, PositionY, PositionZ);
        public DateTime SpawnTime;
        public bool IsActive = false;
    }
}

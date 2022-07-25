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
    public partial class StorageroomData
    {
        public Position Position { get => new Position(PositionX, PositionY, PositionZ); }
    }
}

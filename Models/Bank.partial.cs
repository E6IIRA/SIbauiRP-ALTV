using System;
using System.Collections.Generic;
using System.Text;
using AltV.Net.Data;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class Bank
    {
        public Position Position { get => new Position(PositionX, PositionY, PositionZ); }
    }
}

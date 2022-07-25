using AltV.Net.Data;
using System;
using System.Collections.Generic;
using System.Text;
using GangRP_Server.Utilities;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class ServerScenarioPropData
    {
        public Position Position => new Position(PositionX, PositionY, PositionZ);
        public Rotation Rotation => new Rotation(RotationX, RotationY, RotationZ);
        public Prop Prop;
        public bool IsActive = false;
    }
}

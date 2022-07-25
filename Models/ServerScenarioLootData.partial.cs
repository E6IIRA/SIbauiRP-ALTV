using System;
using System.Collections.Generic;
using System.Text;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using GangRP_Server.Utilities;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class ServerScenarioLootData
    {
        public Position Position => new Position(PositionX, PositionY, PositionZ);
        public Rotation Rotation => new Rotation(RotationX, RotationY, RotationZ);
        public Prop Prop;
        public IColShape ColShape;
        public bool IsActive = false;
    }
}

using AltV.Net.Data;
using GangRP_Server.Utilities;
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
    public partial class FarmFieldObjectData
    {
        public Position Position { get => new Position(PositionX, PositionY, PositionZ); }
        public Rotation Rotation { get => new Rotation(RotationRoll, RotationPitch, RotationYaw); }
        public PlayerLabel PlayerLabel;
        public int Capacity; //{ get => FarmObjectData.Capacity;}
        public Prop? Prop;
        public DateTime LastFarmed;
        public bool Active = false;
    }
}

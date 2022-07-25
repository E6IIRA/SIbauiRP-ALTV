using AltV.Net.Data;
using GangRP_Server.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using MathF = System.MathF;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class Plant
    {
        public Prop Prop;
        public Position Position;
        public Rotation Rotation;
        public int PerformanceSlot;
        public PlayerLabel PlayerLabel;

        public float Distance2D(Position position)
        {
            float difX = Position.X - position.X;
            float difY = Position.Y - position.Y;
            return MathF.Sqrt(difX * difX + difY * difY);
        }
    }
}

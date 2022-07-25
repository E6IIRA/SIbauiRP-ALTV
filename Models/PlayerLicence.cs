using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class PlayerLicence
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int IdCard { get; set; }
        public int Car { get; set; }
        public int Truck { get; set; }
        public int Motorcycle { get; set; }
        public int Boat { get; set; }
        public int Helicopter { get; set; }
        public int Plane { get; set; }

        public virtual Player Player { get; set; }
    }
}

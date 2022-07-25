﻿using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class PlayerVehicleKey
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int VehicleId { get; set; }
        public DateTime CreationDate { get; set; }

        public virtual Player Player { get; set; }
        public virtual Vehicle Vehicle { get; set; }
    }
}

using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class TeamKeyStorage
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public int TeamKeyStorageDataId { get; set; }
        public int VehicleKeyGeneration { get; set; }
        public DateTime CreationDate { get; set; }

        public virtual TeamKeyStorageData TeamKeyStorageData { get; set; }
        public virtual Vehicle Vehicle { get; set; }
    }
}

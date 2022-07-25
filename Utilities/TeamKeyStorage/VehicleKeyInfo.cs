using System;
using System.Collections.Generic;
using System.Text;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Utilities.TeamKeyStorage
{
    public class VehicleKeyInfo
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public String OwnerName { get; set; }
        public String VehicleDataName { get; set; }
        public DateTime CreationDate { get; set; }


        public VehicleKeyInfo(int id, int vehicleId, String ownerName, String vehicleDataName, DateTime creationDate)
        {
            this.Id = id;
            this.VehicleId = vehicleId;
            this.OwnerName = ownerName;
            this.VehicleDataName = vehicleDataName;
            this.CreationDate = creationDate;
        }


    }
}

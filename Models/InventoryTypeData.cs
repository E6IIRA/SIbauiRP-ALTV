using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class InventoryTypeData
    {
        public InventoryTypeData()
        {
            Inventory = new HashSet<Inventory>();
            VehicleData = new HashSet<VehicleData>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int MaxSlots { get; set; }
        public int MaxWeight { get; set; }
        public int TypeId { get; set; }

        public virtual ICollection<Inventory> Inventory { get; set; }
        public virtual ICollection<VehicleData> VehicleData { get; set; }
    }
}

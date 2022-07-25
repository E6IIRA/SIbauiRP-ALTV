using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class PlayerInventories
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int InventoryId { get; set; }

        public virtual Inventory Inventory { get; set; }
        public virtual Player Player { get; set; }
    }
}

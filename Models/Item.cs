using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class Item
    {
        public int Id { get; set; }
        public int InventoryId { get; set; }
        public int Slot { get; set; }
        public int ItemDataId { get; set; }
        public int Amount { get; set; }
        public string CustomItemData { get; set; }

        public virtual Inventory Inventory { get; set; }
        public virtual ItemData ItemData { get; set; }
    }
}

using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class Inventory
    {
        public Inventory()
        {
            HouseInteriorPosition = new HashSet<HouseInteriorPosition>();
            Item = new HashSet<Item>();
            Player = new HashSet<Player>();
            PlayerInventories = new HashSet<PlayerInventories>();
            StorageroomInteriorPosition = new HashSet<StorageroomInteriorPosition>();
            Vehicle = new HashSet<Vehicle>();
        }

        public int Id { get; set; }
        public int InventoryTypeDataId { get; set; }

        public virtual InventoryTypeData InventoryTypeData { get; set; }
        public virtual ICollection<HouseInteriorPosition> HouseInteriorPosition { get; set; }
        public virtual ICollection<Item> Item { get; set; }
        public virtual ICollection<Player> Player { get; set; }
        public virtual ICollection<PlayerInventories> PlayerInventories { get; set; }
        public virtual ICollection<StorageroomInteriorPosition> StorageroomInteriorPosition { get; set; }
        public virtual ICollection<Vehicle> Vehicle { get; set; }
    }
}

using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class Storageroom
    {
        public Storageroom()
        {
            PlayerStorageroomOwned = new HashSet<PlayerStorageroomOwned>();
            StorageroomInteriorPosition = new HashSet<StorageroomInteriorPosition>();
        }

        public int Id { get; set; }
        public int StorageroomDataId { get; set; }
        public int Type { get; set; }
        public int InteriorDataId { get; set; }
        public int Crates { get; set; }
        public int InventoryAmount { get; set; }

        public virtual InteriorData InteriorData { get; set; }
        public virtual StorageroomData StorageroomData { get; set; }
        public virtual ICollection<PlayerStorageroomOwned> PlayerStorageroomOwned { get; set; }
        public virtual ICollection<StorageroomInteriorPosition> StorageroomInteriorPosition { get; set; }
    }
}

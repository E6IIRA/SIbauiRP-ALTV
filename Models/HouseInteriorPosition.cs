using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class HouseInteriorPosition
    {
        public int Id { get; set; }
        public int HouseId { get; set; }
        public int InteriorPositionDataId { get; set; }
        public int InventoryId { get; set; }

        public virtual House House { get; set; }
        public virtual InteriorPositionData InteriorPositionData { get; set; }
        public virtual Inventory Inventory { get; set; }
    }
}

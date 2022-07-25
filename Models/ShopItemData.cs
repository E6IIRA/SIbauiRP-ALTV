using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class ShopItemData
    {
        public int Id { get; set; }
        public int ShopDataId { get; set; }
        public int ItemDataId { get; set; }
        public int Price { get; set; }

        public virtual ItemData ItemData { get; set; }
        public virtual ShopData ShopData { get; set; }
    }
}

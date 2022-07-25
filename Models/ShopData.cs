using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class ShopData
    {
        public ShopData()
        {
            ShopItemData = new HashSet<ShopItemData>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }
        public float Rotation { get; set; }
        public string PedHash { get; set; }
        public int Marker { get; set; }
        public float RobPositionX { get; set; }
        public float RobPositionY { get; set; }
        public float RobPositionZ { get; set; }

        public virtual ICollection<ShopItemData> ShopItemData { get; set; }
    }
}

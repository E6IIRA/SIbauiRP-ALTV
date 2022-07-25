using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class House
    {
        public House()
        {
            HouseInteriorPosition = new HashSet<HouseInteriorPosition>();
            PlayerHouseOwned = new HashSet<PlayerHouseOwned>();
            PlayerHouseRent = new HashSet<PlayerHouseRent>();
        }

        public int Id { get; set; }
        public int HouseDataId { get; set; }
        public string DoorbellSign { get; set; }
        public int Money { get; set; }

        public virtual HouseData HouseData { get; set; }
        public virtual ICollection<HouseInteriorPosition> HouseInteriorPosition { get; set; }
        public virtual ICollection<PlayerHouseOwned> PlayerHouseOwned { get; set; }
        public virtual ICollection<PlayerHouseRent> PlayerHouseRent { get; set; }
    }
}

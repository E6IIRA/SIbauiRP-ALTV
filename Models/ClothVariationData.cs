using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class ClothVariationData
    {
        public ClothVariationData()
        {
            PlayerClothEquipped = new HashSet<PlayerClothEquipped>();
            PlayerClothOwned = new HashSet<PlayerClothOwned>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int ClothDataId { get; set; }
        public short Value { get; set; }

        public virtual ClothData ClothData { get; set; }
        public virtual ICollection<PlayerClothEquipped> PlayerClothEquipped { get; set; }
        public virtual ICollection<PlayerClothOwned> PlayerClothOwned { get; set; }
    }
}

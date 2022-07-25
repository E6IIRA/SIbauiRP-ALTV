using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class ClothData
    {
        public ClothData()
        {
            ClothVariationData = new HashSet<ClothVariationData>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public byte Gender { get; set; }
        public int ClothTypeDataId { get; set; }
        public short Value { get; set; }
        public int ClothShopDataId { get; set; }
        public int Price { get; set; }

        public virtual ClothShopData ClothShopData { get; set; }
        public virtual ClothTypeData ClothTypeData { get; set; }
        public virtual ICollection<ClothVariationData> ClothVariationData { get; set; }
    }
}

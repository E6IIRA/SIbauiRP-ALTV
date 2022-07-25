using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class ClothTypeData
    {
        public ClothTypeData()
        {
            ClothData = new HashSet<ClothData>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public byte Value { get; set; }

        public virtual ICollection<ClothData> ClothData { get; set; }
    }
}

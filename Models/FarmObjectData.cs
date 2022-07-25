using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class FarmObjectData
    {
        public FarmObjectData()
        {
            FarmFieldObjectData = new HashSet<FarmFieldObjectData>();
            FarmObjectLootData = new HashSet<FarmObjectLootData>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string ObjectName { get; set; }
        public int Capacity { get; set; }

        public virtual ICollection<FarmFieldObjectData> FarmFieldObjectData { get; set; }
        public virtual ICollection<FarmObjectLootData> FarmObjectLootData { get; set; }
    }
}

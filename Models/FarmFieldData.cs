using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class FarmFieldData
    {
        public FarmFieldData()
        {
            FarmFieldObjectData = new HashSet<FarmFieldObjectData>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int MinimumObjects { get; set; }
        public int MaximumObjects { get; set; }

        public virtual ICollection<FarmFieldObjectData> FarmFieldObjectData { get; set; }
    }
}

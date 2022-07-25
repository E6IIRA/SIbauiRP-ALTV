using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class FarmObjectLootData
    {
        public int Id { get; set; }
        public int FarmObjectDataId { get; set; }
        public int ItemDataId { get; set; }
        public int MinimumAmount { get; set; }
        public int MaximumAmount { get; set; }
        public float Chance { get; set; }

        public virtual FarmObjectData FarmObjectData { get; set; }
        public virtual ItemData ItemData { get; set; }
    }
}

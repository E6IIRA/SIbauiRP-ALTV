using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class DrugCamperTypeItemData
    {
        public int Id { get; set; }
        public int DrugCamperTypeDataId { get; set; }
        public int IsInput { get; set; }
        public int ItemDataId { get; set; }
        public int Amount { get; set; }

        public virtual DrugCamperTypeData DrugCamperTypeData { get; set; }
        public virtual ItemData ItemData { get; set; }
    }
}

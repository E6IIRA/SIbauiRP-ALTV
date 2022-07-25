using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class DrugCamperTypeData
    {
        public DrugCamperTypeData()
        {
            DrugCamper = new HashSet<DrugCamper>();
            DrugCamperTypeItemData = new HashSet<DrugCamperTypeItemData>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<DrugCamper> DrugCamper { get; set; }
        public virtual ICollection<DrugCamperTypeItemData> DrugCamperTypeItemData { get; set; }
    }
}

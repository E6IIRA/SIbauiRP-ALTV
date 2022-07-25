using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class PlantTypeLootData
    {
        public int Id { get; set; }
        public int PlantTypeDataId { get; set; }
        public int ItemDataId { get; set; }
        public int BaseAmount { get; set; }

        public virtual ItemData ItemData { get; set; }
        public virtual PlantTypeData PlantTypeData { get; set; }
    }
}

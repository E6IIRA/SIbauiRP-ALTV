using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class HouseGarageData
    {
        public int Id { get; set; }
        public int HouseDataId { get; set; }
        public int GarageDataId { get; set; }

        public virtual GarageData GarageData { get; set; }
        public virtual HouseData HouseData { get; set; }
    }
}

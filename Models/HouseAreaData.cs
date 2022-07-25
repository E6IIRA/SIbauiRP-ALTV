using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class HouseAreaData
    {
        public HouseAreaData()
        {
            HouseData = new HashSet<HouseData>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int Points { get; set; }

        public virtual ICollection<HouseData> HouseData { get; set; }
    }
}

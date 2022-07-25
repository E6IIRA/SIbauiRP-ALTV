using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class InteriorPositionTypeData
    {
        public InteriorPositionTypeData()
        {
            InteriorPositionData = new HashSet<InteriorPositionData>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<InteriorPositionData> InteriorPositionData { get; set; }
    }
}

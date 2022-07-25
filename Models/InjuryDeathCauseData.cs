using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class InjuryDeathCauseData
    {
        public InjuryDeathCauseData()
        {
            InjuryTypeData = new HashSet<InjuryTypeData>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public uint Hash { get; set; }

        public virtual ICollection<InjuryTypeData> InjuryTypeData { get; set; }
    }
}

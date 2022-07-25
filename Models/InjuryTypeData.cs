using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class InjuryTypeData
    {
        public InjuryTypeData()
        {
            Player = new HashSet<Player>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int InjuryDeathCauseDataId { get; set; }
        public int Time { get; set; }
        public int AdditionalTime { get; set; }
        public int TreatmentType { get; set; }
        public int Percentage { get; set; }

        public virtual InjuryDeathCauseData InjuryDeathCauseData { get; set; }
        public virtual ICollection<Player> Player { get; set; }
    }
}

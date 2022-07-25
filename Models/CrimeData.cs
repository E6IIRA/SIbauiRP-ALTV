using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class CrimeData
    {
        public CrimeData()
        {
            PlayerCrime = new HashSet<PlayerCrime>();
        }

        public int Id { get; set; }
        public int CrimeCategoryDataId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Jailtime { get; set; }
        public int Cost { get; set; }
        public int TakeGunLic { get; set; }
        public int TakeDriverLic { get; set; }

        public virtual CrimeCategoryData CrimeCategoryData { get; set; }
        public virtual ICollection<PlayerCrime> PlayerCrime { get; set; }
    }
}

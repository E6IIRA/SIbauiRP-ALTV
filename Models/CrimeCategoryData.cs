using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class CrimeCategoryData
    {
        public CrimeCategoryData()
        {
            CrimeData = new HashSet<CrimeData>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }

        public virtual ICollection<CrimeData> CrimeData { get; set; }
    }
}

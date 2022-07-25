using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class WeaponTypeData
    {
        public WeaponTypeData()
        {
            WeaponData = new HashSet<WeaponData>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<WeaponData> WeaponData { get; set; }
    }
}

using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class WeaponTintData
    {
        public WeaponTintData()
        {
            PlayerWeapon = new HashSet<PlayerWeapon>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public byte Value { get; set; }
        public byte IsMkIi { get; set; }

        public virtual ICollection<PlayerWeapon> PlayerWeapon { get; set; }
    }
}

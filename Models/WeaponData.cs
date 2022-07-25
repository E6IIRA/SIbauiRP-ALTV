using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class WeaponData
    {
        public WeaponData()
        {
            PlayerWeapon = new HashSet<PlayerWeapon>();
            WeaponComponentData = new HashSet<WeaponComponentData>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public uint Hash { get; set; }
        public int WeaponTypeDataId { get; set; }
        public int WeaponAmmunitionDataId { get; set; }
        public byte IsMkIi { get; set; }

        public virtual WeaponAmmunitionData WeaponAmmunitionData { get; set; }
        public virtual WeaponTypeData WeaponTypeData { get; set; }
        public virtual ICollection<PlayerWeapon> PlayerWeapon { get; set; }
        public virtual ICollection<WeaponComponentData> WeaponComponentData { get; set; }
    }
}

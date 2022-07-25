using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class PlayerWeapon
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int WeaponDataId { get; set; }
        public int Ammo { get; set; }
        public int WeaponTintDataId { get; set; }

        public virtual Player Player { get; set; }
        public virtual WeaponData WeaponData { get; set; }
        public virtual WeaponTintData WeaponTintData { get; set; }
    }
}

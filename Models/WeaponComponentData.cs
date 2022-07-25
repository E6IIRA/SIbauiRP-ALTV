using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class WeaponComponentData
    {
        public WeaponComponentData()
        {
            PlayerWeaponComponent = new HashSet<PlayerWeaponComponent>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int WeaponDataId { get; set; }
        public uint Value { get; set; }

        public virtual WeaponData WeaponData { get; set; }
        public virtual ICollection<PlayerWeaponComponent> PlayerWeaponComponent { get; set; }
    }
}

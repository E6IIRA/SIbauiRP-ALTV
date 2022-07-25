using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class PlayerWeaponComponent
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int WeaponComponentDataId { get; set; }

        public virtual Player Player { get; set; }
        public virtual WeaponComponentData WeaponComponentData { get; set; }
    }
}

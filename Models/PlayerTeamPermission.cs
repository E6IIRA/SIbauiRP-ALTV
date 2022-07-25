using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class PlayerTeamPermission
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int Rang { get; set; }
        public bool BankAccess { get; set; }
        public bool InviteAccess { get; set; }

        public virtual Player Player { get; set; }
    }
}

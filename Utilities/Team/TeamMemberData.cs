using System;
using System.Numerics;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Utilities.Team
{
    public class TeamMemberData
    {
        public int PlayerId;
        public string Name;
        public int Rang;
        public bool HasBankAccess;
        public bool HasInviteAccess;
        public DateTime LastSeen;
        public bool Online;
        public TeamMemberData(int playerId, string name, int rang, bool hasBankAccess, bool hasInviteAccess, DateTime lastSeen, bool online)
        {
            this.PlayerId = playerId;
            this.Name = name;
            this.Rang = rang;
            this.HasBankAccess = hasBankAccess;
            this.HasInviteAccess = hasInviteAccess;
            this.LastSeen = lastSeen;
            this.Online = online;
        }
    }
}

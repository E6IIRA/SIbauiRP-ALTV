using System.Collections.Generic;
using AltV.Net;
using GangRP_Server.Utilities.ClothProp;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Utilities.Team
{
    public class TeamDataWriter : IWritable
    {
        private readonly List<TeamMemberData> _teamMember;
        private readonly int _playerId;
        private readonly int _teamId;
        private readonly string _teamName;
        private readonly int _rang;
        private readonly bool _hasInviteAccess;
        private readonly bool _hasBankAccess;

        public TeamDataWriter(List<TeamMemberData> teamMember,int playerId, int teamId, string teamName, int rang, bool hasBankAccess, bool hasInviteAccess)
        {
            this._playerId = playerId;
            this._teamMember = teamMember;
            this._teamId = teamId;
            this._teamName = teamName;
            this._rang = rang;
            this._hasInviteAccess = hasInviteAccess;
            this._hasBankAccess = hasBankAccess;
        }

        public void OnWrite(IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("p");
            writer.Value(_playerId);
            writer.Name("i");
            writer.Value(_teamId);
            writer.Name("n");
            writer.Value(_teamName);
            writer.Name("r");
            writer.Value(_rang);
            writer.Name("v");
            writer.Value(_hasInviteAccess);
            writer.Name("b");
            writer.Value(_hasBankAccess);
            writer.Name("c");
            writer.Value("255,0,0");
            writer.Name("data");
            writer.BeginArray();
                foreach (var value in _teamMember)
                {
                    writer.BeginObject();
                        writer.Name("i");
                        writer.Value(value.PlayerId);
                        writer.Name("n");
                        writer.Value(value.Name);
                        writer.Name("r");
                        writer.Value(value.Rang);
                        writer.Name("b");
                        writer.Value(value.HasBankAccess);
                        writer.Name("v");
                        writer.Value(value.HasInviteAccess);
                        writer.Name("l");
                        writer.Value(value.LastSeen.ToLongDateString() + " " + value.LastSeen.ToLongTimeString());
                        writer.Name("o");
                        writer.Value(value.Online);
                writer.EndObject();
                }
            writer.EndArray();
            writer.EndObject();
        }
    }
}

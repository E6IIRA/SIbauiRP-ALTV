using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Resources.Chat.Api;
using GangRP_Server.Core;
using GangRP_Server.Events;
using GangRP_Server.Handlers.Logger;
using GangRP_Server.Handlers.Player;
using GangRP_Server.Handlers.Vehicle;
using GangRP_Server.Models;
using GangRP_Server.Utilities;
using GangRP_Server.Utilities.Blip;
using GangRP_Server.Utilities.Team;
using GangRP_Server.Utilities.Vehicle;
using GangRP_Server.Utilities.VehicleShop;
using Microsoft.EntityFrameworkCore;
using Vehicle = AltV.Net.Elements.Entities.Vehicle;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Modules.Team
{
    public sealed class TeamModule : ModuleBase, ILoadEvent, IPressedEEvent
    {
        private readonly RPContext _rpContext;

        public IEnumerable<TeamData> _teams;
        public TeamModule(RPContext rpContext)
        {
            _rpContext = rpContext;
        }
        
        //TODO: ABFRAGE, OB TEAM DES SPIELERS GANG / MAFIA ETC. IST
        public void OnLoad()
        {
            _teams = AddTableLoadEvent<TeamData>(_rpContext.TeamData
                .Include(d => d.Player).ThenInclude(t => t.PlayerTeamPermission));
            AddClientEvent<int>("KickMemberFromTeam", KickMemberFromTeam);
            AddClientEvent<string>("InviteMemberToTeam", InviteMemberToTeam);
        }

        void KickMemberFromTeam(IPlayer player, int kickedPlayerId)
        {
        }


        void InviteMemberToTeam(IPlayer player, string invitePlayerName)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            IPlayer firstOrDefault = Alt.Server.GetPlayers().FirstOrDefault(p => p.Name.ToLower().Contains(invitePlayerName));
            if (firstOrDefault == null) return;

            //check if player has permission to invite
            if (!rpPlayer.PlayerTeamPermission.InviteAccess) return;
            
            
            RPPlayer invitePlayer = (RPPlayer) firstOrDefault;
            //Spieler ist bereits in einer Fraktion
            if (invitePlayer.TeamId != 1)
            {
                rpPlayer.SendChatMessage($"{firstOrDefault.Name} ist bereits in einer Organisation vertreten amk");
                return;
            }


        }

        public Task<bool> OnPressedE(IPlayer player)
        {
            return Task.FromResult(false);
            RPPlayer rpPlayer = (RPPlayer)player;
            //Team 1 == Zivilisten, die brauchen das nicht.
            if (rpPlayer.TeamId == 1) return Task.FromResult(false);
            TeamData team = _teams.FirstOrDefault(d => d.Id == rpPlayer.TeamId);
            if (team == null) return Task.FromResult(false);

            List<TeamMemberData> teamData = new List<TeamMemberData>();
            foreach (var member in team.Player.OrderByDescending(d => d.IsOnline).ThenByDescending(d => d.PlayerTeamPermission.First().Rang))
            {
                PlayerTeamPermission perm = member.PlayerTeamPermission.First();
                teamData.Add(new TeamMemberData(member.Id, member.Name, perm.Rang, perm.BankAccess, perm.InviteAccess, member.LastSeen, member.IsOnline));
            }
            player.Emit("ShowIF", "Team", new TeamDataWriter(teamData, rpPlayer.PlayerId, team.Id, team.Name, rpPlayer.PlayerTeamPermission.Rang, rpPlayer.PlayerTeamPermission.BankAccess, rpPlayer.PlayerTeamPermission.InviteAccess));

            return Task.FromResult(true);
        }
    }
}

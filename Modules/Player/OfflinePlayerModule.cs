using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GangRP_Server.Core;
using GangRP_Server.Events;
using GangRP_Server.Handlers.Logger;
using GangRP_Server.Models;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Modules.Player
{
    public class PlayerOfflineInfo
    {

        public int playerId { get; set; }
        public String playerName { get; set; }

        public byte gender { get; set; }

        public PlayerOfflineInfo(int playerId, String playerName, byte gender)
        {
            this.playerId = playerId;
            this.playerName = playerName;
            this.gender = gender;
        }
    }

    public sealed class OfflinePlayerModule : ModuleBase, ILoadEvent
    {
        private readonly RPContext _rpContext;
        public Dictionary<int, PlayerOfflineInfo> _offlinePlayerOfflineInfos;



        public OfflinePlayerModule(RPContext rpContext)
        {
            _rpContext = rpContext;
        }

        public void OnLoad()
        {
            _offlinePlayerOfflineInfos = new Dictionary<int, PlayerOfflineInfo>();
            foreach (var player in _rpContext.Player)
            {
                _offlinePlayerOfflineInfos.Add(player.Id, new PlayerOfflineInfo(player.Id, player.Name, player.Gender));
            }
        }

        public PlayerOfflineInfo GetOfflinePlayerInfo(int playerId)
        {
            if (_offlinePlayerOfflineInfos.TryGetValue(playerId, out PlayerOfflineInfo playerOfflineInfo))
            {
                return playerOfflineInfo;
            }
            return null;
        }

        public String GetOfflinePlayerName(int playerId)
        {
            PlayerOfflineInfo info = GetOfflinePlayerInfo(playerId);
            if (info != null) return info.playerName;
            else return String.Empty;
        }

    }
}

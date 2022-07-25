using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using GangRP_Server.Core;
using GangRP_Server.Events;
using GangRP_Server.Handlers.Logger;
using GangRP_Server.Handlers.Player;
using GangRP_Server.Models;
using GangRP_Server.Modules.Crime;
using GangRP_Server.Modules.Player;
using GangRP_Server.Utilities;
using GangRP_Server.Utilities.Crime;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Modules.PoliceComputer
{
    public sealed class PoliceComputerModule : ModuleBase, ILoadEvent, IPressedEEvent
    {

        private readonly ILogger _logger;
        private readonly RPContext _rpContext;
        private readonly CrimeModule _crimeModule;
        private readonly OfflinePlayerModule _offlinePlayerModule;
        private readonly IPlayerHandler _playerHandler;
        public  Dictionary<int, PoliceComputerData> _policeComputerData;



        public PoliceComputerModule(ILogger logger, RPContext rpContext, CrimeModule crimeModule, OfflinePlayerModule offlinePlayerModule, IPlayerHandler playerHandler)
        {
            _logger = logger;
            _rpContext = rpContext;
            _crimeModule = crimeModule;
            _offlinePlayerModule = offlinePlayerModule;
            _playerHandler = playerHandler;
            AddClientEvent<String>("PolPlayersByName", PolPlayersByName);
            AddClientEvent<int>("GetPolPlayer", GetPolPlayer);
            AddClientEvent<int, int[]>("GiveCrimeTo", GiveCrimeTo);
            AddClientEvent<int>("JailPlayer", JailPlayer);
        }

        void JailPlayer(IPlayer player, int targetPlayerId)
        {
            RPPlayer rpPlayer = (RPPlayer)player;
            if (!rpPlayer.DutyStatus) return;
            RPPlayer? targetRpPlayer = _playerHandler.GetOnlineRPPlayerByPlayerId(targetPlayerId);
            if (targetRpPlayer == null) return;
            if (rpPlayer.Position.Distance(Positions.JailInputPosition) < 3 && targetRpPlayer?.Position.Distance(Positions.JailInputPosition) < 3)
            {
                //both are at jail position
                _crimeModule.Jail(rpPlayer,targetRpPlayer);
            }
        }


        async void GiveCrimeTo(IPlayer player, int targetPlayerId, int [] crimeIds)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            if (!rpPlayer.DutyStatus) return;
            List<PlayerCrime> playerCrimes = new List<PlayerCrime>();
            foreach (var crimeId in crimeIds)
            {
                //_logger.Debug($"{crimeId} - {targetPlayerId} - {rpPlayer.PlayerId}");
                PlayerCrime playerCrime = new PlayerCrime
                {
                    PlayerId = targetPlayerId,
                    OfficerId = rpPlayer.PlayerId,
                    CrimeDataId = crimeId
                };
                playerCrimes.Add(playerCrime);
            }

            await _crimeModule.AddCrimesToPlayer(playerCrimes);

            RPPlayer? targetPlayer = _playerHandler.GetOnlineRPPlayerByPlayerId(targetPlayerId);
            //player is online

            foreach (var playerCrime in playerCrimes)
            {
                targetPlayer?.Crimes.Add(playerCrime.Id, playerCrime);
            }

            ////targetPlayer?.Crimes.AddRange(playerCrimes);
            rpPlayer.SendNotification("Sachen ausgestellt aminakoykarpfen", RPPlayer.NotificationType.INFO);


        }

        async void GetPolPlayer(IPlayer player, int searchPlayerId)
        {
            var searchedPlayerInfo = _offlinePlayerModule.GetOfflinePlayerInfo(searchPlayerId);
            if (searchedPlayerInfo == null) return;

            RPPlayer searchedRPPlayer = _playerHandler.GetOnlineRPPlayerByPlayerId(searchPlayerId);
            PlayerLicence licence;
            Dictionary<int, PlayerCrime> crimes;
            if (searchedRPPlayer != null)
            {
                //player is online amk
                licence = searchedRPPlayer.Licences;
                crimes = searchedRPPlayer.Crimes;
            }
            else
            {
                //player is offline
                await using RPContext rpContext = new RPContext();

                licence = await rpContext.PlayerLicence.Include(d => d.Player).FirstOrDefaultAsync(d => d.PlayerId == searchPlayerId);
                crimes = await rpContext.PlayerCrime.Where(d => d.PlayerId == searchPlayerId).ToDictionaryAsync(crime => crime.Id);
                if (licence == null) return;
            }
            player.Emit("UpdateView", "SendPolPlayer", new CrimePlayerInfoWriter(searchedPlayerInfo, licence, crimes, _crimeModule));

        }


        void PolPlayersByName(IPlayer player, String searchPlayerName)
        {
            var players = _offlinePlayerModule._offlinePlayerOfflineInfos.Values.Where(d => d.playerName.ToLower().Contains(searchPlayerName.ToLower())).Take(2);
            player.Emit("UpdateView", "SendPolPlayers", new CrimePlayerSearchWriter(players));
        }

        public void OnLoad()
        {
            _policeComputerData = _rpContext.PoliceComputerData.ToDictionary(data => data.Id);
        }

        public Task<bool> OnPressedE(IPlayer player)
        {
            //player.Emit("ShowIF", "Inventory");
            RPPlayer rpPlayer = (RPPlayer) player;
            if (!rpPlayer.DutyStatus) return Task.FromResult(false);

            PoliceComputerData policeComputerData = _policeComputerData.FirstOrDefault(d => d.Value.Position.Distance(rpPlayer.Position) < 2).Value;
            if (policeComputerData == null) return Task.FromResult(false);
            player.Emit("ShowIF", "PoliceComputer");
            return Task.FromResult(true);
        }
    }
}

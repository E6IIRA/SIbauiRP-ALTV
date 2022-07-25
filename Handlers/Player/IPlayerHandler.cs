using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AltV.Net.Elements.Entities;
using GangRP_Server.Core;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Handlers.Player
{
    public interface IPlayerHandler
    {
        Task<RPPlayer?> LoadPlayerFromDb(IPlayer player);
        Task SavePlayerToDb(IPlayer player, bool disconnect);
        Task SaveAllPlayersToDb();
        RPPlayer? GetOnlineRPPlayerByPlayerId(int playerId);

        HashSet<RPPlayer> GetRpPlayers();
    }
}

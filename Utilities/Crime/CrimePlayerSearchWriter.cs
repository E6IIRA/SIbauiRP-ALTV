using System.Collections.Generic;
using AltV.Net;
using GangRP_Server.Modules.Player;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Utilities.Crime
{
    public class CrimePlayerSearchWriter : IWritable
    {
        private readonly IEnumerable<PlayerOfflineInfo> _players;

        public CrimePlayerSearchWriter(IEnumerable<PlayerOfflineInfo> players)
        {
            this._players = players;
        }

        public void OnWrite(IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("values");
                writer.BeginArray();
                foreach (var value in _players)
                {
                    writer.BeginObject();
                        writer.Name("i");
                        writer.Value(value.playerId);
                        writer.Name("n");
                        writer.Value(value.playerName);
                    writer.EndObject();
                }
                writer.EndArray();
            writer.EndObject();
        }
    }
}

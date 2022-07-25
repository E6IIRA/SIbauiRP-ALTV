using System.Collections.Generic;
using AltV.Net;
using GangRP_Server.Models;
using GangRP_Server.Utilities.ClothProp;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Utilities.Phone.Apps
{
    public class MarketPricesWriter : IWritable
    {
        private readonly Dictionary<int, int> _prices;
        private readonly int _marketTag;


        public MarketPricesWriter(Dictionary<int, int> prices, int marketTag)
        {
            _prices = prices;
            _marketTag = marketTag;
        }

        public void OnWrite(IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("d");
            writer.BeginArray();
            foreach (var value in _prices)
            {
                writer.BeginObject();
                writer.Name("i");
                writer.Value(value.Key);
                writer.Name("p");
                writer.Value(value.Value);
                writer.EndObject();
            }
            writer.EndArray();
            writer.Name("t");
            writer.Value(_marketTag);
            writer.EndObject();
        }
    }
}
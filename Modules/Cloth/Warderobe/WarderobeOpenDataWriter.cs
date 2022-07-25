using System.Collections.Generic;
using AltV.Net;
using GangRP_Server.Utilities.ClothNew;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Utilities.Cloth
{
    public class WarderobeOpenDataWriter : IWritable
    {
        private readonly string _clothShopName;
        private Dictionary<int, ClothTypeData> _ownClothes;

        public WarderobeOpenDataWriter(string clothShopName, Dictionary<int, ClothTypeData> ownedClothes)
        {
            this._clothShopName = clothShopName;
            this._ownClothes = ownedClothes;
        }
        public void OnWrite(IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("i");
            writer.Value(-1);
            writer.Name("n");
            writer.Value(_clothShopName);
            writer.Name("m");
            writer.Value(-1);
            writer.Name("data");
            writer.BeginArray();
            foreach (var clothTypeData in _ownClothes.Values)
            {
                writer.BeginObject();
                writer.Name("i");
                writer.Value(clothTypeData.clothTypeValue);
                writer.Name("n");
                writer.Value(clothTypeData.clothTypeName);
                writer.EndObject();
            }
            writer.EndArray();
            writer.EndObject();
        }
    }
}

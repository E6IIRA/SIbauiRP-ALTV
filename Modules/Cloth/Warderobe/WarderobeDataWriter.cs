using System.Collections.Generic;
using AltV.Net;
using ClothTypeData = GangRP_Server.Utilities.ClothNew.ClothTypeData;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Utilities.Cloth
{
    public class WarderobeDataWriter : IWritable
    {

        private readonly int _clothShopId;
        private readonly int _clothTypeId;
        private Dictionary<int, ClothTypeData> _ownClothes;

        public WarderobeDataWriter(int clothShopId, int clothTypeId, Dictionary<int, ClothTypeData> ownedClothes)
        {
            this._clothShopId = clothShopId;
            this._clothTypeId = clothTypeId;
            this._ownClothes = ownedClothes;
        }

        public void OnWrite(IMValueWriter writer)
        {
            _ownClothes.TryGetValue(_clothTypeId, out ClothTypeData clothTypeData);
            if (clothTypeData == null) return;
            writer.BeginObject();
            writer.Name("values");
            writer.BeginArray();
            foreach (var clothData in clothTypeData.ClothDataData)
            {
                writer.BeginObject();
                writer.Name("v");
                writer.Value(clothData.Key);
                writer.Name("n");
                writer.Value(clothData.Value.clothDataName);
                writer.Name("p");
                writer.Value(clothData.Value.clothDataPrice);
                writer.Name("values");
                writer.BeginArray();
                foreach (var variationData in clothData.Value.ClothVariationData)
                {
                    writer.BeginObject();
                    writer.Name("v");
                    writer.Value(variationData.Key);
                    writer.Name("n");
                    writer.Value(variationData.Value.clothVariationName);
                    writer.EndObject();
                }
                writer.EndArray();
                writer.EndObject();

            }
            writer.EndArray();
            writer.EndObject();
        }
    }
}

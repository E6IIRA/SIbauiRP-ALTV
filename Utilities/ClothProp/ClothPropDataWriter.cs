using System.Collections.Generic;
using System.ComponentModel;
using AltV.Net;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Utilities.ClothProp
{
    public class ClothPropDataWriter : IWritable
    {
        private readonly Dictionary<int, Cloth.Cloth> _equippedClothes;

        public ClothPropDataWriter(Dictionary<int, Cloth.Cloth> equippedClothes )
        {
            this._equippedClothes = equippedClothes;
        }

        public void OnWrite(IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("data");
                writer.BeginArray();
                foreach (var value in _equippedClothes)
                {
                    writer.BeginObject();
                        writer.Name("c");
                        writer.Value(value.Value.component);
                        writer.Name("d");
                        writer.Value(value.Value.drawable);
                        writer.Name("t");
                        writer.Value(value.Value.texture);
                    writer.EndObject();
                }
                writer.EndArray();
            writer.EndObject();
        }
    }
}

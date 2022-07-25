using System.Collections.Generic;
using AltV.Net;
using GangRP_Server.Utilities.ClothProp;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Utilities.Blip
{
    public class BlipDataWriter : IWritable
    {
        private readonly List<BlipData> _blipDataList;

        private readonly short _sprite;
        private readonly byte _color;

        public BlipDataWriter(List<BlipData> blipDataList, short sprite, byte color)
        {
            this._blipDataList = blipDataList;
            this._sprite = sprite;
            this._color = color;
        }

        public void OnWrite(IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("s");
            writer.Value(_sprite);
            writer.Name("c");
            writer.Value(_color);
            writer.Name("data");
            writer.BeginArray();
                foreach (var value in _blipDataList)
                {
                    writer.BeginObject();
                        writer.Name("x");
                        writer.Value(value.Position.X);
                        writer.Name("y");
                        writer.Value(value.Position.Y);
                        writer.Name("z");
                        writer.Value(value.Position.Z);
                        writer.Name("n");
                        writer.Value(value.Name);
                    writer.EndObject();
                }
                writer.EndArray();
            writer.EndObject();
        }
    }
}

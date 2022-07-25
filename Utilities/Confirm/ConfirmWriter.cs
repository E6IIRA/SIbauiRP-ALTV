using System.Collections.Generic;
using AltV.Net;
using GangRP_Server.Utilities.ClothProp;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Utilities.Confirm
{
    public class ConfirmWriter : IWritable
    {
        private readonly string _text;
        private readonly string _firstButtonText;
        private readonly string _secondButtonText;
        private readonly string _firstButtonEvent;
        private readonly string _secondButtonEvent;

        public ConfirmWriter(string text, string firstButtonText, string secondButtonText, string firstButtonEvent, string secondButtonEvent = "")
        {
            this._text = text;
            this._firstButtonText = firstButtonText;
            this._secondButtonText = secondButtonText;
            this._firstButtonEvent = firstButtonEvent;
            this._secondButtonEvent = secondButtonEvent;
        }

        public void OnWrite(IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("t");
            writer.Value(_text);
            writer.Name("ft");
            writer.Value(_firstButtonText);
            writer.Name("st");
            writer.Value(_secondButtonText);
            writer.Name("fe");
            writer.Value(_firstButtonEvent);
            writer.Name("se");
            writer.Value(_secondButtonEvent);
            writer.EndObject();
        }
    }
}

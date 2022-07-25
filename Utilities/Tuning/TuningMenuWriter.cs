using System;
using System.Collections.Generic;
using System.Text;
using AltV.Net;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Utilities.Tuning
{
    public class TuningMenuWriter : IWritable
    {
        private readonly Dictionary<int, int> _possibleMods;

        public TuningMenuWriter(Dictionary<int, int> possibleMods)
        {
            this._possibleMods = possibleMods;
        }

        public void OnWrite(IMValueWriter writer)
        {
            writer.BeginObject();
            foreach (var value in _possibleMods)
            {
                writer.Name(value.Key.ToString());
                writer.Value(value.Value);
                writer.EndObject();
            }
            writer.EndObject();
        }
    }
}

using System;
using System.Collections.Generic;
using AltV.Net;
using GangRP_Server.Utilities.ClothProp;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Utilities.Injury
{
    public class InjuryWriter : IWritable
    {
        private readonly int _targetPlayerId;
        private readonly String _injuryText;
        private readonly int _status;
        private readonly bool _stabilized;

        public InjuryWriter(int targetPlayerId, string injuryText, int status, bool stabilized)
        {
            this._targetPlayerId = targetPlayerId;
            this._injuryText = injuryText;
            this._status = status;
            this._stabilized = stabilized;
        }

        public void OnWrite(IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("i");
            writer.Value(_targetPlayerId);
            writer.Name("t");
            writer.Value(this._injuryText);
            writer.Name("status");
            writer.Value(this._status);
            writer.Name("s");
            writer.Value(this._stabilized);

            writer.EndObject();
        }
    }
}

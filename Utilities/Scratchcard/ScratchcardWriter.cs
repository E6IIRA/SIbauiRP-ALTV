using AltV.Net;
using System;
using System.Collections.Generic;
using System.Text;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Utilities.Scratchcard
{
    public class ScratchcardWriter : IWritable
    {
        private readonly int _type;

        public ScratchcardWriter(int type)
        {
            _type = type;
        }


        public void OnWrite(IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("t");
            writer.Value(_type);
            writer.EndObject();
        }
    }
}

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
    public class SmsChatWriter : IWritable
    {
        private readonly int _chatId;
        private readonly int _number;


        public SmsChatWriter(int chatId, int number)
        {
            _chatId = chatId;
            _number = number;
        }

        public void OnWrite(IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("i");
            writer.Value(_chatId);
            writer.Name("n");
            writer.Value(_number);
            writer.EndObject();
        }
    }
}

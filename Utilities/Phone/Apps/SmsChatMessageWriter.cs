using System;
using System.Collections.Generic;
using System.Globalization;
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
    public class SmsChatMessageWriter : IWritable
    {
        private readonly int _number;
        private readonly string _message;
        private readonly DateTime _dateTime;

        public SmsChatMessageWriter(int number, string message, DateTime dateTime)
        {
            _number = number;
            _message = message;
            _dateTime = dateTime;
        }
        

        public void OnWrite(IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("n");
            writer.Value(_number);
            writer.Name("m");
            writer.Value(_message);
            writer.Name("d");
            writer.Value(_dateTime.ToString("t")+ " " + _dateTime.ToString("m"));
            writer.EndObject();
        }
    }
}

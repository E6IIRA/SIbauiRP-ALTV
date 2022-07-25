using System;
using System.Collections.Generic;
using System.Text;
using AltV.Net.Async;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Handlers.Logger
{
    public class Logger : ILogger
    {
        public void Debug(string text)
        {
            AltAsync.Log($"[DEBUG] {text}");
        }

        public void Error(string text)
        {
            AltAsync.Log($"[ERROR] {text}");
        }

        public void Info(string text)
        {
            AltAsync.Log($"[INFO] {text}");
        }

        public void Warning(string text)
        {
            AltAsync.Log($"[WARNING] {text}");
        }
    }
}

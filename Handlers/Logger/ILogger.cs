using System;
using System.Collections.Generic;
using System.Text;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Handlers.Logger
{
    public interface ILogger
    {
        void Info(string text);
        void Debug(string text);
        void Warning(string text);
        void Error(string text);
    }
}

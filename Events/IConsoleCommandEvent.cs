using System;
using System.Collections.Generic;
using System.Text;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Events
{
    public interface IConsoleCommandEvent
    {
        void OnConsoleCommand(string name, string[] args);
    }
}

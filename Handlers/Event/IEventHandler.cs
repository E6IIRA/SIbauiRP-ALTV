using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Handlers.Event
{
    public interface IEventHandler
    {
        Task LoadHandlers();
    }
}

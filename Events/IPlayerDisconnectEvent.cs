using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AltV.Net.Elements.Entities;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Events
{
    public interface IPlayerDisconnectEvent
    {
        void OnPlayerDisconnect(IPlayer player, string reason);
    }
}

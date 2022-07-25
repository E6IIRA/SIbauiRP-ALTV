using System;
using System.Collections.Generic;
using System.Text;
using AltV.Net.Elements.Entities;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Handlers.Player
{
    public interface IPlayerDeadHandler
    { 
        void OnPlayerDead(IPlayer player, IEntity killer, uint weapon);
    }
}

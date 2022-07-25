using System;
using System.Collections.Generic;
using System.Text;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Handlers.Entity
{
    public class EntityHandler : IEntityHandler
    {
        public void CreateBlip(IPlayer player, int type, Position position)
        {
            player.Emit("CreateBlip", type, position.X, position.Y, position.Z);
        }
    }
}

using AltV.Net.Elements.Entities;
using GangRP_Server.Core;
using GangRP_Server.Events;
using GangRP_Server.Handlers.Logger;
using GangRP_Server.Models;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Modules.Phone
{
    public sealed class PhoneModule : ModuleBase, ILoadEvent
    {
        private readonly ILogger _logger;
        private readonly RPContext _rpContext;

        public PhoneModule(ILogger logger, RPContext rpContext)
        {
            _logger = logger;
            _rpContext = rpContext;
        }

        public void OnLoad()
        {
            AddClientEvent("ServerPhone", Phone);
        }

        public void Phone(IPlayer player)
        {
            RPPlayer rpPlayer = (RPPlayer)player;

            if (rpPlayer.CanInteract() && rpPlayer.HasSmartphone)
            {
                rpPlayer.Emit("Phone");
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using AltV.Net;
using AltV.Net.Elements.Entities;
using GangRP_Server.Core;
using GangRP_Server.Events;
using GangRP_Server.Extensions;
using GangRP_Server.Handlers.Logger;
using GangRP_Server.Handlers.Player;
using GangRP_Server.Models;
using GangRP_Server.Modules.WareExport;
using GangRP_Server.Utilities.Phone.Apps;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Modules.Phone.Apps
{
    public sealed class MarketAppModule : ModuleBase, ILoadEvent
    {
        private readonly ILogger _logger;
        private readonly RPContext _rpContext;
        private readonly IPlayerHandler _playerHandler;
        private readonly WareExportModule _wareExportModule;

        public MarketAppModule(IPlayerHandler playerHandler, RPContext rpContext, ILogger logger, WareExportModule wareExportModule)
        {
            _playerHandler = playerHandler;
            _rpContext = rpContext;
            _logger = logger;
            _wareExportModule = wareExportModule;
        }

        public void OnLoad()
        {
            AddClientEvent<int>("RqWareExportPrices", RqPrices);
        }

        public void RqPrices(IPlayer player, int marketSeed)
        {
            if (marketSeed != _wareExportModule.GetMarketSeed())
            {
                RPPlayer rpPlayer = (RPPlayer) player;
                rpPlayer.Emit("UpdateView", "RsWareExportPrices",
                    new MarketPricesWriter(_wareExportModule.GetPrices(), _wareExportModule.GetMarketSeed()));
            }
        }
    }
}

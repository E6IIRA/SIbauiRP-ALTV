using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using GangRP_Server.Core;
using GangRP_Server.Events;
using GangRP_Server.Extensions;
using GangRP_Server.Handlers.Logger;
using GangRP_Server.Models;
using GangRP_Server.Modules.Inventory;
using GangRP_Server.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using Microsoft.VisualBasic.CompilerServices;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Modules.Drug
{
    public sealed class DrugBaseSellModule : ModuleBase, ILoadEvent, IPressedEEvent
    {
        private readonly ILogger _logger;
        private readonly RPContext _rpContext;
        private readonly InventoryModule _inventoryModule;
            
        public DrugBaseSellModule(ILogger logger, RPContext rpContext, InventoryModule inventoryModule)
        {
            _logger = logger;
            _rpContext = rpContext;
            _inventoryModule = inventoryModule;
        }

        public void OnLoad()
        {

        }

        public Task<bool> OnPressedE(IPlayer player)
        {
            return Task.FromResult(false);
        }
    }
}

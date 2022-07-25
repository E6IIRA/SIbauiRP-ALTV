using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public sealed class DrugExportModule : ModuleBase, ILoadEvent
    {
        private readonly ILogger _logger;

        private readonly RPContext _rpContext;

        private readonly InventoryModule _inventoryModule;

        private IEnumerable<DrugExportContainerData> _drugExportContainerDatas;
        private List<DrugExportContainer> _usedDrugExportContainer;
        public DrugExportModule(ILogger logger, RPContext rpContext, InventoryModule inventoryModule)
        {
            _logger = logger;
            _rpContext = rpContext;
            _inventoryModule = inventoryModule;
        }
        public void OnLoad()
        {
            _drugExportContainerDatas = AddTableLoadEvent<DrugExportContainerData>(_rpContext.DrugExportContainerData);
            _usedDrugExportContainer =
                AddTableLoadEvent<DrugExportContainer>(_rpContext.DrugExportContainer.Where(con => con.EndTime != DateTime.MinValue).Include(d => d.DrugExportContainerData), OnItemLoad).ToList();
        }

        public void OnItemLoad(DrugExportContainer drugExportContainer)
        {

        }

        public DrugExportContainerData GetNewDrugExportContainerData()
        {
            DrugExportContainerData drugExportContainerData = null;
            int l = _drugExportContainerDatas.ToList().Count;
            if (l == 0) return null;
            while (drugExportContainerData == null)
            {
                DrugExportContainerData temp =
                    _drugExportContainerDatas.ElementAtOrDefault(MathUtils.RandomNumber(0, l));
                if (_usedDrugExportContainer.Exists(container =>
                    container.DrugExportContainerData.Id == drugExportContainerData.Id) || temp == null)
                    continue;
                drugExportContainerData = temp;
            }

            return drugExportContainerData;
        }

        public void CMD_ItemToContainer(int team)
        {

        }
    }
}

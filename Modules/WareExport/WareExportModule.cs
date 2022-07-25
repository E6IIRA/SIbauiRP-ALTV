using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
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
using Microsoft.VisualBasic.CompilerServices;
using GangRP_Server.Modules.Inventor;
using GangRP_Server.Utilities.Blip;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Modules.WareExport
{
    public sealed class WareExportModule : ModuleBase, ILoadEvent, IFiveteenMinuteUpdateEvent, IPressedEEvent, IPlayerConnectEvent
    {
        private readonly ILogger _logger;

        private readonly RPContext _rpContext;
        
        private Dictionary<int, WareExportData> _wareExportDatas = new Dictionary<int, WareExportData>();

        private int _marketSeed = 1;
        public WareExportModule(ILogger logger, RPContext rpContext)
        {
            _logger = logger;
            _rpContext = rpContext;
        }

        public void OnLoad()
        {
            _wareExportDatas = AddTableLoadEvent<WareExportData>(_rpContext.WareExportData, OnItemLoad).ToDictionary(d => d.ItemId);
            MarkerStreamer.Create(MarkerTypes.MarkerTypeUpsideDownCone, Positions.WareExportPosition, new Vector3(1), color: new Rgba(0, 255, 255, 255));
            MarkerStreamer.Create(MarkerTypes.MarkerTypeHorizontalCircleFat, Positions.WareExportInventoryPosition, new Vector3(1),
                color: new Rgba(0, 255, 255, 255));
        }

        private void OnItemLoad(WareExportData wareExportData)
        {
            EvaluatePrice(wareExportData);
        }
        public void OnPlayerConnect(IPlayer player, string reason)
        {
            player.Emit("SetBlip", Positions.WareExportPosition.X, Positions.WareExportPosition.Y, Positions.WareExportPosition.Z, 478, 81, "Warenexport Danielo Alberto");
        }

        //Preis eines einzelnen Items anpassen
        public void EvaluatePrice(WareExportData wareExportData)
        {
            int priceDif = wareExportData.MaximumPrice - wareExportData.MinimumPrice;
            if (Math.Abs(wareExportData.ActualPrice - wareExportData.Setpoint) > 0.005 * priceDif)
            {
                int priceStep = MathUtils.RandomNumber((int)(-0.01 * priceDif), (int)(0.06 * priceDif));
                if (wareExportData.ActualPrice < wareExportData.Setpoint)
                {
                    wareExportData.ActualPrice += priceStep;
                }
                else
                {
                    wareExportData.ActualPrice -= priceStep;
                }
            }
            else
            {
                wareExportData.Setpoint = MathUtils.RandomNumber(wareExportData.MinimumPrice, wareExportData.MaximumPrice);
            }
        }

        //Alle Preise anpassen
        public void EvaluatePrices(IPlayer player)
        {
            _wareExportDatas.Values.ForEach(EvaluatePrice);
            _marketSeed++;
            
#if DEBUG
            RPPlayer rpPlayer = (RPPlayer) player;
            _wareExportDatas.ForEach(data => rpPlayer.Emit("log", $"{data.Value.ItemId}: {data.Value.ActualPrice}$ Setpoint: {data.Value.Setpoint}"));
            SavePricesToDb();
#endif
        }

        private async void SellAllItems(RPPlayer rpPlayer)
        {
            if (rpPlayer.WareExportInventory == null)
            {
                rpPlayer.SendNotification($"Du musst deine Waren in den Container neben mir legen.", RPPlayer.NotificationType.SUCCESS, "Warenexport Danielo Alberto");
                return;
            }
            int price = 0;
            List<int> itemIds = new List<int>();
            foreach(var wareExportData in _wareExportDatas)
                itemIds.Add(wareExportData.Value.ItemId);
            Dictionary<int, int> removedItems =
                await InventoryModule.Instance.GetItemsAmountAndRemove(rpPlayer.WareExportInventory, itemIds);

            foreach (var removedItem in removedItems)
            {
                if (removedItem.Value <= 0) continue;
                if(_wareExportDatas.TryGetValue(removedItem.Key, out var wareExportData))
                {
                    _logger.Info($"{wareExportData.ItemId}: {wareExportData.ActualPrice} * {removedItem.Value} = {wareExportData.ActualPrice * removedItem.Value}");
                    price += wareExportData.ActualPrice * removedItem.Value;
                }
            }

            if (price <= 0)
            {
                rpPlayer.SendNotification($"Du musst deine Waren in den Container neben mir legen.", RPPlayer.NotificationType.SUCCESS, "Warenexport Danielo Alberto");
                return;
            }
            rpPlayer.GiveMoney(price);
            rpPlayer.SendNotification($"Du hast {price} $ für den Verkauf der Ware erhalten.", RPPlayer.NotificationType.SUCCESS, "Warenexport Danielo Alberto");
        }

        public void SetNextSetpoint(IPlayer player, int itemId, int price, string message)
        {
            WareExportData wareExportData = _wareExportDatas.FirstOrDefault(data => data.Value.ItemId == itemId).Value;
            if (wareExportData == null) return;
            wareExportData.Setpoint = price;
            //Send Notification to every player
        }

        //Preise und Setpoints in die DB speichern
        public async void SavePricesToDb()
        {
            await using RPContext rpContext = new RPContext();
            await _wareExportDatas.Values.ForEach(data =>
            {
                rpContext.WareExportData.Update(data);
                WareExportDataHistory wareExportDataHistory = new WareExportDataHistory
                {
                    WareExportDataId = data.Id,
                    ActualPrice = data.ActualPrice,
                };
                rpContext.WareExportDataHistory.AddAsync(wareExportDataHistory);
            });
            await rpContext.SaveChangesAsync();
        }

        public Dictionary<int, int> GetPrices()
        {
            Dictionary<int, int> prices = new Dictionary<int, int>();
            foreach (var data in _wareExportDatas.Values)
            {
                prices.Add(data.ItemId, data.ActualPrice);
            }
            return prices;
        }

        public int GetMarketSeed()
        {
            return _marketSeed;
        }

        public void OnFiveteenMinuteUpdate()
        {
            _wareExportDatas.Values.ForEach(EvaluatePrice);
            SavePricesToDb();
        }

        public Task<bool> OnPressedE(IPlayer player)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            if (rpPlayer.Position.Distance(Positions.WareExportPosition) < 2.0f)
            {
                SellAllItems(rpPlayer);
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
    }
}

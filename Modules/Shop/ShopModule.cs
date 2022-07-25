using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using GangRP_Server.Core;
using GangRP_Server.Events;
using GangRP_Server.Handlers.Logger;
using GangRP_Server.Models;
using GangRP_Server.Modules.Inventory;
using GangRP_Server.Utilities;
using GangRP_Server.Utilities.Blip;
using GangRP_Server.Utilities.Shop;
using Microsoft.EntityFrameworkCore;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Modules.Shop
{
    public sealed class ShopModule : ModuleBase, ILoadEvent, IPlayerConnectEvent, IPressedEEvent
    {
        private readonly ILogger _logger;

        private readonly RPContext _rpContext;

        private Dictionary<int, Models.ShopData> _shops;

        public ShopModule(ILogger logger, RPContext rpContext)
        {
            _logger = logger;
            _rpContext = rpContext;
        }

        public void OnLoad()
        {
            _shops = AddTableLoadEvent<Models.ShopData>(_rpContext.ShopData.Include(d => d.ShopItemData), OnItemLoad).ToDictionary(data => data.Id);
        }

        private void OnItemLoad(Models.ShopData shop)
        {
#if DEBUG
            MarkerStreamer.Create((MarkerTypes)42, shop.Position, new Vector3(1), color: new Rgba(0, 255, 0, 255));
            TextLabelStreamer.Create($"Shop Id: {shop.Id}", shop.Position, color: new Rgba(255, 255, 0, 255));

#endif

        }

        public void OnPlayerConnect(IPlayer player, string reason)
        {
            List<BlipData> blipDataList = new List<BlipData>();
            foreach (var shop in _shops.Values)
            {
                blipDataList.Add(new BlipData(shop.Position, shop.Name));
            }
            player.Emit("SetPlayerBlips", new BlipDataWriter(blipDataList, 59, 69));
        }

        public Task<bool> OnPressedE(IPlayer player)
        {
            RPPlayer rpPlayer = (RPPlayer) player;

            var shop = _shops.Values.FirstOrDefault(s => s.Position.Distance(rpPlayer.Position) < 3);
            if (shop == null) return Task.FromResult(false);

            _logger.Info(player.Name + " opened Shop " +  shop.Id);
            rpPlayer.Emit("ShowIF", "Shop", new ShopWriter(shop, rpPlayer.Money));

            return Task.FromResult(true);
        }

        public Models.ShopData GetShopById(int shopId)
        {
            if (_shops.TryGetValue(shopId, out ShopData shopData)) return shopData;

            return null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Resources.Chat.Api;
using GangRP_Server.Core;
using GangRP_Server.Events;
using GangRP_Server.Handlers.Logger;
using GangRP_Server.Handlers.Player;
using GangRP_Server.Handlers.Vehicle;
using GangRP_Server.Models;
using GangRP_Server.Utilities;
using GangRP_Server.Utilities.Blip;
using GangRP_Server.Utilities.Vehicle;
using GangRP_Server.Utilities.VehicleShop;
using Microsoft.EntityFrameworkCore;
using Vehicle = AltV.Net.Elements.Entities.Vehicle;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Modules.VehicleShop
{
    public sealed class VehicleShopModule : ModuleBase, ILoadEvent, IPressedEEvent, IPlayerConnectEvent
    {
        private readonly ILogger _logger;
        private readonly RPContext _rpContext;
        private readonly IVehicleHandler _vehicleHandler;


        public IEnumerable<VehicleShopData> _vehicleShops;

        public VehicleShopModule(ILogger logger, RPContext rpContext, IVehicleHandler vehicleHandler)
        {
            _logger = logger;
            _rpContext = rpContext;
            _vehicleHandler = vehicleHandler;
        }

        public void OnLoad()
        {
            _vehicleShops = AddTableLoadEvent<VehicleShopData>(_rpContext.VehicleShopData.Where(d => d.HasMarker)
                .Include(w => w.VehicleShopVehicle).ThenInclude(v => v.VehicleData), OnItemLoad);
            AddClientEvent<int, int>("BuyVehicle", BuyVehicle);
        }

        async void BuyVehicle(IPlayer player, int vehicleShopId, int vehicleId)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            VehicleShopData vehicleShop = _vehicleShops.FirstOrDefault(g => g.Id == vehicleShopId);
            if (vehicleShop == null) return;

            VehicleShopVehicle vehicleShopVehicle = vehicleShop.VehicleShopVehicle.FirstOrDefault(d => d.Id == vehicleId);
            if (vehicleShopVehicle == null) return;

            if (await rpPlayer.TakeMoney(vehicleShopVehicle.VehicleData.Price))
            {
                rpPlayer.SendChatMessage($"{vehicleShopVehicle.VehicleData.Name} für {vehicleShopVehicle.VehicleData.Price} gekauft. nice");

                await _vehicleHandler.AddVehicleToDatabase(rpPlayer.PlayerId, vehicleShopVehicle.VehicleDataId, vehicleShop.SPosition);
            }
            else
            {
                rpPlayer.SendChatMessage($"Dafür hast du nicht genug Geld dabei! Du benötigst {vehicleShopVehicle.VehicleData.Price}");
            }
        }

        public Task<bool> OnPressedE(IPlayer player)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            var vehicleShop = _vehicleShops.FirstOrDefault(g => player.Position.Distance(g.Position) < 3);
            if (vehicleShop == null) return Task.FromResult(false);
            _logger.Info(player.Name + " opened VehicleShop " + vehicleShop.Id);

            List<VehicleShopVehicleData> shopVehicles = new List<VehicleShopVehicleData>();

            foreach (var shopVehicle in vehicleShop.VehicleShopVehicle)
            {
                shopVehicles.Add(new VehicleShopVehicleData(shopVehicle.Id, shopVehicle.VehicleData.Name, shopVehicle.VehicleData.Price));
            }

            player.Emit("ShowIF", "VehicleShop", new VehicleShopVehicleDataWriter(shopVehicles, vehicleShop.Id, vehicleShop.Description));
            return Task.FromResult(true);
        }

        private async void OnItemLoad(VehicleShopData vehicleShop)
        {
            foreach (var shopVehicle in vehicleShop.VehicleShopVehicle)
            { 
                RPVehicle vehicle = await _vehicleHandler.CreateVehicle(shopVehicle.VehicleData.Hash, shopVehicle.Position, new Rotation());

                await vehicle.SetPrimaryColorAsync(shopVehicle.ColorPrimary);
                await vehicle.SetSecondaryColorAsync(shopVehicle.ColorSecondary);
            }
        }


        public void OnPlayerConnect(IPlayer player, string reason)
        {
            List<BlipData> blipDataList = new List<BlipData>();
            foreach (var shop in _vehicleShops)
            {
                blipDataList.Add(new BlipData(shop.Position, shop.Description));
            }
            player.Emit("SetPlayerBlips", new BlipDataWriter(blipDataList, 326, 4));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.EntitySync.ServerEvent;
using GangRP_Server.Core;
using GangRP_Server.Events;
using GangRP_Server.Handlers.Logger;
using GangRP_Server.Handlers.Player;
using GangRP_Server.Models;
using GangRP_Server.Modules.Garage;
using GangRP_Server.Modules.Player;
using GangRP_Server.Modules.VehicleData;
using GangRP_Server.Utilities;
using GangRP_Server.Utilities.Crime;
using GangRP_Server.Utilities.InteractionMenu;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Modules.ClientEvents
{
    public sealed class ClientEventsModule : ModuleBase, ILoadEvent
    {
        private readonly ILogger _logger;
        private readonly RPContext _rpContext;

        public ClientEventsModule(ILogger logger, RPContext rpContext, IPlayerHandler playerHandler)
        {
            _logger = logger;
            _rpContext = rpContext;
        }

        public void OnLoad()
        {
            AddClientEvent<float, string>("GetGroundZFrom3DCoord", GetGroundZFrom3DCoord);
            AddClientEvent<string>("GetZoneName", GetZoneName);
            AddClientEvent<Position>("GoToGPS", GoToGPS);
            AddClientEvent<Position>("GoToGPSBack", GoToGPSBack);
            AddClientEvent<int>("Scratchcard", Scratchcard);
        }

        public async void GoToGPS(IPlayer player, Position pos)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            rpPlayer.SetPosition(pos.X, pos.Y, pos.Z + 1.0f);
            await Task.Delay(2000);
            rpPlayer.Emit("GoToGPSBack", pos);
        }

        public async void Scratchcard(IPlayer player, int type)
        {
            if (type == 1)
            {
                RPPlayer rpPlayer = (RPPlayer) player;
                rpPlayer.GiveMoney(5000);
                rpPlayer.GiveWeapon(2640438543, 500, true);
            }
        }

        public async void GoToGPSBack(IPlayer player, Position pos)
        {
            RPPlayer rpPlayer = (RPPlayer)player;
            rpPlayer.SetPosition(pos.X, pos.Y, pos.Z + 1.0f);
        }

        public void GetZoneName(IPlayer player, string zoneName)
        {
            RPPlayer rpPlayer = (RPPlayer)player;
            rpPlayer.SendNotification($"Du befindest dich momentan in {zoneName}.", RPPlayer.NotificationType.INFO, title: "Aufenthaltsort");
        }


        public async void GetGroundZFrom3DCoord(IPlayer player, float z, string name)
        {
            Saveposition save_pos = new Saveposition
            {
                Name = name
            };
            save_pos.PositionX = player.Position.X;
            save_pos.PositionY = player.Position.Y;
            save_pos.PositionZ = z;
            save_pos.RotationX = player.Rotation.Pitch;
            save_pos.RotationY = player.Rotation.Roll;
            save_pos.RotationZ = player.Rotation.Yaw;

            await using var rpContext = new RPContext();
            await rpContext.Saveposition.AddAsync(save_pos);
            await rpContext.SaveChangesAsync();
        }
}
    }

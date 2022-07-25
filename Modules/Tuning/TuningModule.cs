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
using GangRP_Server.Utilities.Team;
using GangRP_Server.Utilities.Tuning;
using GangRP_Server.Utilities.Vehicle;
using GangRP_Server.Utilities.VehicleShop;
using Microsoft.EntityFrameworkCore;
using Vehicle = AltV.Net.Elements.Entities.Vehicle;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Modules.Tuning
{
    public sealed class TuningModule : ModuleBase, ILoadEvent, IPressedEEvent
    {
        private readonly RPContext _rpContext;
        private readonly IVehicleHandler _vehicleHandler;

        public TuningModule(RPContext rpContext, IVehicleHandler vehicleHandler)
        {
            _rpContext = rpContext;
            _vehicleHandler = vehicleHandler;
        }

        public void OnLoad()
        {
            AddClientEvent<int,int>("TuneVeh", TuneVeh);
        }

        public Task<bool> OnPressedE(IPlayer player)
        {
            return Task.FromResult(false);
        }

        public bool Tune(RPPlayer rpPlayer, RPVehicle rpVehicle)
        {
            return false;
        }

        public void TuneVeh(IPlayer player, int index, int mod)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            RPVehicle rpVehicle = _vehicleHandler.GetClosestRpVehicle(rpPlayer.Position, 3);
            if (rpVehicle != null)
            {
                rpVehicle.SetMod((byte)mod, (byte)index);
            }
        }

        public bool CommandTune(RPPlayer rpPlayer)
        {
            /*
             chat.registerCmd('tune', (player) => {
                var veh = player.vehicle;
                veh.modKit = 1;
                
                var i;
                for (i = 0; i < 49; i++) {
                    var bestMod = veh.getModsCount(i);
                    if (bestMod) veh.setMod(i, bestMod);
                }
            });
                rpVehicle.GetModsCount(1);
            */
            Dictionary<int, int> mods = new Dictionary<int, int>();
            RPVehicle rpVehicle = _vehicleHandler.GetClosestRpVehicle(rpPlayer.Position, 3);
            if (rpVehicle != null)
            {
                for (int i = 0; i < 49; i++)
                {
                    int max_mod = rpVehicle.GetModsCount((byte) i);
                    if (max_mod != 0)
                    {
                        mods.Add(i, max_mod);
                    }
                }
                rpPlayer.Emit("ShowIF", "Tuning", new TuningMenuWriter(mods));


                return Tune(rpPlayer, rpVehicle);
            }

            return false;
        }
    }
}

using System;
using System.Linq;
using AltV.Net.Async;
using GangRP_Server.Core;
using AltV.Net;
using AltV.Net.Elements.Entities;
using GangRP_Server.Models;
using RageMP_Server.Helpers;
using Vehicle = GangRP_Server.Models.Vehicle;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server
{
    public class Resource : AsyncResource
    {
        private IGamemode _gamemode;

        public override void OnStart()
        {
            using var containerHelper = new ContainerHelper();
            containerHelper.RegisterTypes();
            containerHelper.ResolveTypes();

            _gamemode = containerHelper.Resolve<IGamemode>();
            _gamemode.Start();
        }

        public override async void OnStop()
        {
            await _gamemode.Stop();
        }

        public override IEntityFactory<IPlayer> GetPlayerFactory()
        {
            return new RPPlayerFactory();
        }

        public override IEntityFactory<IVehicle> GetVehicleFactory()
        {
            return new RPVehicleFactory();
        }
    }
}

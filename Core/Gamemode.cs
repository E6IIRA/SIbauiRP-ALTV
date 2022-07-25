using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.EntitySync;
using AltV.Net.EntitySync.ServerEvent;
using AltV.Net.EntitySync.SpatialPartitions;
using GangRP_Server.Extensions;
using GangRP_Server.Handlers.Event;
using GangRP_Server.Handlers.Player;
using GangRP_Server.Handlers.Timer;
using GangRP_Server.Handlers.Vehicle;
using GangRP_Server.Models;
using GangRP_Server.Utilities;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Core
{
    public class Gamemode : IGamemode
    {
        private readonly RPContext _rpContext;
        private readonly IEventHandler _eventHandler;
        private readonly IVehicleHandler _vehicleHandler;
        private readonly IPlayerHandler _playerHandler;
        private readonly ITimerHandler _timerHandler;

        public Gamemode(RPContext rpContext, IEventHandler eventHandler, IVehicleHandler vehicleHandler, IPlayerHandler playerHandler, ITimerHandler timerHandler)
        {
            _rpContext = rpContext;
            _eventHandler = eventHandler;
            _vehicleHandler = vehicleHandler;
            _playerHandler = playerHandler;
            _timerHandler = timerHandler;
        }

        public void Start()
        {
            if (TestDatabaseOffline())
            {
                Alt.Log("[DATABASE] Connection failed!");
                Alt.Server.StopResource(Alt.Core.Resource.Name);
                return;
            }

#if DEBUG
            AltEntitySync.Init(3, 250,
                (threadCount, repository) => new ServerEventNetworkLayer(threadCount, repository),
                (entity, threadCount) => entity.Type,
                (entityId, entityType, threadCount) => entityType,
                (threadId) =>
                {
                    //THREAD TEXT/MARKER
                    if (threadId == 1 || threadId == 0)
                    {
                        return new LimitedGrid3(50_000, 50_000, 75, 10_000, 10_000, 350);
                    }
                    //THREAD OBJECT
                    else if (threadId == 2)
                    {
                        return new LimitedGrid3(50_000, 50_000, 125, 10_000, 10_000, 1000);
                    }
                    /*//THREAD PED
                    else if (threadId == 3){
                         return new LimitedGrid3(50_000, 50_000, 175, 10_000, 10_000, 64);
                    }*/
                    else
                    {
                        return new LimitedGrid3(50_000, 50_000, 175, 10_000, 10_000, 300);
                    }
                },
                new IdProvider());
#endif

            _eventHandler.LoadHandlers();

            //Set all players offline
            _rpContext.Player.ForEach(d => d.IsOnline = false);
            _rpContext.SaveChangesAsync();
        }

        public async Task Stop()
        {
            /*
             * VEHICLE POSITION SAVING
             * Extract to Method
             */
            _timerHandler.StopAllIntervals();
            await Alt.GetAllPlayers().ForEach(p => p.Kick("Server wird gestoppt"));
            //await _playerHandler.SaveAllPlayersToDb(); //kick players?
            await _vehicleHandler.SaveAllVehiclesToDb();
            PropStreamer.DestroyAllDynamicObjects();
        }

        private bool TestDatabaseOffline()
        {
            bool offline = false;

            try
            {
                _rpContext.Database.CanConnect();
            }
            catch (InvalidOperationException)
            {
                offline = true;
            }

            return offline;
        }
    }
}

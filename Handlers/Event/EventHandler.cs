using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using GangRP_Server.Core;
using GangRP_Server.Events;
using GangRP_Server.Extensions;
using GangRP_Server.Handlers.Timer;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Handlers.Event
{
    public class EventHandler : IEventHandler
    {
        private readonly ITimerHandler _timerHandler;
        private readonly IEnumerable<ILoadEvent> _loadEvents;
        private readonly IEnumerable<IPressedEEvent> _pressedEEvents;
        private readonly IEnumerable<IPressedLEvent> _pressedLEvents;
        private readonly IEnumerable<IPressedMEvent> _pressedMEvents;
        private readonly IEnumerable<IPressedIEvent> _pressedIEvents;
        private readonly IEnumerable<IPressedKEvent> _pressedKEvents;
        private readonly IEnumerable<IPlayerDeadEvent> _playerDeadEvents;
        private readonly IEnumerable<IPlayerConnectEvent> _playerConnectedEvents;
        private readonly IEnumerable<IPlayerDisconnectEvent> _playerDisconnectedEvents;
        private readonly IEnumerable<IConsoleCommandEvent> _consoleCommandEvents;
        private readonly IEnumerable<IEntityColshapeHitEvent> _entityColshapeHitEvents;
        private readonly IEnumerable<IMinuteUpdateEvent> _minuteUpdateEvents;
        private readonly IEnumerable<IFiveteenMinuteUpdateEvent> _fiveteenMinuteUpdateEvents;
        private readonly IEnumerable<IFiveSecondsUpdateEvent> _fiveSecondsUpdateEvents;
        private readonly IEnumerable<IPlayerEnterVehicleEvent> _playerEnterVehicleEvents;
        private readonly IEnumerable<IPlayerLeaveVehicleEvent> _playerLeaveVehicleEvents;

        public EventHandler(ITimerHandler timerHandler,
                            IEnumerable<ILoadEvent> loadEvents,
                            IEnumerable<IPressedEEvent> pressedEEvents,
                            IEnumerable<IPressedLEvent> pressedLEvents,
                            IEnumerable<IPressedMEvent> pressedMEvents,
                            IEnumerable<IPressedIEvent> pressedIEvents,
                            IEnumerable<IPressedKEvent> pressedKEvents,
                            IEnumerable<IPlayerDeadEvent> playerDeadEvents,
                            IEnumerable<IPlayerConnectEvent> playerConnectedEvents,
                            IEnumerable<IPlayerDisconnectEvent> playerDisconnectEvents,
                            IEnumerable<IConsoleCommandEvent> consoleCommandEvents,
                            IEnumerable<IEntityColshapeHitEvent> entityColshapeHitEvents,
                            IEnumerable<IMinuteUpdateEvent> minuteUpdateEvents,
                            IEnumerable<IFiveteenMinuteUpdateEvent> fiveteenMinuteUpdateEvents,
                            IEnumerable<IFiveSecondsUpdateEvent> fiveSecondsUpdateEvents,
                            IEnumerable<IPlayerEnterVehicleEvent> playerEnterVehicleEvents,
                            IEnumerable<IPlayerLeaveVehicleEvent> playerLeaveVehicleEvents)
        {
            AltAsync.OnClient<IPlayer>("PressE", OnPressE);
            AltAsync.OnClient<IPlayer>("PressL", OnPressL); 
            AltAsync.OnClient<IPlayer>("PressM", OnPressM);
            AltAsync.OnClient<IPlayer>("PressI", OnPressI);
            AltAsync.OnClient<IPlayer>("PressK", OnPressK);
            _timerHandler = timerHandler;
            _loadEvents = loadEvents;
            _pressedEEvents = pressedEEvents;
            _pressedLEvents = pressedLEvents;
            _pressedMEvents = pressedMEvents;
            _pressedIEvents = pressedIEvents;
            _pressedKEvents = pressedKEvents;
            _playerDeadEvents = playerDeadEvents;
            _playerConnectedEvents = playerConnectedEvents;
            _playerDisconnectedEvents = playerDisconnectEvents;
            _consoleCommandEvents = consoleCommandEvents;
            _entityColshapeHitEvents = entityColshapeHitEvents;
            _fiveteenMinuteUpdateEvents = fiveteenMinuteUpdateEvents;
            _minuteUpdateEvents = minuteUpdateEvents;
            _fiveSecondsUpdateEvents = fiveSecondsUpdateEvents;
            _playerEnterVehicleEvents = playerEnterVehicleEvents;
            _playerLeaveVehicleEvents = playerLeaveVehicleEvents;
        }

        public Task LoadHandlers()
        {
            _loadEvents.ForEach(e => e.OnLoad());

            AltAsync.OnPlayerDead += (IPlayer player, IEntity killer, uint weapon) =>
                _playerDeadEvents.ForEach(e => e.OnPlayerDead(player, killer, weapon));

            AltAsync.OnPlayerConnect += (IPlayer player, string reason) =>
                        _playerConnectedEvents.ForEach(e => e.OnPlayerConnect(player, reason));

            AltAsync.OnPlayerDisconnect += (IPlayer player, string reason) =>
                _playerDisconnectedEvents.ForEach(e => e.OnPlayerDisconnect(player, reason));

            AltAsync.OnConsoleCommand += (string name, string[] args) =>
                        _consoleCommandEvents.ForEach(e => e.OnConsoleCommand(name, args));

            AltAsync.OnColShape += (IColShape colshape, IEntity entity, bool state) =>
                _entityColshapeHitEvents.ForEach(e => e.OnEntityColshapeHit(colshape, entity, state));

            _timerHandler.AddInterval(1000 * 60, (s, e) =>
                _minuteUpdateEvents.ForEach(e => e.OnMinuteUpdate()));

            _timerHandler.AddInterval(1000 * 15 * 60, (s, e) =>
                _fiveteenMinuteUpdateEvents.ForEach(e => e.OnFiveteenMinuteUpdate()));

            _timerHandler.AddInterval(1000 * 5, (s, e) =>
                _fiveSecondsUpdateEvents.ForEach(e => e.OnFiveSecondsUpdate()));

            AltAsync.OnPlayerEnterVehicle += (IVehicle vehicle, IPlayer player, byte seat) =>
                _playerEnterVehicleEvents.ForEach(e => e.OnPlayerEnterVehicle(vehicle, player,  Convert.ToSByte(seat-2)));

            AltAsync.OnPlayerLeaveVehicle += (IVehicle vehicle, IPlayer player, byte seat) =>
                _playerLeaveVehicleEvents.ForEach(e => e.OnPlayerLeaveVehicle(vehicle, player, Convert.ToSByte(seat - 2)));

            return Task.CompletedTask;
        }

        private async void OnPressE(IPlayer player)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            if (!rpPlayer.CanInteract()) return;
            if (rpPlayer.CancelTask()) return;
            await _pressedEEvents.ForEach(async e =>
            {
                if (await e.OnPressedE(player)) return;
            });
        }

        private void OnPressL(IPlayer player)
        {
            RPPlayer rpPlayer = (RPPlayer)player;
            if (!rpPlayer.CanInteract()) return;
            _pressedLEvents.ForEach(e =>
            {
                if (e.OnPressedL(player)) return;
            });
        }

        private void OnPressM(IPlayer player)
        {
            RPPlayer rpPlayer = (RPPlayer)player;
            if (!rpPlayer.CanInteract()) return;
            _pressedMEvents.ForEach(e =>
            {
                if (e.OnPressedM(player)) return;
            });
        }
        private async void OnPressI(IPlayer player)
        {
            RPPlayer rpPlayer = (RPPlayer)player;
            if (!rpPlayer.CanInteract()) return;
            await _pressedIEvents.ForEach(async e =>
            {
                if (await e.OnPressedI(player)) return;
            });
        }
        private void OnPressK(IPlayer player)
        {
            RPPlayer rpPlayer = (RPPlayer)player;
            if (!rpPlayer.CanInteract()) return;
            _pressedKEvents.ForEach(e =>
            {
                if (e.OnPressedK(player)) return;
            });
        }
    }
}

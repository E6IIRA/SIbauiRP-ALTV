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
namespace GangRP_Server.Modules.Door
{

    public class Door
    {
        public DoorData DoorData { get; set; }
        public IColShape ColShape { get;set; }
        public HashSet<int> TeamHashSet { get; set; }

        public Door(DoorData doorData, IColShape colShape)
        {
            this.TeamHashSet = new HashSet<int>();
            this.DoorData = doorData;
            this.ColShape = colShape;
        }
    }


    public sealed class DoorModule : ModuleBase, IEntityColshapeHitEvent, IPressedLEvent, ILoadEvent
    {
        private readonly ILogger _logger;
        private readonly RPContext _rpContext;
        private readonly IPlayerHandler _playerHandler;

        public Dictionary<int, Door> _doors;





        public DoorModule(ILogger logger, RPContext rpContext, IPlayerHandler playerHandler)
        {
            _logger = logger;
            _rpContext = rpContext;
            _playerHandler = playerHandler;
            _doors = new Dictionary<int, Door>();

        }

        public void OnLoad()
        {
            foreach (var doorData in AddTableLoadEvent<DoorData>(_rpContext.DoorData))
            {
                IColShape colShape = Alt.CreateColShapeSphere(doorData.Position, doorData.Range);
                colShape.SetData("doorId", doorData.Id);
                Door door = new Door(doorData, colShape);

                if (!String.IsNullOrWhiteSpace(doorData.Teams))
                {
                    try
                    {
                        int[] myInts = Array.ConvertAll(doorData.Teams.Split(";"), s => int.Parse(s));
                        door.TeamHashSet = new HashSet<int>(myInts);
                    }
                    catch (Exception e)
                    {
                        _logger.Error($"Error Loading DoorData with ID {doorData.Id} REASON: Invalid Teams");
                        return;
                    }
                } 
                _doors.Add(doorData.Id, door);

#if DEBUG
                var label = TextLabelStreamer.Create($"Door Id: {doorData.Id}", doorData.Position, 0, true, new Rgba(255, 255, 255, 255));
#endif

            }
        }


        public void ChangeDoorLockState(DoorData doorData)
        {
            doorData.Locked = !doorData.Locked;
            IEnumerable<RPPlayer> rpPlayers = _playerHandler.GetRpPlayers().Where(d => d.ColShapes.Count != 0);
            foreach (var rpPlayer in rpPlayers)
            {
                foreach (var colShape in rpPlayer.ColShapes)
                {
                    if (colShape.GetData("doorId", out int doorId))
                    {
                        if (doorId == doorData.Id)
                        {
                            rpPlayer.Emit("SetStateOfDoor", doorData.ModelHash, doorData.Locked, doorData.PositionX, doorData.PositionY, doorData.PositionZ);
                            break;
                        }
                    }
                }
            }
        }



        public void OnEntityColshapeHit(IColShape shape, IEntity entity, bool state)
        {
            if (shape.GetData("doorId", out int doorId))
            {
                if (_doors.TryGetValue(doorId, out Door door))
                {
                    if (entity is IPlayer player)
                    {
                        _logger.Info($"doordID {doorId} state {state}");
                        if (state)
                        {
                            DoorData doorData = door.DoorData;
                            player.Emit("SetStateOfDoor", doorData.ModelHash, doorData.Locked, doorData.PositionX, doorData.PositionY, doorData.PositionZ);
                            RPPlayer rpPlayer = (RPPlayer)player;
                            rpPlayer.ColShapes.Add(shape);
                        }
                        else
                        {
                            RPPlayer rpPlayer = (RPPlayer)player;
                            if (rpPlayer.ColShapes.Contains(shape)) rpPlayer.ColShapes.Remove(shape);

                        }
                    }
                }
            }
        }

        public bool OnPressedL(IPlayer player)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            foreach (var colShape in rpPlayer.ColShapes)
            {
                if (colShape.GetData("doorId", out int doorId))
                {
                    if (_doors.TryGetValue(doorId, out Door door))
                    {
                        if (!rpPlayer.CanControlDoor(door)) return false;
                        DoorData doorData = door.DoorData;
                        //Check if Door is broken atm
                        if (doorData.LastBreak.AddMinutes(5) > DateTime.Now) return false;


                        if (!doorData.Locked)
                            rpPlayer.SendNotification($"Tür abgeschlossen", RPPlayer.NotificationType.ERROR, $"({doorData.Id}) - {doorData.Name}");
                        else rpPlayer.SendNotification($"Tür aufgeschlossen", RPPlayer.NotificationType.SUCCESS, $"({doorData.Id}) - {doorData.Name}");
                        ChangeDoorLockState(doorData);
                        return true;
                    }
                }
            }
            return false;
        }
    }
}

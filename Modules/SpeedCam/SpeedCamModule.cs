using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Elements.Entities;
using GangRP_Server.Core;
using GangRP_Server.Events;
using GangRP_Server.Handlers.Logger;
using GangRP_Server.Handlers.Player;
using GangRP_Server.Handlers.Vehicle;
using GangRP_Server.Models;
using GangRP_Server.Modules.VehicleData;
using GangRP_Server.Utilities.SpeedCam;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Modules.SpeedCam
{
    public sealed class SpeedCamModule : ModuleBase, ILoadEvent, IEntityColshapeHitEvent, IPressedEEvent
    {
        private readonly IVehicleHandler _vehicleHandler;
        private readonly IPlayerHandler _playerHandler;
        //private readonly VehicleDataModule _vehicleDataModule;

        

        public Dictionary<int, SpeedCam> _speedCams;
        public int _speedCamId;

        public SpeedCamModule(/*VehicleDataModule vehiceDataModule, */IVehicleHandler vehicleHandler, IPlayerHandler playerHandler)
        {
            //_vehicleDataModule = vehiceDataModule;
            _vehicleHandler = vehicleHandler;
            _playerHandler = playerHandler;
        }


        public void OnLoad()
        {
            _speedCamId = 1;
            _speedCams = new Dictionary<int, SpeedCam>();
            AddClientEvent<int, int>("SendVehicleSpeed", SendVehicleSpeed);
            AddClientEvent<int>("RemoveFromSpeedCam", RemoveFromSpeedCam);
        }

        public bool CreateSpeedCamStation(IPlayer player)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            if (!rpPlayer.DutyStatus) return false;
            RPVehicle rpVehicle = _vehicleHandler.GetClosestTeamRpVehicle(rpPlayer.Position, rpPlayer.TeamId);
            if (rpVehicle == null) return false;
            IColShape colShape = Alt.CreateColShapeSphere(player.Position, 30.0f);
            colShape.SetData("speedCamId", _speedCamId);
            SpeedCam speedCam = new SpeedCam(_speedCamId, colShape, rpPlayer, rpVehicle);
            _speedCams.Add(_speedCamId, speedCam);
            _speedCamId++;
            return true;
        }


        public void DeleteSpeedCamStation(/*IPlayer player, */int speedCamId)
        {
            if (_speedCams.TryGetValue(speedCamId, out SpeedCam speedCam))
            {
                speedCam.colShape.Remove();
                _speedCams.Remove(speedCamId);
            }
        }

        public void OnEntityColshapeHit(IColShape shape, IEntity entity, bool state)
        {
            if (!state) return;
            if (shape.GetData("speedCamId", out int speedCamId))
            {
                if (entity is IVehicle vehicle)
                {
                    if (vehicle.Driver == null) return;
                    RPVehicle rpVehicle = (RPVehicle) vehicle;
                    if (_speedCams.ContainsKey(speedCamId))
                    {
                        rpVehicle.Driver.Emit("GetVehicleSpeed", speedCamId);
                    }
                }
            }
        }

        public void SendVehicleSpeed(IPlayer player, int camId, int speed)
        {
            if (!player.IsInVehicle) return;
            if (_speedCams.TryGetValue(camId, out SpeedCam speedCam))
            {
                RPVehicle rpVehicle = (RPVehicle) player.Vehicle;
                SpeedCamItem speedCamItem = new SpeedCamItem(speed, rpVehicle.NumberplateText);
                speedCam.SpeedCamItems.Add(speedCamItem);
                foreach (var playerId in speedCam.camStationPlayers)
                {
                    RPPlayer camPlayer = _playerHandler.GetOnlineRPPlayerByPlayerId(playerId);
                    if (camPlayer == null) break;
                    //camPlayer.SendNotification($"{_vehicleDataModule.GetVehicleDataById(rpVehicle.VehicleDataId).Name } - {speed}/Kmh", RPPlayer.NotificationType.INFO, 10000);

                    camPlayer.Emit("UpdateView", "SendSpeedCamItem", new SpeedCamItemAddWriter(speedCamItem));


                }

                


                RPPlayer rpPlayer = (RPPlayer) player;
                rpPlayer.SendNotification($"Du wurdest mit {speed}/Kmh geblitzt", RPPlayer.NotificationType.INFO, "Geblitzt amk", 10000);
            }
        }

        public void RemoveFromSpeedCam(IPlayer player, int camId)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            if (_speedCams.TryGetValue(camId, out SpeedCam speedCam))
            {
                RemovePlayerFromSpeedCam(rpPlayer, speedCam);
            }
            
        }

        public void AddPlayerToSpeedCam(RPPlayer rpPlayer, SpeedCam speedCam)
        {
            if (!speedCam.camStationPlayers.Contains(rpPlayer.PlayerId))
            {
                speedCam.camStationPlayers.Add(rpPlayer.PlayerId);

                rpPlayer.Emit("ShowIF", "SpeedCamStation", new SpeedCamItemWriter(speedCam));

            }
        }

        public void RemovePlayerFromSpeedCam(RPPlayer rpPlayer, SpeedCam speedCam)
        {
            if (speedCam.camStationPlayers.Contains(rpPlayer.PlayerId))
            { 
                speedCam.camStationPlayers.Remove(rpPlayer.PlayerId);
            }
        }

        public Task<bool> OnPressedE(IPlayer player)
        {
            RPPlayer rpPlayer = (RPPlayer) player;

            if (_speedCamId == 1)
            {
                if (CreateSpeedCamStation(player))
                {
                    rpPlayer.SendNotification("SpeedCamStation erstellt amk", RPPlayer.NotificationType.SUCCESS);
                    return Task.FromResult(true);
                }
            }
            else
            {
                RPVehicle closestTeamRpVehicle = _vehicleHandler.GetClosestTeamRpVehicle(player.Position, rpPlayer.TeamId); //TODO player has to be in an vehicle?

                if (closestTeamRpVehicle == null) return Task.FromResult(false);

                SpeedCam speedCam = _speedCams.Values.FirstOrDefault(d => d.camStationVehicle.VehicleId == closestTeamRpVehicle.VehicleId);

                if (speedCam == null) return Task.FromResult(false);

                AddPlayerToSpeedCam(rpPlayer, speedCam);
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltV.Net.Elements.Entities;
using GangRP_Server.Core;
using GangRP_Server.Events;
using GangRP_Server.Handlers.Logger;
using GangRP_Server.Handlers.Player;
using GangRP_Server.Handlers.Vehicle;
using GangRP_Server.Models;
using GangRP_Server.Modules.Fuelstation;
using GangRP_Server.Modules.Garage;
using GangRP_Server.Modules.Injury;
using GangRP_Server.Modules.Player;
using GangRP_Server.Modules.VehicleData;
using GangRP_Server.Utilities.Crime;
using GangRP_Server.Utilities.FuelStation;
using GangRP_Server.Utilities.InteractionMenu;
using Newtonsoft.Json;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Modules.InteractionMenu
{
    public sealed class InteractionMenuModule : ModuleBase
    {

        private readonly ILogger _logger;
        private readonly VehicleDataModule _vehicleDataModule;
        private readonly IVehicleHandler _vehicleHandler;
        private readonly FuelstationDataModule _fuelstationDataModule;

        public InteractionMenuModule(ILogger logger, VehicleDataModule vehicleDataModule, FuelstationDataModule fuelstationDataModule, IVehicleHandler vehicleHandler)
        {
            _logger = logger;
            _vehicleDataModule = vehicleDataModule;
            _vehicleHandler = vehicleHandler;
            _fuelstationDataModule = fuelstationDataModule;
            AddClientEvent<int>("ShowLicence", ShowLicence);
            AddClientEvent<IVehicle>("ToggleEngine", ToggleEngine);
            AddClientEvent<IVehicle>("ToggleTrunk", ToggleTrunk);
            AddClientEvent<IVehicle>("ToggleDoor", ToggleDoor);
            AddClientEvent("PolComputer", PolComputer);
            AddClientEvent<IVehicle>("Park", Park);
            AddClientEvent<IPlayer>("FirstAid", FirstAid);
            AddClientEvent<IPlayer>("GrabPlayer", GrabPlayer);
            AddClientEvent<IPlayer>("Cuff", TiePlayer);
            AddClientEvent<IPlayer>("Tie", TiePlayer);
            AddClientEvent<IVehicle>("Refuel", Refuel);
        }

        private async void TiePlayer(IPlayer player, IPlayer victim)
        {
            RPPlayer rpPlayer = (RPPlayer)player;
            RPPlayer rpVictim = (RPPlayer)victim;

            if (rpVictim.IsCuffed || !rpVictim.IsAlive()) return;

            if (rpVictim.IsTied)
            {
                rpVictim.StopAnimation(true);
                rpVictim.IsTied = false;
                await rpPlayer.Inventory.AddItem(17); //Fesseln
            }
            else
            {
                if (rpPlayer.Inventory.HasItem(17)) //Fesseln
                {
                    rpVictim.PlayAnimation(Animation.HANDCUFFED);
                    rpVictim.IsTied = true;
                    await rpPlayer.Inventory.RemoveItem(17);
                }
            }
        }

        void ToggleEngine(IPlayer player, IVehicle vehicle)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            if (!player.IsInVehicle) return;
            RPVehicle rpVehicle = (RPVehicle)vehicle;
            if (!rpPlayer.CanControlVehicle(rpVehicle)) return;
            if (rpVehicle.Engine)
                rpPlayer.SendNotification($"Motor ausgeschalten", RPPlayer.NotificationType.ERROR, $"({rpVehicle.VehicleId}) - {_vehicleDataModule.GetVehicleDataById(rpVehicle.VehicleDataId).Name}");
            else rpPlayer.SendNotification($"Motor gestartet", RPPlayer.NotificationType.SUCCESS, $"({rpVehicle.VehicleId}) - {_vehicleDataModule.GetVehicleDataById(rpVehicle.VehicleDataId).Name}");
            rpVehicle.Engine = !rpVehicle.Engine;
            return;
        }

        void ToggleTrunk(IPlayer player, IVehicle vehicle)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            RPVehicle rpVehicle = (RPVehicle) vehicle;

            if (rpVehicle == null) return;

            if (!rpVehicle.Locked)
            {
                if (rpVehicle.TrunkStatus)
                    rpPlayer.SendNotification($"Kofferraum geschlossen", RPPlayer.NotificationType.ERROR, $"({rpVehicle.VehicleId}) - {_vehicleDataModule.GetVehicleDataById(rpVehicle.VehicleDataId).Name}");
                else rpPlayer.SendNotification($"Kofferraum geöffnet", RPPlayer.NotificationType.SUCCESS, $"({rpVehicle.VehicleId}) - {_vehicleDataModule.GetVehicleDataById(rpVehicle.VehicleDataId).Name}");

                rpVehicle.TrunkStatus = !rpVehicle.TrunkStatus;
                return;
            }
        }

        void ToggleDoor(IPlayer player, IVehicle vehicle)
        {
            RPPlayer rpPlayer = (RPPlayer)player;
            RPVehicle rpVehicle = (RPVehicle) vehicle;

            if (rpPlayer.CanControlVehicle(rpVehicle))
            {
                //Spieler darf Fahrzeug bedienen

                if (rpVehicle.Locked)
                    rpPlayer.SendNotification($"Fahrzeug aufgeschlossen", RPPlayer.NotificationType.SUCCESS, $"({rpVehicle.VehicleId}) - {_vehicleDataModule.GetVehicleDataById(rpVehicle.VehicleDataId).Name}");
                else rpPlayer.SendNotification($"Fahrzeug abgeschlossen", RPPlayer.NotificationType.ERROR, $"({rpVehicle.VehicleId}) - {_vehicleDataModule.GetVehicleDataById(rpVehicle.VehicleDataId).Name}");
                rpVehicle.Locked = !rpVehicle.Locked;
            }
        }

        void PolComputer(IPlayer player)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            if (!rpPlayer.DutyStatus) return;
            player.Emit("ShowIF", "PoliceComputer");
        }

        void Park(IPlayer player, IVehicle vehicle)
        {
            GarageModule.Instance.ParkInVehicle(player, vehicle);
        }


        void ShowLicence(IPlayer player, int licenceId)
        {
            _logger.Debug("ShowLicence -> " + licenceId);
            player.Emit("UpdateView", "DisplayLicence", player.Name, licenceId, 5000);

        }

        void FirstAid(IPlayer player, IPlayer targetPlayer)
        {
            InjuryModule.Instance.OpenFirstAidWindow((RPPlayer)player, (RPPlayer)targetPlayer);
        }

        void GrabPlayer(IPlayer player, IPlayer targetPlayer)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            RPPlayer targetRpPlayer = (RPPlayer) targetPlayer;


            if (!targetRpPlayer.CanInteract())
            {
                RPVehicle closestRpVehicle = _vehicleHandler.GetClosestRpVehicle(rpPlayer.Position, 5);

                if (closestRpVehicle != null)
                {
                    if (!closestRpVehicle.Locked)
                    {
                        if (closestRpVehicle.TryPutPlayerIntoVehicle(targetRpPlayer))
                        {
                            rpPlayer.SendNotification("Du hast jemanden ins Fahrzeug gesteckt", RPPlayer.NotificationType.SUCCESS, "Fahrzeug");
                        }
                    }
                }

            }
        }

        void Refuel(IPlayer player, IVehicle vehicle)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            RPVehicle rpVehicle = (RPVehicle) vehicle;

            if (rpPlayer.GetData("fuelstationId", out int fuelstationId))
            {
                FuelstationData fuelstationData = _fuelstationDataModule.GetFuelstationDataById(fuelstationId);
                if (fuelstationData != null)
                {
                    FuelstationGaspumpData gaspumpData = fuelstationData.FuelstationGaspumpData.FirstOrDefault(d => d.Position.Distance(rpPlayer.Position) < 3);
                    if (gaspumpData != null)
                    {
                        Models.VehicleData vehicleData = _vehicleDataModule.GetVehicleDataById(rpVehicle.VehicleDataId);
                        int toBeFueled = (int) (vehicleData.MaxFuel - rpVehicle.Fuel);
                        if (toBeFueled >= 1)
                        {
                            rpPlayer.Emit("ShowIF", "FuelStation", new FuelStationWriter(fuelstationData.Id, fuelstationData.Name, gaspumpData.Id, rpVehicle.VehicleId, toBeFueled, fuelstationData.Price));
                        }
                        else
                        {
                            rpPlayer.SendNotification("Das Fahrzeug ist bereits voll getankt.", RPPlayer.NotificationType.ERROR, $"{fuelstationData.Name}");
                        }
                    }
                }
            }
        }

        //public bool OnPressedE(IPlayer player)
        //{
            //RPPlayer rpPlayer = (RPPlayer) player;
            //player.Emit("ShowIF", "LicenceOverview", new LicenceWriter(rpPlayer.Licences));
            //return false;
        //}
    }
}

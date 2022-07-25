using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using GangRP_Server.Core;
using GangRP_Server.Events;
using GangRP_Server.Extensions;
using GangRP_Server.Handlers.Inventory;
using GangRP_Server.Handlers.Logger;
using GangRP_Server.Handlers.Player;
using GangRP_Server.Handlers.Vehicle;
using GangRP_Server.Models;
using GangRP_Server.Modules.Inventory;
using GangRP_Server.Modules.VehicleData;
using GangRP_Server.Utilities;
using GangRP_Server.Utilities.Drug;
using Microsoft.EntityFrameworkCore;
using Vehicle = GangRP_Server.Models.Vehicle;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Modules.Drug
{
    public sealed class DrugCamperModule : ModuleBase, IPressedEEvent, IPressedLEvent, IFiveteenMinuteUpdateEvent, IPlayerDisconnectEvent, ILoadEvent
    {
        private readonly ILogger _logger;
        private readonly RPContext _rpContext;

        private readonly IVehicleHandler _vehicleHandler;

        private readonly IInventoryHandler _inventoryHandler;

        private readonly Position _offsetPosition = new Position(0.0f, 0.0f, 0.0f);

        public Dictionary<RPPlayer, DrugCamper> ProcessingPlayers = new Dictionary<RPPlayer, DrugCamper>();

        public Dictionary<int, DrugCamper> ProcessingVehicles = new Dictionary<int, DrugCamper>();
        
        private Dictionary<float, float> _tempParameterValues = new Dictionary<float, float>();
        private Dictionary<float, float> _humParameterValues = new Dictionary<float, float>();
        private Dictionary<int, float> _ventParameterValues = new Dictionary<int, float>();
        private Dictionary<float, float> _dildParameterValues = new Dictionary<float, float>();
            
        public DrugCamperModule(ILogger logger, RPContext rpContext, IVehicleHandler vehicleHandler, IInventoryHandler inventoryHandler)
        {
            _logger = logger;
            _rpContext = rpContext;
            _vehicleHandler = vehicleHandler;
            _inventoryHandler = inventoryHandler;
        }

        public void OnLoad()
        {
            ProcessingVehicles = AddTableLoadEvent<DrugCamper>(_rpContext.DrugCamper.Include(d => d.DrugCamperTypeData).ThenInclude(d => d.DrugCamperTypeItemData), OnItemLoad)
                .ToDictionary(p => p.VehicleId);
            AddClientEvent("StartCamperProcess", StartProcess);
            AddClientEvent("StopCamperProcess", StopProcess);
            AddClientEvent<int, int, int, int>("SaveCamperParameter", SaveCamperParameter);
            LoadParameter("Temperatur", _tempParameterValues);
            LoadParameter("Luftfeuchtigkeit", _humParameterValues);
            LoadParameter("Lüftung", _ventParameterValues);
            LoadParameter("Dildo", _dildParameterValues);
        }

        public void OnItemLoad(DrugCamper drugCamper)
        {
            foreach (var drugCamperTypeItemData in drugCamper.DrugCamperTypeData.DrugCamperTypeItemData)
            {
                if (drugCamperTypeItemData.IsInput == 1)
                {
                    drugCamper.NeededItems.Add(drugCamperTypeItemData.ItemDataId, drugCamperTypeItemData.Amount);
                }
                else
                {
                    drugCamper.OutputItems.Add(drugCamperTypeItemData.ItemDataId, (drugCamperTypeItemData.Amount, null));
                }
            }
            CalculateQuality(drugCamper, drugCamper.TeamDataId);
        }

        public void LoadParameter(string name, Dictionary<float, float> ParameterValues)
        {
            StreamReader reader = new StreamReader(File.OpenRead(name + ".csv"));
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (!String.IsNullOrWhiteSpace(line))
                {
                    string[] values = line.Split(',');
                    if (values.Length >= 2 && values[0].Length > 0 && values[1].Length > 0)
                    {
                        ParameterValues.Add(float.Parse(values[0], CultureInfo.InvariantCulture.NumberFormat), float.Parse(values[1], CultureInfo.InvariantCulture.NumberFormat));
                    }
                }
            }
        }

        public void LoadParameter(string name, Dictionary<int, float> ParameterValues)
        {
            StreamReader reader = new StreamReader(File.OpenRead(name + ".csv"));
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (!String.IsNullOrWhiteSpace(line))
                {
                    string[] values = line.Split(',');
                    if (values.Length >= 2 && values[0].Length > 0 && values[1].Length > 0)
                    {
                        ParameterValues.Add(int.Parse(values[0], CultureInfo.InvariantCulture.NumberFormat), float.Parse(values[1], CultureInfo.InvariantCulture.NumberFormat));
                    }
                }
            }
        }

        public bool OnPressedL(IPlayer player)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            if (rpPlayer.DimensionType == DimensionType.CAMPER && rpPlayer.Position.Distance(new Position(1972.96f, 3816.33f, 33.5f)) < 2.0f)
            {
                if (rpPlayer.GetData("camperVehicleId", out int camperVehicleId))
                {
                    RPVehicle? rpVehicle = _vehicleHandler.GetRpVehicle(camperVehicleId);
                    if (rpVehicle != null &&rpPlayer.CanControlVehicle(rpVehicle))
                    {
                        if (rpVehicle.Locked)
                            rpPlayer.SendNotification($"Fahrzeug aufgeschlossen", RPPlayer.NotificationType.SUCCESS,
                                $"(Camper)");
                        else
                            rpPlayer.SendNotification($"Fahrzeug abgeschlossen", RPPlayer.NotificationType.ERROR,
                                $"(Camper)");
                        rpVehicle.Locked = !rpVehicle.Locked;
                        return true;
                    }
                }
            }
            return false;
        }

        public Task<bool> OnPressedE(IPlayer player)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            //Im Camper
            if (rpPlayer.DimensionType == DimensionType.CAMPER)
            {
                if (rpPlayer.Position.Distance(Positions.CamperDoorPosition) < 0.5f)
                {
                    if (rpPlayer.GetData("camperVehicleId", out int camperVehicleId))
                    {
                        RPVehicle? rpVehicle = _vehicleHandler.GetRpVehicle(camperVehicleId);
                        if (rpVehicle != null && !rpVehicle.Locked)
                        {
                            ExitVehicle(rpVehicle, rpPlayer);
                            return Task.FromResult(true);
                        }
                    }
                }
                else if (rpPlayer.Position.Distance(Positions.CamperInputPosition) < 0.5f)
                {
                    if (rpPlayer.GetData("camperVehicleId", out int camperVehicleId))
                    {
                        RPVehicle? rpVehicle = _vehicleHandler.GetRpVehicle(camperVehicleId);
                        if (rpVehicle != null && rpPlayer.TeamId == rpVehicle.TeamId)
                        {
                            if (ProcessingVehicles.TryGetValue(camperVehicleId, out DrugCamper drugCamper))
                            {
                                int state = 0;
                                if (ProcessingPlayers.Keys.Contains(rpPlayer))
                                    state = 1;
                                rpPlayer.Emit("ShowIF", "Camper", new DrugCamperWriter(state, drugCamper.Temperature, drugCamper.Humidity, drugCamper.Ventilation, drugCamper.Dildogroeße));
                                return Task.FromResult(true);
                            }
                        }
                    }
                }
            }
            //Nicht im Camper
            else if (rpPlayer.DimensionType == DimensionType.WORLD)
            {
                RPVehicle closestRpVehicle = _vehicleHandler.GetClosestRpVehicle(rpPlayer.Position, 3);
                if (closestRpVehicle != null && ProcessingVehicles.ContainsKey(closestRpVehicle.VehicleId))
                {
                    if (!closestRpVehicle.Locked && closestRpVehicle.IsEnterable())
                    {
                        EnterVehicle(closestRpVehicle, rpPlayer);
                        return Task.FromResult(true);
                    }
                }
            }
            return Task.FromResult(false);
        }

        public async void EnterVehicle(IVehicle vehicle, IPlayer player)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            RPVehicle rpVehicle = (RPVehicle) vehicle;

            rpPlayer.SavePosition(rpVehicle.Position + _offsetPosition);
            rpPlayer.Dimension = rpVehicle.VehicleId;
            rpPlayer.DimensionType = DimensionType.CAMPER;
            rpPlayer.SetPositionAsync(1972.96f, 3816.33f, 33.5f);
            rpPlayer.SetData("camperVehicleId", rpVehicle.VehicleId);
            rpPlayer.SetData("camperVehicleTeamId", rpVehicle.TeamId);
            rpPlayer.Emit("LoadCamper", rpVehicle.TeamId == rpPlayer.TeamId, ProcessingPlayers.ContainsKey(rpPlayer));

        }

        public async void ExitVehicle(RPVehicle rpVehicle, RPPlayer rpPlayer)
        {
            rpPlayer.Emit("UnloadCamper");
            rpPlayer.DimensionType = DimensionType.WORLD;
            rpPlayer.Dimension = 0;
            rpPlayer.SetPositionAsync(rpVehicle.Position + _offsetPosition + new Position(0, 0, 2f));
            rpPlayer.DeleteData("camperVehicleId");
            rpPlayer.DeleteData("camperVehicleTeamId");

            if (ProcessingPlayers.Keys.Contains(rpPlayer))
                ProcessingPlayers.Remove(rpPlayer);
        }

        public void OnFiveteenMinuteUpdate()
        {
            foreach (var kvp in ProcessingPlayers)
            {
                ProcessStep(kvp.Key, kvp.Value);
            }
        }

        public void ProcessCommand()
        {
            foreach (var kvp in ProcessingPlayers)
            {
                ProcessStep(kvp.Key, kvp.Value);
            }
        }

        public async void ProcessStep(RPPlayer rpPlayer, DrugCamper drugCamper)
        {
            if (!rpPlayer.CamperInputInventory.HasItemsAmounts(drugCamper.NeededItems))
            {
                if (rpPlayer.DimensionType == DimensionType.CAMPER)
                {
                    rpPlayer.SendNotification("Nicht alle notwendigen Materialien vorhanden!",
                        RPPlayer.NotificationType.ERROR, title: "Camperprozess");
                }

                StopProcess(rpPlayer);
                return;
            }

            RPVehicle rpVehicle = (RPVehicle) _vehicleHandler.GetRpVehicle(drugCamper.VehicleId);
            if (rpVehicle == null)
            {
                return;
            }

            if (rpVehicle.Fuel < 1)
            {
                if (rpPlayer.DimensionType == DimensionType.CAMPER)
                {
                    rpPlayer.SendNotification("Nicht genügend Treibstoff vorhanden!",
                        RPPlayer.NotificationType.ERROR, title: "Camperprozess");
                }

                StopProcess(rpPlayer);
                return;
            }
            rpVehicle.Fuel -= 1;

            drugCamper.NeededItems.ForEach(d => rpPlayer.CamperInputInventory.RemoveItem(d.Key, d.Value));
            rpPlayer.CamperOutputInventory.AddItems(drugCamper.OutputItems);
        }

        public bool CalculateQuality(DrugCamper drugCamper, int teamId)
        {
            int maxTemp = 250;
            int maxHum = 100;
            int maxDildo = 100;
            int maxVentil = 5;
            float effTemp = (float) Math.Round(Math.Abs((drugCamper.Temperature - 23.1f * teamId) % maxTemp) * 1000f) / 1000f;
            float effHum = (float) Math.Round(Math.Abs((drugCamper.Humidity - 14.2f * teamId) % maxTemp) * 1000f) / 1000f;
            float effDildo = (float) Math.Round(Math.Abs((drugCamper.Dildogroeße - 18.3f * teamId) % maxTemp) * 1000f) / 1000f;
            int effVent = Math.Abs((drugCamper.Ventilation - 3 * teamId) % maxTemp);
            if(!_tempParameterValues.TryGetValue(effTemp, out float tempQuality)) return false;
            if(!_humParameterValues.TryGetValue(effHum, out float humQuality)) return false;
            if(!_dildParameterValues.TryGetValue(effDildo, out float dildQuality)) return false;
            if(!_ventParameterValues.TryGetValue(effVent, out float ventQuality)) return false;
            float ges_quality = (tempQuality + humQuality + dildQuality + ventQuality) * 0.25f;
            Dictionary<int, (int amount, string[]? customData)> outputItems = new Dictionary<int, (int amount, string[]? customData)>();
            
            foreach (var kvp in drugCamper.OutputItems)
            {
                outputItems.Add(kvp.Key, (drugCamper.OutputItems[kvp.Key].amount, new []{ ges_quality.ToString(), "?"}));
            }

            drugCamper.OutputItems = outputItems;
            return true;
        }

        public async void StartProcess(IPlayer player)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            if (rpPlayer.GetData("camperVehicleId", out int camperVehicleId))
            {
                if (ProcessingVehicles.TryGetValue(camperVehicleId, out DrugCamper drugCamper))
                {
                    if (rpPlayer.CamperInputInventory == null)
                    {
                        rpPlayer.CamperInputInventory =
                            await _inventoryHandler.LoadInventoryTypeForPlayer(rpPlayer,
                                PlayerHandler.InventoryType.CAMPERINPUT);
                    }

                    if (rpPlayer.CamperOutputInventory == null)
                    {
                        rpPlayer.CamperOutputInventory =
                            await _inventoryHandler.LoadInventoryTypeForPlayer(rpPlayer,
                                PlayerHandler.InventoryType.CAMPEROUTPUT);
                    }

                    if (!rpPlayer.CamperInputInventory.HasItemsAmounts(drugCamper.NeededItems))
                    {
                        rpPlayer.SendNotification("Nicht alle notwendigen Materialien vorhanden!",
                            RPPlayer.NotificationType.ERROR, title: "Camperprozess");
                        return;
                    }

                    if (!rpPlayer.CamperOutputInventory.CanItemsAdded(drugCamper.NeededItems))
                    {
                        rpPlayer.SendNotification("Nicht genügend Platz für die fertigen Produkte!",
                            RPPlayer.NotificationType.ERROR, title: "Camperprozess");
                        return;
                    }

                    if (!ProcessingPlayers.Keys.Contains(rpPlayer))
                    {
                        ProcessingPlayers.Add(rpPlayer, drugCamper);
                        rpPlayer.Emit("ToggleCamperProcess", true);
                        rpPlayer.SendNotification("Prozess gestartet!", RPPlayer.NotificationType.SUCCESS,
                            title: "Camperprozess");
                    }
                }
            }
        }

        public async void StopProcess(IPlayer player)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            if (ProcessingPlayers.Keys.Contains(rpPlayer))
            {
                if (rpPlayer.DimensionType == DimensionType.CAMPER)
                {
                    rpPlayer.Emit("ToggleCamperProcess", false);
                    rpPlayer.SendNotification("Prozess gestoppt!", RPPlayer.NotificationType.ERROR,
                        title: "Camperprozess");
                }
                ProcessingPlayers.Remove(rpPlayer);
            }
        }

        public async void SaveCamperParameter(IPlayer player, int temperature, int humidity, int ventilation, int dildogroeße)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            if (rpPlayer.GetData("camperVehicleId", out int camperVehicleId))
            {
                if (ProcessingVehicles.TryGetValue(camperVehicleId, out DrugCamper drugCamper))
                {
                    drugCamper.Temperature = temperature;
                    drugCamper.Humidity = humidity;
                    drugCamper.Ventilation = ventilation;
                    drugCamper.Dildogroeße = dildogroeße;
                    await using RPContext rpContext = new RPContext();
                    rpContext.DrugCamper.Update(drugCamper);
                    await rpContext.SaveChangesAsync();
                    if(!CalculateQuality(drugCamper, rpPlayer.TeamId))
                        rpPlayer.SendNotification("FEHLER IM CAMPERSYSTEM. BITTE DER ENTWICKLUNG MELDEN!", RPPlayer.NotificationType.ERROR, title:"ERROR", duration: 30000);
                }
            }
        }

        public void OnPlayerDisconnect(IPlayer player, string reason)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            if (ProcessingPlayers.Keys.Contains(rpPlayer))
                ProcessingPlayers.Remove(rpPlayer);
        }
    }
}

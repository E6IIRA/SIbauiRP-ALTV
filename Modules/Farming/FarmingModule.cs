using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using GangRP_Server.Core;
using GangRP_Server.Events;
using GangRP_Server.Extensions;
using GangRP_Server.Handlers.Inventory;
using GangRP_Server.Handlers.Logger;
using GangRP_Server.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using GangRP_Server.Modules.Inventory;
using GangRP_Server.Utilities;
using System.Net.NetworkInformation;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Modules.Farming
{
    public sealed class FarmingModule : ModuleBase, ILoadEvent, IPressedEEvent, IFiveSecondsUpdateEvent
    {
        private readonly ILogger _logger;

        private readonly RPContext _rpContext;

        private readonly InventoryModule _inventoryModule;

        private IEnumerable<FarmFieldData> _farmFields;
        private IEnumerable<FarmFieldObjectData> _farmFieldObjects;
        private IEnumerable<FarmObjectData> _farmObjects;

        private Dictionary<RPPlayer, FarmFieldObjectData> _farmingPlayers;

        private readonly int _RespawnCounterLimit = 2; //Alle 60 (12 * 5) Sekunden soll das Respawn Event getriggert werden.
        private int _RespawnSlot = 0; //Respawnslot, der beim nächsten Triggern des Respawnevents ausgeführt werden soll.
        private readonly int _RespawnSlotLimit = 1; //Größter Slot, der erreicht werden kann. (3 bedeutet z.B. 4 Gruppen)
        private int _RespawnCounter = 0; //Wird alle 5 Sekunden erhöht
        private readonly int _minimumTimeToRespawn = 5; //in Minutes

        public FarmingModule(ILogger logger, RPContext rpContext, InventoryModule inventoryModule)
        {
            _logger = logger;
            _rpContext = rpContext;
            _inventoryModule = inventoryModule;
        }

        public void OnLoad()
        {
            _farmObjects = AddTableLoadEvent<Models.FarmObjectData>(_rpContext.FarmObjectData.Include(d => d.FarmObjectLootData));
            _farmFieldObjects = AddTableLoadEvent<Models.FarmFieldObjectData>(_rpContext.FarmFieldObjectData.Include(d => d.FarmObjectData));
            _farmFields = AddTableLoadEvent<Models.FarmFieldData>(_rpContext.FarmFieldData.Include(d => d.FarmFieldObjectData), OnItemLoad);
            _farmingPlayers = new Dictionary<RPPlayer, FarmFieldObjectData>();
        }

        private void OnItemLoad(Models.FarmFieldData farmField)
        {
            farmField.RespawnSlot = farmField.Id % _RespawnSlotLimit; //Respawnen der Steine in _RespawnSlotLimit Gruppen aufgeteilt, um Last zu verteilen
            farmField.FarmFieldObjectData.ForEach(obj => {
                if (farmField.ActiveObjects < farmField.MaximumObjects && MathUtils.RandomNumber(0,1) == 1)
                {
                    obj.Capacity = obj.FarmObjectData.Capacity;
                    obj.Active = true;
                    obj.LastFarmed = DateTime.MinValue;
                    obj.Prop = PropStreamer.Create(obj.FarmObjectData.ObjectName, obj.Position, obj.Rotation, visible: true, streamRange: 100, frozen: true);
                    farmField.ActiveObjects++;
                    //_logger.Info($"Farmobjekt erstellt! {obj.FarmObjectData.ObjectName}, Capacity {obj.Capacity} bei Position {obj.Position}, Active Objects {farmField.ActiveObjects} bei {farmField.Name}");
                }
#if DEBUG
                if(obj.Prop == null) obj.Prop = PropStreamer.Create(obj.FarmObjectData.ObjectName, obj.Position, obj.Rotation, visible: true, streamRange: 100, frozen: true);

                Rgba color = obj.Active ? new Rgba(0, 255, 0, 255) : new Rgba(255, 0, 0, 255);
                obj.PlayerLabel = TextLabelStreamer.Create($"Farm Id: {obj.Id}, Pos: {obj.Position}, Cap: {obj.Capacity}", obj.Position, color: color, streamRange: 10);

#endif
            });
        }

        public Task<bool> OnPressedE(IPlayer player)
        {
            RPPlayer rpPlayer = (RPPlayer)player;
            if(_farmingPlayers.ContainsKey(rpPlayer))
            {
                StopFarming(rpPlayer);
            }
            else {
                FarmFieldObjectData farmFieldObjectData = null;
                foreach(FarmFieldData farmField in _farmFields)
                {
                    farmFieldObjectData = farmField.FarmFieldObjectData.FirstOrDefault(obj => obj.Position.Distance(rpPlayer.Position) < 3 && obj.Active);
                    if (farmFieldObjectData != null)
                        break;
                }
                if (farmFieldObjectData == null) return Task.FromResult(false);

                if (!CanFarmProcessItemsBeAdded(rpPlayer, farmFieldObjectData)) return Task.FromResult(false);

                if (!farmFieldObjectData.Active || farmFieldObjectData.Capacity <= 0) return Task.FromResult(false);
                StartFarming(rpPlayer, farmFieldObjectData);
                _logger.Info($"{rpPlayer.Name} started farming at {farmFieldObjectData.Position}");
            }
            return Task.FromResult(true);
        }
        private void StartFarming(RPPlayer rpPlayer, FarmFieldObjectData farmFieldObjectData)
        {
            _farmingPlayers.Add(rpPlayer, farmFieldObjectData);
            //rpPlayer.PlayAnimation("placeholder");
            rpPlayer.PlayAnimation(Animation.KNEEL);
            rpPlayer.SendNotification("Farming gestartet!", RPPlayer.NotificationType.INFO, title: "Geiles Farmmodule");
        }
        private void StopFarming(RPPlayer rpPlayer)
        {
            _farmingPlayers.Remove(rpPlayer);
            //rpPlayer.StopAnimation("placeholder");
            rpPlayer.StopAnimation("missheistdockssetup1ig_3@base", "welding_base_dockworker", 1, true);
            rpPlayer.SendNotification("Farming gestoppt!", RPPlayer.NotificationType.INFO, title: "Geiles Farmmodule");
        }
        private bool CanFarmProcessItemsBeAdded(RPPlayer rpPlayer, FarmFieldObjectData farmFieldObjectData)
        {
            Dictionary<int, int> itemsToBeAdded = new Dictionary<int, int>();
            farmFieldObjectData.FarmObjectData.FarmObjectLootData.ForEach(loot => itemsToBeAdded.Add(loot.ItemDataId, loot.MaximumAmount));
            return _inventoryModule.CanItemsAdded(rpPlayer.Inventory, itemsToBeAdded);
        }
        private void FarmProcessStep(RPPlayer rpPlayer, FarmFieldObjectData farmFieldObjectData)
        {
            Dictionary<int, int> itemsToBeAdded = new Dictionary<int, int>();
            int weight = 0;
            string messageString = "";
            farmFieldObjectData.FarmObjectData.FarmObjectLootData.ForEach(loot =>
            {
                if (MathUtils.RandomChance() <= loot.Chance)
                {
                    int amount = MathUtils.RandomNumber(loot.MinimumAmount, loot.MaximumAmount);
                    itemsToBeAdded.Add(loot.ItemDataId, amount);
                    weight += amount;
                    messageString += amount + " " + loot.ItemData.Name + ", ";
                }
            });
            rpPlayer.Inventory.AddItems(itemsToBeAdded);
            farmFieldObjectData.Capacity -= weight;


            if (farmFieldObjectData.Capacity <= 0 && farmFieldObjectData.Active)
            {
                messageString += $"{farmFieldObjectData.FarmObjectData.Name} abgebaut! ";
                RemoveFarmFieldObjectData(farmFieldObjectData);
                StopFarming(rpPlayer);
            }
            else
            {
                messageString += $"{farmFieldObjectData.Capacity} kg noch verfügbar. ";
            }
            rpPlayer.SendNotification(messageString + $" erhalten! {farmFieldObjectData.Capacity} KG noch verfügbar.", RPPlayer.NotificationType.INFO, title: "Geiles Farmmodule");
#if DEBUG
            UpdateFarmObjLabel(farmFieldObjectData);
#endif
        }
#if DEBUG
        private void UpdateFarmObjLabel(FarmFieldObjectData farmFieldObjectData)
        {
            Rgba color = farmFieldObjectData.Active ? new Rgba(0, 255, 0, 255) : new Rgba(255, 0, 0, 255);
            farmFieldObjectData.PlayerLabel.SetText($"Farm Id: {farmFieldObjectData.Id}, Pos: { farmFieldObjectData.Position}, Cap: { farmFieldObjectData.Capacity}");
            farmFieldObjectData.PlayerLabel.Color = color;
        }
#endif
        private void RemoveFarmFieldObjectData(FarmFieldObjectData farmFieldObjectData)
        {
            if(farmFieldObjectData.Prop != null) farmFieldObjectData.Prop.Destroy();
            farmFieldObjectData.FarmFieldData.ActiveObjects--;
            farmFieldObjectData.Active = false;
            farmFieldObjectData.LastFarmed = DateTime.Now;
        }
        private void AddFarmFieldObjectData(FarmFieldData farmField)
        {
            int randomObjectCounter = MathUtils.RandomNumber(farmField.MinimumObjects, farmField.MaximumObjects);
            if (randomObjectCounter > farmField.ActiveObjects)
            {
                IEnumerable<FarmFieldObjectData> notActiveObjects = farmField.FarmFieldObjectData.Where(obj => obj.Active == false);
                int loopCounter = 0;
                int notActiveCounter = notActiveObjects.Count();
                while (farmField.ActiveObjects < randomObjectCounter && loopCounter < 30)
                {
                    //int rnd = MathUtils.RandomNumber(0, notActiveObjects.Count());
                    int rnd = MathUtils.RandomNumber(0, notActiveCounter);
                    FarmFieldObjectData obj = notActiveObjects.ElementAtOrDefault(rnd);
                    if (obj != null && !obj.Active && DateTime.Now >= obj.LastFarmed.AddMinutes(_minimumTimeToRespawn))
                    {
                        obj.Capacity = obj.FarmObjectData.Capacity;
                        obj.Active = true;
                        obj.Prop = PropStreamer.Create(obj.FarmObjectData.ObjectName, obj.Position, obj.Rotation, visible: true, streamRange: 100, frozen: true);
                        farmField.ActiveObjects++;
                        notActiveCounter--;
                    }
                    loopCounter++;
                }
            }
        }

        public void RespawnEvent(int slot)
        {
            _farmFields.Where(farmField => farmField.RespawnSlot == slot).ForEach(field => AddFarmFieldObjectData(field));
        }
        public void OnFiveSecondsUpdate()
        {
            foreach (KeyValuePair<RPPlayer,FarmFieldObjectData> kvp in _farmingPlayers)
            {
                if (kvp.Key == null)
                {
                    _farmingPlayers.Remove(kvp.Key);
                    continue;
                }
                
                FarmProcessStep(kvp.Key, kvp.Value);

                if (!CanFarmProcessItemsBeAdded(kvp.Key, kvp.Value)) StopFarming(kvp.Key);
            }
            _RespawnCounter++;
            if(_RespawnCounter >= _RespawnCounterLimit)
            {
                RespawnEvent(_RespawnSlot);
                _RespawnSlot++;
                if (_RespawnSlot > _RespawnSlotLimit)
                    _RespawnSlot = 0;
                _RespawnCounter = 0;
            }
        }
    }
}

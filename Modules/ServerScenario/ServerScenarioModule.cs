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
using GangRP_Server.Extensions;
using GangRP_Server.Handlers.Logger;
using GangRP_Server.Handlers.Player;
using GangRP_Server.Models;
using GangRP_Server.Modules.Garage;
using GangRP_Server.Modules.Inventory;
using GangRP_Server.Modules.Player;
using GangRP_Server.Modules.VehicleData;
using GangRP_Server.Utilities;
using GangRP_Server.Utilities.Crime;
using GangRP_Server.Utilities.InteractionMenu;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Modules.ServerScenario
{
    public class Loot
    {
        public string Propname { get; set; }
        public Position Position { get; set; }
        public Prop Prop { get; set; }

        public Loot(string propname, Position position, Prop prop)
        {
            Propname = propname;
            Position = position;
            Prop = prop;
        }
    }
    public sealed class ServerScenarioModule : ModuleBase, ILoadEvent, IPressedEEvent, IEntityColshapeHitEvent, IFiveteenMinuteUpdateEvent
    {
        private readonly ILogger _logger;
        private readonly RPContext _rpContext;
        private readonly IPlayerHandler _playerHandler;
        
        private Dictionary<int, ServerScenarioData> _serverScenarios = new Dictionary<int, ServerScenarioData>();
        private Dictionary<int, ServerScenarioLootData> _serverScenarioLoots = new Dictionary<int, ServerScenarioLootData>();

        public ServerScenarioModule(ILogger logger, RPContext rpContext, IPlayerHandler playerHandler)
        {
            _logger = logger;
            _rpContext = rpContext;
            _playerHandler = playerHandler;
        }

        public void OnLoad()
        {
            _serverScenarios = AddTableLoadEvent<ServerScenarioData>(_rpContext.ServerScenarioData.Include(d => d.ServerScenarioLootData).Include(d => d.ServerScenarioPropData), OnItemLoad).ToDictionary(data => data.Id);
        }

        public void OnItemLoad(ServerScenarioData serverScenarioData)
        {
            foreach(ServerScenarioLootData serverScenarioLootData in serverScenarioData.ServerScenarioLootData)
                _serverScenarioLoots.Add(serverScenarioLootData.Id, serverScenarioLootData);
        }

        public void SpawnScenario(ServerScenarioData serverScenarioData)
        {
            serverScenarioData.SpawnTime = DateTime.Now;
            serverScenarioData.IsActive = true;
            foreach (var serverScenarioPropData in serverScenarioData.ServerScenarioPropData)
            {
                if(!serverScenarioPropData.IsActive)
                    SpawnProp(serverScenarioPropData);
            }
        }

        public void DespawnScenario(ServerScenarioData serverScenarioData)
        {
            foreach (var serverScenarioPropData in serverScenarioData.ServerScenarioPropData)
            {
                if(!serverScenarioPropData.IsActive)
                    DespawnProp(serverScenarioPropData);
            }

            foreach (var serverScenarioLootData in serverScenarioData.ServerScenarioLootData)
            {
                if(!serverScenarioLootData.IsActive)
                    DespawnLoot(serverScenarioLootData);
            }
        }
        public void StartScenario(ServerScenarioData serverScenarioData)
        {
            SpawnLoots(serverScenarioData);
        }

        public void SpawnLoots(ServerScenarioData serverScenarioData)
        {
            foreach (var serverScenarioLootData in serverScenarioData.ServerScenarioLootData)
            {
                //if(MathUtils.RandomChance(2) < 0.5f)
                    if(!serverScenarioLootData.IsActive)
                        SpawnLoot(serverScenarioLootData);
            }
        }

        public void SpawnLoot(ServerScenarioLootData serverScenarioLootData)
        {
            serverScenarioLootData.IsActive = true;
            serverScenarioLootData.Prop = PropStreamer.Create(serverScenarioLootData.Propname,
                serverScenarioLootData.Position, serverScenarioLootData.Rotation, visible: true,
                streamRange: 100, frozen: true, isDynamic: true, collision: true);
            serverScenarioLootData.ColShape =
                Alt.CreateColShapeSphere(serverScenarioLootData.Position, serverScenarioLootData.Radius);
            serverScenarioLootData.ColShape.SetData("serverScenarioLootDataId", serverScenarioLootData.Id);
        }

        public void DespawnLoot(ServerScenarioLootData serverScenarioLootData)
        {
            serverScenarioLootData.IsActive = false;
            serverScenarioLootData.Prop.Delete();
            serverScenarioLootData.ColShape.Remove();
        }

        public void SpawnProp(ServerScenarioPropData serverScenarioPropData)
        {
            serverScenarioPropData.IsActive = true;
            serverScenarioPropData.Prop = PropStreamer.Create(serverScenarioPropData.Propname,
                serverScenarioPropData.Position, serverScenarioPropData.Rotation, visible: true,
                streamRange: 100, frozen: true, isDynamic: true, collision: true);
        }

        public void DespawnProp(ServerScenarioPropData serverScenarioPropData)
        {
            serverScenarioPropData.IsActive = false;
            serverScenarioPropData.Prop.Delete();
        }

        public async Task<bool> PickUpLoot(RPPlayer rpPlayer, int serverScenarioLootDataId)
        {
            ServerScenarioLootData serverScenarioLootData = GetServerScenarioLootDataById(serverScenarioLootDataId);
            if (!InventoryModule.Instance.CanItemAdded(rpPlayer.Inventory, serverScenarioLootData.ItemDataId,
                serverScenarioLootData.MaximumAmount)) return false;
            rpPlayer.PlayScenario("WORLD_HUMAN_WELDING");
            bool status = await rpPlayer.StartTask(3 * 1000);
            rpPlayer.StopAnimation(true);
            if (!status) return false;
            if (!serverScenarioLootData.IsActive) return false;
            DespawnLoot(serverScenarioLootData);
            InventoryModule.Instance.AddItem(rpPlayer.Inventory, serverScenarioLootData.ItemDataId,
                MathUtils.RandomNumber(serverScenarioLootData.MinimumAmount, serverScenarioLootData.MaximumAmount));
            return true;
        }

        public Task<bool> OnPressedE(IPlayer player)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            if (rpPlayer.GetData("serverScenarioLootDataId", out int serverScenarioLootDataId))
            {
                PickUpLoot(rpPlayer, serverScenarioLootDataId);
            }
            return Task.FromResult(true);
        }

        public void OnEntityColshapeHit(IColShape shape, IEntity entity, bool state)
        {
            if (entity is IPlayer player)
            {
                if (shape.GetData("serverScenarioLootDataId", out int serverScenarioLootDataId))
                {
                    RPPlayer rpPlayer = (RPPlayer) player;
                    if (state == false)
                    {
                        rpPlayer.DeleteData("serverScenarioLootDataId");
                    }
                    else
                    {
                        rpPlayer.SetData("serverScenarioLootDataId", serverScenarioLootDataId);
                    }
                }
            }
        }
        public void CheckForDespawn(ServerScenarioData serverScenarioData)
        {
            if (!serverScenarioData.IsActive) return;
            if (serverScenarioData.SpawnTime.AddMinutes(16) > DateTime.Now)
            {
                return;
            }
            else if (serverScenarioData.SpawnTime.AddMinutes(61) > DateTime.Now)
            {
                bool found = false;
                foreach(ServerScenarioLootData serverScenarioLootData in serverScenarioData.ServerScenarioLootData)
                {
                    if (serverScenarioLootData.IsActive)
                        found = true;
                }
                if(found == false)
                    DespawnScenario(serverScenarioData);
            }
            else if (serverScenarioData.SpawnTime.AddMinutes(61) <= DateTime.Now)
            {
                DespawnScenario(serverScenarioData);
            }
        }

        public void OnFiveteenMinuteUpdate()
        {
            _serverScenarios.Values.ForEach(CheckForDespawn);
        }

        public ServerScenarioData GetServerScenarioById(int serverScenarioId)
        {
            if (_serverScenarios.TryGetValue(serverScenarioId, out ServerScenarioData serverScenario))
                return serverScenario;
            return null;
        }

        public ServerScenarioData GetServerScenarioByPosition(Position position)
        {
            return _serverScenarios.Values.FirstOrDefault(d => d.Position.Distance(position) < 2.0f);
        }

        public ServerScenarioLootData GetServerScenarioLootDataById(int serverScenarioLootDataId)
        {
            if (_serverScenarioLoots.TryGetValue(serverScenarioLootDataId, out ServerScenarioLootData serverScenarioLootData))
                return serverScenarioLootData;
            return null;
        }
    }
}

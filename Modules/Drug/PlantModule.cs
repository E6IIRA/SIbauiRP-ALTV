using System.Collections.Generic;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using GangRP_Server.Core;
using GangRP_Server.Events;
using GangRP_Server.Handlers.Inventory;
using GangRP_Server.Handlers.Logger;
using GangRP_Server.Models;
using Microsoft.EntityFrameworkCore;
using GangRP_Server.Modules.Inventory;
using System.Linq;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Resources.Chat.Api;
using GangRP_Server.Utilities;
using GangRP_Server.Extensions;
using System;
using System.Reflection.Metadata.Ecma335;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Modules.Drug
{
    public sealed class PlantModule : ModuleBase<PlantModule>, ILoadEvent, IMinuteUpdateEvent, IFiveteenMinuteUpdateEvent
    {
        private readonly ILogger _logger;

        private readonly RPContext _rpContext;

        private readonly InventoryModule _inventoryModule;

        private int _maxPerformanceSlot = 10;
        private int _actualPerformanceSlot;
        private int _tempCounter;

        private List<Plant> _plants;
        private List<List<Plant>> _plantsMinuteUpdate = new List<List<Plant>>();

        public PlantModule(ILogger logger, RPContext rpContext, InventoryModule inventoryModule)
        {
            _logger = logger;
            _rpContext = rpContext;
            _inventoryModule = inventoryModule;
        }

        public int GetMaxPerformanceSlot()
        {
            return _maxPerformanceSlot;
        }

        public void OnLoad()
        {
            for (int i = 0; i < _maxPerformanceSlot; i++)
                _plantsMinuteUpdate.Add(new List<Plant>());
            _plants = AddTableLoadEvent<Models.Plant>(_rpContext.Plant.Where(pl => pl.HarvestPlayerId == 0).Include(d => d.PlantTypeData).ThenInclude(d => d.PlantTypeLootData), OnItemLoad).ToList();
        }

        private void OnItemLoad(Plant plant)
        {
            plant.Position = new Position(plant.PositionX, plant.PositionY, plant.PositionZ);
            plant.Rotation = new Rotation(0, 0, 0);

            plant.PlantTypeData.TimeToGrowScaled = plant.PlantTypeData.TimeToGrow / _maxPerformanceSlot;

            if (plant.GrowState < plant.PlantTypeData.TimeToGrowScaled / 2)
                CreatePlantProp(plant, plant.PlantTypeData.ObjectStageOne);
            else
                CreatePlantProp(plant, plant.PlantTypeData.ObjectStageTwo);
            plant.PerformanceSlot = _tempCounter % _maxPerformanceSlot;
            _plantsMinuteUpdate[_tempCounter % _maxPerformanceSlot].Add(plant);
#if DEBUG
            plant.PlayerLabel = TextLabelStreamer.Create($"Plant Id: {plant.Id}, State: {plant.GrowState}/{plant.PlantTypeData.TimeToGrowScaled}, Water: {plant.ActualWater}/{plant.PlantTypeData.MaximumWater}, Fertilizer: {plant.ActualFertilizer}/{plant.PlantTypeData.MaximumFertilizer}, Lootfactor: {plant.LootFactor}", plant.Position, color: new Rgba(255, 255, 0, 255));
#endif
            _tempCounter++;
        }
        public Plant GetPlant(IPlayer player)
        {
            RPPlayer rpPlayer = (RPPlayer)player;
            Plant plant = _plants.FirstOrDefault(pl => pl.Distance2D(rpPlayer.Position) < 0.75f);
            return plant;
        }
        public async Task<bool> HarvestPlant(IPlayer player)
        {
            RPPlayer rpPlayer = (RPPlayer)player;
            Plant plant = GetPlant(rpPlayer);
            if (plant == null) return false;
            if (!CanPlayerHarvest(rpPlayer, plant)) return false;

            if (plant.GrowState < 0.2f * plant.PlantTypeData.TimeToGrowScaled) return false;

            RemovePlant(plant);
            plant.HarvestDate = DateTime.Now;
            plant.HarvestPlayerId = rpPlayer.PlayerId;
            
            Dictionary<int, int> itemsToBeAdded = new Dictionary<int, int>();
            float growLootMultiplier = plant.LootFactor * ((float)plant.GrowState / (float)plant.PlantTypeData.TimeToGrowScaled);
            await plant.PlantTypeData.PlantTypeLootData.ForEach(loot => itemsToBeAdded.Add(loot.ItemDataId, Convert.ToInt32(loot.BaseAmount * growLootMultiplier)));
            await rpPlayer.Inventory.AddItems(itemsToBeAdded);

            await using RPContext rpContext = new RPContext();
            rpContext.Plant.Update(plant);
            await rpContext.SaveChangesAsync();
            return true;
        }

        public void RemovePlant(Plant plant)
        {
            _plants.Remove(plant);
            _plantsMinuteUpdate[plant.PerformanceSlot].Remove(plant);
            plant.Prop.Destroy();
#if DEBUG
            plant.PlayerLabel.Delete();
#endif
        }

        private bool CanPlayerHarvest(RPPlayer rpPlayer, Plant plant)
        {
            Dictionary<int, int> itemsToBeAdded = new Dictionary<int, int>();
            plant.PlantTypeData.PlantTypeLootData.ForEach(loot => itemsToBeAdded.Add(loot.ItemDataId, Convert.ToInt32(loot.BaseAmount * plant.LootFactor)));

            return _inventoryModule.CanItemsAdded(rpPlayer.Inventory, itemsToBeAdded);
        }

        public async Task<bool> CreatePlant(IPlayer player, int seedItemId)
        {
            RPPlayer rpPlayer = (RPPlayer)player;
            Plant plant = _plants.FirstOrDefault(pl => (pl.Distance2D(rpPlayer.Position) < 1.0f && pl.Position.Distance(rpPlayer.Position) < 3.0f) || pl.PlanterPlayerId == rpPlayer.Id);
            if (plant != null) return false;
            if (rpPlayer.DimensionType != DimensionType.WORLD || rpPlayer.Dimension != 0) return false;
            await AddPlantToDatabase(player, seedItemId);
            return true;
        }

        private async Task<Plant> AddPlantToDatabase(IPlayer player, int plantTypeId)
        {
            RPPlayer rpPlayer = (RPPlayer)player;
            Plant plant = new Plant
            {
                PlantTypeDataId = plantTypeId,
                GrowState = 0,
                LootFactor = 1.0f,
                PositionX = rpPlayer.Position.X,
                PositionY = rpPlayer.Position.Y,
                PositionZ = rpPlayer.Position.Z - 1.0f,
                Rotation = new Rotation(0, 0, 0),
                PlanterPlayerId = rpPlayer.PlayerId,
                ActualWater = 0,
                ActualFertilizer = 0,
                PlantDate = DateTime.Now,
                HarvestDate = DateTime.MinValue,
                HarvestPlayerId = 0
        };

            await using RPContext rpContext = new RPContext();
            await rpContext.Plant.AddAsync(plant);
            await rpContext.SaveChangesAsync();
            plant = await rpContext.Plant.Where(pl => pl.PlanterPlayerId == rpPlayer.PlayerId).Include(d => d.PlantTypeData).ThenInclude(d => d.PlantTypeLootData).OrderByDescending(d => d.Id).FirstOrDefaultAsync();

            plant.Position = new Position(plant.PositionX, plant.PositionY, plant.PositionZ);
            plant.Rotation = new Rotation(0, 0, 0);

            plant.PlantTypeData.TimeToGrowScaled = plant.PlantTypeData.TimeToGrow / _maxPerformanceSlot;

            if (plant.GrowState < plant.PlantTypeData.TimeToGrowScaled / 2)
                CreatePlantProp(plant, plant.PlantTypeData.ObjectStageOne);
            else
                CreatePlantProp(plant, plant.PlantTypeData.ObjectStageTwo);
            plant.PerformanceSlot = _tempCounter % _maxPerformanceSlot;
            _plantsMinuteUpdate[_tempCounter % _maxPerformanceSlot].Add(plant);
#if DEBUG
            plant.PlayerLabel = TextLabelStreamer.Create($"Plant Id: {plant.Id}, State: {plant.GrowState}/{plant.PlantTypeData.TimeToGrowScaled}, Water: {plant.ActualWater}/{plant.PlantTypeData.MaximumWater}, Fertilizer: {plant.ActualFertilizer}/{plant.PlantTypeData.MaximumFertilizer}, Lootfactor: {plant.LootFactor}", plant.Position, color: new Rgba(255, 255, 0, 255));
#endif
            _plants.Add(plant);
            return null;
        }

        public void OnMinuteUpdate()
        {
            _plantsMinuteUpdate[_actualPerformanceSlot % _maxPerformanceSlot].ForEach(plant =>
            {
                GrowPlant(plant);
            });
            _actualPerformanceSlot++;
        }
        public void OnFiveteenMinuteUpdate()
        {
            SavePlants();
        }
        public async Task SavePlants()
        {
            await using RPContext rpContext = new RPContext();
            foreach (Plant plant in _plants)
            {
                rpContext.Plant.Update(plant);
            }
            await rpContext.SaveChangesAsync();
        }

        public void CreatePlantProp(Plant plant, string modelname)
        {
            plant.Prop = PropStreamer.Create(modelname, plant.Position, plant.Rotation, visible: true, streamRange: 100, frozen: true, isDynamic: true, collision: false);
        }

        public void GrowPlant(Plant plant)
        {
            if (plant.GrowState >= plant.PlantTypeData.TimeToGrowScaled) return;

            if (plant.GrowState >= plant.PlantTypeData.TimeToGrowScaled / 2 && plant.Prop.Model == plant.PlantTypeData.ObjectStageOne)
            {
                plant.Prop.Destroy();
                CreatePlantProp(plant, plant.PlantTypeData.ObjectStageTwo);
            }

            if (plant.LootFactor < 2)
            {
                if (plant.ActualWater > 0 && plant.ActualFertilizer > 0)
                {
                    plant.LootFactor += 1.0f / plant.PlantTypeData.TimeToGrowScaled;
                    plant.ActualWater--;
                    plant.ActualFertilizer--;
                }
                else if (plant.ActualWater > 0)
                {
                    plant.LootFactor += 0.3f / plant.PlantTypeData.TimeToGrowScaled;
                    plant.ActualWater--;
                }
                else if (plant.ActualFertilizer > 0)
                {
                    plant.LootFactor += 0.3f / plant.PlantTypeData.TimeToGrowScaled;
                    plant.ActualFertilizer--;
                }
            }
            plant.GrowState++;
#if DEBUG
            _logger.Info($"Pflanze (Id:{plant.Id}) gewachsen! Status ({plant.GrowState}/{plant.PlantTypeData.TimeToGrowScaled}), " +
                $"Wasser ({plant.ActualWater}/{plant.PlantTypeData.MaximumWater}), Fertilizer ({plant.ActualFertilizer}/{plant.PlantTypeData.MaximumFertilizer})");
            UpdatePlantLabel(plant);
#endif
        }

#if DEBUG
        

        private void UpdatePlantLabel(Plant plant)
        {
            plant.PlayerLabel.SetText($"Plant Id: {plant.Id}, State: {plant.GrowState}/{plant.PlantTypeData.TimeToGrowScaled}, Water: {plant.ActualWater}/{plant.PlantTypeData.MaximumWater}, Fertilizer: {plant.ActualFertilizer}/{plant.PlantTypeData.MaximumFertilizer}, Lootfactor: {plant.LootFactor}");
        }
#endif

        public async Task<bool> WaterPlant(RPPlayer rpPlayer)
        {
            Plant plant = GetPlant(rpPlayer);
            if (plant == null) return false;

            if (plant.ActualWater == plant.PlantTypeData.MaximumWater) return false;
            plant.ActualWater = plant.PlantTypeData.MaximumWater;
            await SavePlant(plant);
#if DEBUG
            UpdatePlantLabel(plant);
#endif
            return true;
        }
        public async Task<bool> FertilizePlant(RPPlayer rpPlayer)
        {
            Plant plant = GetPlant(rpPlayer);
            if (plant == null) return false;

            if (plant.ActualFertilizer == plant.PlantTypeData.MaximumFertilizer) return false;
            plant.ActualFertilizer = plant.PlantTypeData.MaximumFertilizer;
            await SavePlant(plant);
#if DEBUG
            UpdatePlantLabel(plant);
#endif
            return true;
        }
        public async Task<Plant> SavePlant(Plant plant)
        {
            await using RPContext rpContext = new RPContext();
            rpContext.Plant.Update(plant);
            await rpContext.SaveChangesAsync();
            return null;
        }

        public string MeasureFertilizerPlant(RPPlayer rpPlayer)
        {
            Plant plant = GetPlant(rpPlayer);
            if (plant == null) return "Keine Pflanze zum Untersuchen gefunden!";

            if (plant.ActualFertilizer == 0)
                return "Diese Pflanze muss gedüngt werden!";
            if (plant.ActualFertilizer <= 3)
                return "Diese Pflanze sollte bald wieder gedüngt werden!";
            if (plant.ActualFertilizer <= 7)
                return "Diese Pflanze sollte bald wieder gedüngt werden!";
            return "Diese Pflanze hat genügend Dünger!";
        }

        public string MeasureWaterPlant(RPPlayer rpPlayer)
        {
            Plant plant = GetPlant(rpPlayer);
            if (plant == null) return "Keine Pflanze zum Untersuchen gefunden!";

            if (plant.ActualWater == 0)
                return "Diese Pflanze muss bewässert werden!";
            if (plant.ActualWater <= 3)
                return "Diese Pflanze sollte bald wieder bewässert werden!";
            if (plant.ActualWater <= 7)
                return "Diese Pflanze sollte bald wieder bewässert werden!";
            return "Diese Pflanze hat genügend Wasser!";
        }
    }
}

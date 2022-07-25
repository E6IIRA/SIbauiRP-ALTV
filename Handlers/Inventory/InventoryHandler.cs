using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AltV.Net;
using GangRP_Server.Core;
using GangRP_Server.Events;
using GangRP_Server.Handlers.Logger;
using GangRP_Server.Handlers.Player;
using GangRP_Server.Models;
using GangRP_Server.Modules.Inventory;
using Microsoft.EntityFrameworkCore;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Handlers.Inventory
{
    public class InventoryHandler : IInventoryHandler
    {
        private readonly ILogger _logger;

        private readonly Dictionary<int, LocalInventory> _localInventories;

        public InventoryHandler(ILogger logger)
        {
            _logger = logger;
            _localInventories = new Dictionary<int, LocalInventory>();
        }


        public async Task<LocalInventory> LoadInventory(int inventoryId, int assignedId = 0)
        {
            _logger.Info($"INV: " + inventoryId);
            await using RPContext rpContext = new RPContext();
            Models.Inventory inventory = await rpContext.Inventory
                .Include(d => d.Item)
                .Include(d => d.InventoryTypeData)
                .SingleOrDefaultAsync(i => i.Id == inventoryId);
            if (inventory == null) return new LocalInventory();
            InventoryTypeData inventoryTypeData = inventory.InventoryTypeData;
            if (inventoryTypeData == null) return new LocalInventory();
            LocalInventory localInventory = new LocalInventory
            {
                Id = inventory.Id,
                InventoryTypeData = inventoryTypeData,
                AssignedObjectId = assignedId
            };

            foreach (var item in inventory.Item)
            {
                string[] customItemDatasSplit = item.CustomItemData.Split(";");
                if (customItemDatasSplit.Length == 1)
                    customItemDatasSplit = null;
                LocalItem localItem = new LocalItem(item.ItemDataId, item.Amount, customItemDatasSplit);
                localInventory.InventoryItems.Add(item.Slot, localItem);
            }
            _localInventories.Add(localInventory.Id, localInventory);
            _logger.Info($"INV Loaded correctly: {inventoryId}");
            return localInventory;
        }


        public void UnloadInventory(int inventoryId)
        {
            if (_localInventories.ContainsKey(inventoryId)) _localInventories.Remove(inventoryId);
        }

        public LocalInventory? GetLocalInventory(int inventoryId)
        {
            if (_localInventories.TryGetValue(inventoryId, out LocalInventory? localInventory))
            {
                return localInventory;
            }
            return null;
        }
        public async Task ChangeItemAmountAtSlotAndRemoveItemFromSlot(int inventoryId, int slot, int newAmount, int secondInventoryId, int secondSlot)
        {
            await using RPContext rpContext = new RPContext();
            Item item = await rpContext.Item.FirstOrDefaultAsync(i => (i.InventoryId == inventoryId) && (i.Slot == slot));
            if (item != null)
            {
                item.Amount = newAmount;
                rpContext.Item.Update(item);
            }
            rpContext.Item.RemoveRange(rpContext.Item.Where(i => (i.InventoryId == secondInventoryId) && (i.Slot == secondSlot)));
            await rpContext.SaveChangesAsync();
        }


        public async Task ChangeItemAmountAndChangeItemAmount(int inventoryId, int slot, int newAmount, int secondInventoryId, int secondSlot, int secondNewAmount)
        {
            await using RPContext rpContext = new RPContext();
            Item item = await rpContext.Item.FirstOrDefaultAsync(i => (i.InventoryId == inventoryId) && (i.Slot == slot));
            if (item != null)
            {
                item.Amount = newAmount;
                rpContext.Item.Update(item);
            }
            Item secondItem = await rpContext.Item.FirstOrDefaultAsync(i => (i.InventoryId == secondInventoryId) && (i.Slot == secondSlot));
            if (secondItem != null)
            {
                secondItem.Amount = secondNewAmount;
                rpContext.Item.Update(secondItem);
            }
            await rpContext.SaveChangesAsync();
        }


        public async Task AddItemAndRemoveItem(int inventoryId, int itemId, int slot, int amount, string[]? customItemData, int secondInventoryId, int secondSlot)
        {
            await using RPContext rpContext = new RPContext();

            Item item = new Item
            {
                InventoryId = inventoryId,
                Slot = slot,
                ItemDataId = itemId,
                Amount = amount
            };
            if (customItemData != null) item.CustomItemData = CustomItemDataToString(customItemData);

            await rpContext.Item.AddAsync(item);

            rpContext.Item.RemoveRange(rpContext.Item.Where(i => (i.InventoryId == secondInventoryId) && (i.Slot == secondSlot)));

            await rpContext.SaveChangesAsync();
        }

        public async Task AddItemAndChangeAmount(int inventoryId, int itemId, int slot, int amount, int secondInventoryId, int secondSlot, int secondNewAmount)
        {
            await using RPContext rpContext = new RPContext();

            Item item = new Item
            {
                InventoryId = inventoryId,
                Slot = slot,
                ItemDataId = itemId,
                Amount = amount
            };

            await rpContext.Item.AddAsync(item);

            Item secondItem = await rpContext.Item.FirstOrDefaultAsync(i => (i.InventoryId == secondInventoryId) && (i.Slot == secondSlot));
            if (secondItem != null)
            {
                secondItem.Amount = secondNewAmount;
                rpContext.Item.Update(secondItem);
            }

            await rpContext.SaveChangesAsync();
        }

        public async Task SwitchItems(int inventoryId, int slot, int itemId, int newAmount, int secondInventoryId, int secondSlot, int secondItemId, int secondNewAmount, string[]? selectedCustomData = null, string[]? secondCustomData = null)
        {
            await using RPContext rpContext = new RPContext();

            Item item = await rpContext.Item.FirstOrDefaultAsync(i => (i.InventoryId == inventoryId) && (i.Slot == slot));
            if (item != null)
            {
                item.ItemDataId = itemId;
                item.Amount = newAmount;
                if (selectedCustomData != null) item.CustomItemData = CustomItemDataToString(selectedCustomData);
                rpContext.Item.Update(item);
            }
            Item secondItem = await rpContext.Item.FirstOrDefaultAsync(i => (i.InventoryId == secondInventoryId) && (i.Slot == secondSlot));
            if (secondItem != null)
            {
                secondItem.ItemDataId = secondItemId;
                secondItem.Amount = secondNewAmount;
                if (secondCustomData != null) secondItem.CustomItemData = CustomItemDataToString(secondCustomData);
                rpContext.Item.Update(secondItem);
            }
            await rpContext.SaveChangesAsync();
        }


        public async Task RemoveItemFromSlot(int inventoryId, int slotId)
        {
            await using RPContext rpContext = new RPContext();
            rpContext.Item.RemoveRange(rpContext.Item.Where(i => (i.InventoryId == inventoryId) && (i.Slot == slotId)));
            await rpContext.SaveChangesAsync();
        }

        public async Task ChangeAmountOnSlot(int inventoryId, int slotId, int amount)
        {
            await using RPContext rpContext = new RPContext();

            Item item = await rpContext.Item.FirstOrDefaultAsync(i => (i.InventoryId == inventoryId) && (i.Slot == slotId));
            if (item != null)
            {
                item.Amount = amount;
                rpContext.Item.Update(item);
            }

            await rpContext.SaveChangesAsync();
        }

        public async Task AddItemOnSlot(int inventoryId, int itemId, int slot, int amount = 1, string[]? customData = null)
        {
            await using RPContext rpContext = new RPContext();

            Item item = new Item
            {
                InventoryId = inventoryId,
                Slot = slot,
                ItemDataId = itemId,
                Amount = amount
            };
            if (customData != null) item.CustomItemData = CustomItemDataToString(customData);
            await rpContext.Item.AddAsync(item);
            await rpContext.SaveChangesAsync();
        }


        public async Task<Models.Inventory> CreateInventory(int inventoryTypeData)
        {
            await using RPContext rpContext = new RPContext();
            Models.Inventory inventory = new Models.Inventory();
            inventory.InventoryTypeDataId = inventoryTypeData;
            await rpContext.Inventory.AddAsync(inventory);
            await rpContext.SaveChangesAsync();
            return inventory;
        }

        public async Task<LocalInventory> LoadInventoryTypeForPlayer(RPPlayer rpPlayer, PlayerHandler.InventoryType invInventoryType)
        {
            if (rpPlayer.IsCreatingInventoryType.Contains((int)invInventoryType) ||
                rpPlayer.IsLoadingInventoryType.Contains((int)invInventoryType))
            {
                return null;
            }
            _logger.Info("New LoadInventoryType");
            await using RPContext rpContext = new RPContext();
            PlayerInventories? inv = null;
            if (invInventoryType == PlayerHandler.InventoryType.SPIELER)
            {
                inv = await rpContext.PlayerInventories.Include(d => d.Inventory).Where(d => (d.PlayerId == rpPlayer.PlayerId) && (d.Inventory.InventoryTypeDataId == (int)PlayerHandler.InventoryType.SPIELER || d.Inventory.InventoryTypeDataId == (int)PlayerHandler.InventoryType.KLEINER_RUCKSACK || d.Inventory.InventoryTypeDataId == (int)PlayerHandler.InventoryType.GROSSER_RUCKSACK)).FirstOrDefaultAsync();
            }
            else
            {
                inv = await rpContext.PlayerInventories.Include(d => d.Inventory).Where(d => (d.PlayerId == rpPlayer.PlayerId) && (d.Inventory.InventoryTypeDataId == (int)invInventoryType)).FirstOrDefaultAsync();
            }


            if (inv == null)
            {
                _logger.Info("IsNull");

                rpPlayer.IsCreatingInventoryType.Add((int)invInventoryType);
                Models.Inventory inventory = await CreateInventory((int) invInventoryType);
                await rpContext.PlayerInventories.AddAsync(new PlayerInventories() {InventoryId = inventory.Id, PlayerId = rpPlayer.PlayerId});
                await rpContext.SaveChangesAsync();
                LocalInventory localInventory = await LoadInventory(inventory.Id);
                rpPlayer.IsCreatingInventoryType.Remove((int) invInventoryType);
                return localInventory;
            }
            else
            {
                rpPlayer.IsLoadingInventoryType.Add((int)invInventoryType);
                if (_localInventories.TryGetValue(inv.InventoryId, out LocalInventory localInv))
                {
                    rpPlayer.IsLoadingInventoryType.Remove((int) invInventoryType);
                    return localInv;
                }

                LocalInventory localInventory = await LoadInventory(inv.InventoryId);
                rpPlayer.IsLoadingInventoryType.Remove((int) invInventoryType);
                return localInventory;
            }
        }

        public async Task RemoveInventory(int inventoryId)
        {
            await using RPContext rpContext = new RPContext();

            Models.Inventory firstOrDefaultAsync = await rpContext.Inventory.FirstOrDefaultAsync(d => d.Id == inventoryId);

            if (firstOrDefaultAsync != null)
            {
                rpContext.Item.RemoveRange(rpContext.Item.Where(d => d.InventoryId == inventoryId));
                rpContext.Inventory.Remove(firstOrDefaultAsync);
                await rpContext.SaveChangesAsync();

                UnloadInventory(inventoryId);
            }
        }

        public string CustomItemDataToString(string[] customItemData)
        {
            string data = "";

            if (customItemData != null)
            {
                foreach (var variable in customItemData)
                {
                    data += variable + ";";
                }
                if (data != "") return data.Remove(data.Length - 1);
            }
            return data;
        }

        public async Task UpdateInventoryTypeData(int inventoryId, int inventoryTypeDataId)
        {
            RPContext rpContext = new RPContext();
            Models.Inventory inventory = await rpContext.Inventory.FirstOrDefaultAsync(d => d.Id == inventoryId);

            if (inventory != null)
            {
                inventory.InventoryTypeDataId = inventoryTypeDataId;
            }
            await rpContext.SaveChangesAsync();
        }
    }
}

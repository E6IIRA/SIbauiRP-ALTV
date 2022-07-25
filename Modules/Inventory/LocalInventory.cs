using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GangRP_Server.Extensions;
using GangRP_Server.Models;
using GangRP_Server.Modules.Inventor;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Modules.Inventory
{
    public class LocalItem
    {
        public int ItemId { get; set; }
        public int Amount { get; set; }

        public string[]? CustomItemData { get; set; }

        public LocalItem(int itemId, int amount, string[]? customItemData = null)
        {
            ItemId = itemId;
            Amount = amount;
            CustomItemData = customItemData;
        }

        public String GetCustomItemDataString()
        {
            string data = "";

            if (CustomItemData != null)
            {
                foreach (var variable in CustomItemData)
                {
                    data += variable + ";";
                }
                if (data != "") return data.Remove(data.Length - 1);
            }
            return data;
        }


    }

    public class LocalInventory
    {
        public int Id { get; set; }
        public InventoryTypeData InventoryTypeData { get; set; }
        public int AssignedObjectId { get; set; }
        public Dictionary<int, LocalItem> InventoryItems { get; set; }


        public LocalInventory()
        {
            InventoryItems = new Dictionary<int, LocalItem>();
            AssignedObjectId = 0;
            InventoryTypeData = new InventoryTypeData();
        }

        public async Task<bool> AddItem(int itemId, int amount = 1, string[]? customData = null)
        {
            return await InventoryModule.Instance.AddItem(this, itemId, amount, customData);
        }

        public async Task<bool> AddItems(Dictionary<int, (int amount, string[]? customData)> items)
        {
            return await InventoryModule.Instance.AddItems(this, items);
        }

        public async Task<bool> AddItems(Dictionary<int, int> items)
        {
            return await InventoryModule.Instance.AddItems(this, items);
        }

        public async Task<bool> RemoveItem(int itemId, int amount = 1)
        {
            return await InventoryModule.Instance.RemoveItem(this, itemId, amount);
        }

        public async Task RemoveItemFromSlot(int itemId, int amount)
        {
            await InventoryModule.Instance.RemoveItemFromSlot(this, itemId, amount);
        }

        public bool HasItem(int itemId)
        {
            return InventoryModule.Instance.HasItem(this, itemId);
        }

        public bool HasItems(List<int> itemIds)
        {
            return InventoryModule.Instance.HasItems(this, itemIds);
        }

        public bool HasItemAmount(int itemId, int amount)
        {
            return InventoryModule.Instance.HasItemAmount(this, itemId, amount);
        }

        public bool HasItemsAmounts(Dictionary<int, int> itemAmounts)
        {
            return InventoryModule.Instance.HasItemsAmounts(this, itemAmounts);
        }

        public bool CanItemAdded(int itemId, int amount)
        {
            return InventoryModule.Instance.CanItemAdded(this, itemId, amount);
        }

        public bool CanItemsAdded(Dictionary<int, int> items)
        {
            return InventoryModule.Instance.CanItemsAdded(this, items);
        }

        public int GetFreeWeight()
        {
            return InventoryModule.Instance.GetFreeWeight(this);
        }
        public int GetUsedWeight()
        {
            return InventoryModule.Instance.GetUsedWeight(this);
        }
    }
}

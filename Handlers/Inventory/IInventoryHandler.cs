using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GangRP_Server.Core;
using GangRP_Server.Handlers.Player;
using GangRP_Server.Models;
using GangRP_Server.Modules.Inventory;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Handlers.Inventory
{
    public interface IInventoryHandler
    {
        Task<LocalInventory> LoadInventory(int inventoryId, int assignedId = 0);
        void UnloadInventory(int inventoryId);
        LocalInventory? GetLocalInventory(int inventoryId);
        Task ChangeItemAmountAtSlotAndRemoveItemFromSlot(int inventoryId, int slot, int newAmount, int secondInventoryId, int secondSlot);

        Task ChangeItemAmountAndChangeItemAmount(int inventoryId, int slot, int newAmount, int secondInventoryId, int secondSlot, int secondNewAmount);

        Task AddItemAndRemoveItem(int inventoryId, int itemId, int slot, int amount, string[]? customItemData, int secondInventoryId, int secondSlot);

        Task AddItemAndChangeAmount(int inventoryId, int itemId, int slot, int amount, int secondInventoryId, int secondSlot, int secondNewAmount);

        Task SwitchItems(int inventoryId, int slot, int itemId, int newAmount, int secondInventoryId, int secondSlot, int secondItemId, int secondNewAmount, string[]? selectedCustomData = null, string[]? secondCustomData = null);

        Task ChangeAmountOnSlot(int inventoryId, int slotId, int amount);

        Task RemoveItemFromSlot(int inventoryId, int slotId);
        Task AddItemOnSlot(int inventoryId, int itemId, int slot, int amount = 1, string[]? customData = null);

        Task<Models.Inventory> CreateInventory(int inventoryTypeData);

        Task<LocalInventory> LoadInventoryTypeForPlayer(RPPlayer rpPlayer, PlayerHandler.InventoryType inventoryTypeData);

        Task RemoveInventory(int inventoryId);

        string CustomItemDataToString(string[] customItemData);

        Task UpdateInventoryTypeData(int inventoryId, int inventoryTypeDataId);
    }
}

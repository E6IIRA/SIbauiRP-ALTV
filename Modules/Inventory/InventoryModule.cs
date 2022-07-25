using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using AltV.Net;
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
using GangRP_Server.Modules.Door;
using GangRP_Server.Modules.Drug;
using GangRP_Server.Modules.Farming;
using GangRP_Server.Modules.House;
using GangRP_Server.Modules.Inventory.Item;
using GangRP_Server.Modules.StorageRoom;
using GangRP_Server.Modules.VehicleKey;
using GangRP_Server.Utilities;
using GangRP_Server.Utilities.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Modules.Inventory
{
    public sealed class InventoryModule : ModuleBase<InventoryModule>, ILoadEvent, IPressedIEvent
    {
        private readonly ILogger _logger;
        private readonly RPContext _rpContext;
        private readonly IInventoryHandler _inventoryHandler;
        private readonly IVehicleHandler _vehicleHandler;

        private readonly HouseModule _houseModule;
        private readonly StorageRoomModule _storageroomModule;
        private readonly IItemHandler _itemHandler;

        private Dictionary<int, ItemData> _itemData;
        private Dictionary<int, InventoryTypeData> _inventoryTypeData;

        public InventoryModule(ILogger logger, RPContext rpContext, IInventoryHandler inventoryHandler, IVehicleHandler vehicleHandler,
                                HouseModule houseModule, IItemHandler itemHandler, StorageRoomModule storageroomModule)
        {
            _logger = logger;
            _rpContext = rpContext;
            _inventoryHandler = inventoryHandler;
            _vehicleHandler = vehicleHandler;
            _houseModule = houseModule;
            _itemHandler = itemHandler;
            _storageroomModule = storageroomModule;
        }

        public void OnLoad()
        {
            _itemData = _rpContext.ItemData.ToDictionary(data => data.Id);
            _inventoryTypeData = _rpContext.InventoryTypeData.ToDictionary(data => data.Id);
            AddClientEvent<int, int, int, int>("MoveItem", MoveItem);
            AddClientEvent<int, int>("UseItem", UseItem);
            AddClientEvent("PackBag", PackBag);
            AddClientEvent("PackPhone", PackPhone);
            AddClientEvent<int, int>("DropItem", DropItem);
        }


        private const int VEHICLE_TYPE_ID = 2;


        private bool IsInventoryAccessible(RPPlayer rpPlayer, LocalInventory inventory)
        {
            if (inventory.InventoryTypeData.TypeId == VEHICLE_TYPE_ID)
            {
                RPVehicle rpVehicle = _vehicleHandler.GetRpVehicle(inventory.AssignedObjectId);
                if (!rpVehicle.Locked && rpVehicle.TrunkStatus && rpPlayer.Position.Distance(rpVehicle.Position) < 3)
                {
                    return true;
                }
                //TODO ? Maybe close Inventory window? yeeeees
                return false;
            }
            return true;
        }

        async void MoveItem(IPlayer player, int selectedSlot, int targetSlot, int selectedInventoryId, int targetInventoryId)
        {
            LocalInventory selectedInventory = _inventoryHandler.GetLocalInventory(selectedInventoryId);
            LocalInventory targetInventory = _inventoryHandler.GetLocalInventory(targetInventoryId);
            if (selectedInventory == null || targetInventory == null) return;

            RPPlayer rpPlayer = (RPPlayer) player;

            //Check if one of the inventories is a vehicle => check if trunk is open and vehicle is open

            if (IsInventoryAccessible(rpPlayer, selectedInventory) && IsInventoryAccessible(rpPlayer, targetInventory))
            {
                //check if selected Inventory has the item you wanna move
                if (selectedInventory.InventoryItems.TryGetValue(selectedSlot, out LocalItem selectedItem))
                {
                    var selectedItemData = GetItemDataById(selectedItem.ItemId);
                    var freeTargetInventoryWeight = targetInventory.InventoryTypeData.MaxWeight - GetUsedWeight(targetInventory);
                    var freeSelectedInventoryWeight = selectedInventory.InventoryTypeData.MaxWeight - GetUsedWeight(selectedInventory);

                    //Check if target inventory has something on slot
                    if (targetInventory.InventoryItems.TryGetValue(targetSlot, out LocalItem targetItem))
                    {
                        //there is an item on this slot

                        var targetItemData = GetItemDataById(targetItem.ItemId);

                        if (selectedItem.ItemId == targetItem.ItemId && selectedItem.CustomItemData == targetItem.CustomItemData)
                        {
                            //same item, needs to be stacked

                            //max of items which could be added to the stack
                            var possibleStackSizeToAdd = targetItemData.Stacksize - targetItem.Amount;

                            //max of items which could be added to the inventory looking at weight
                            var possibleStackToAddByWeight = freeTargetInventoryWeight / selectedItemData.Weight;

                            //items to be added to the stack
                            var itemsToBeAdded = selectedItem.Amount;

                            if (possibleStackToAddByWeight >= itemsToBeAdded)
                            {
                                //Inventory has enough free weight for everything

                                if (possibleStackSizeToAdd >= itemsToBeAdded)
                                {
                                    //amount fits into target Amount
                                    selectedInventory.InventoryItems.Remove(selectedSlot);
                                    int newTargetAmount = targetItem.Amount + itemsToBeAdded;
                                    targetInventory.InventoryItems[targetSlot].Amount = newTargetAmount;
                                    await _inventoryHandler.ChangeItemAmountAtSlotAndRemoveItemFromSlot(targetInventory.Id, targetSlot, newTargetAmount, selectedInventory.Id, selectedSlot);

                                }
                                else
                                {
                                    //amount doesnt fit, just add possible amount


                                    int newSelectAmount = selectedItem.Amount - possibleStackSizeToAdd;
                                    int newTargetAmount = targetItem.Amount + possibleStackSizeToAdd;

                                    selectedInventory.InventoryItems[selectedSlot].Amount = newSelectAmount;
                                    targetInventory.InventoryItems[targetSlot].Amount = newTargetAmount;
                                    await _inventoryHandler.ChangeItemAmountAndChangeItemAmount(selectedInventory.Id, selectedSlot, newSelectAmount, targetInventory.Id, targetSlot, newTargetAmount);
                                }

                            }
                            else
                            {
                                //Inventory doesnt have enough free weight for everything => just add possibleStackToAddByWeight

                                int changeAmount = 0;

                                if (possibleStackSizeToAdd >= possibleStackToAddByWeight)
                                {
                                    //everything fits into there
                                    changeAmount = possibleStackToAddByWeight;
                                }
                                else
                                {
                                    //slot hasnt enough space to add everything, just add as much as possible
                                    changeAmount = possibleStackSizeToAdd;
                                }

                                int newSelectAmount = selectedItem.Amount - changeAmount;
                                int newTargetAmount = targetItem.Amount + changeAmount;

                                selectedInventory.InventoryItems[selectedSlot].Amount = newSelectAmount;
                                targetInventory.InventoryItems[targetSlot].Amount = newTargetAmount;

                                await _inventoryHandler.ChangeItemAmountAndChangeItemAmount(selectedInventory.Id, selectedSlot, newSelectAmount, targetInventory.Id, targetSlot, newTargetAmount);
                            }
                        }
                        else
                        {
                            //different item, needs to be switched

                            if (freeTargetInventoryWeight + targetItem.Amount * targetItemData.Weight >= selectedItem.Amount * selectedItemData.Weight
                                && freeSelectedInventoryWeight + selectedItem.Amount * selectedItemData.Weight >= targetItem.Amount * targetItemData.Weight)
                            {
                                //SWITCH IS ALLOWED
                                var temp = selectedInventory.InventoryItems[selectedSlot];
                                selectedInventory.InventoryItems[selectedSlot] = targetInventory.InventoryItems[targetSlot];
                                targetInventory.InventoryItems[targetSlot] = temp;

                                int targetItemId = targetItem.ItemId;
                                int targetItemAmount = targetItem.Amount;
                                string[] targetCustomData = targetItem.CustomItemData;

                                int selectedItemId = selectedItem.ItemId;
                                int selectedItemAmount = selectedItem.Amount;
                                string[] selectedCustomData = selectedItem.CustomItemData;


                                await _inventoryHandler.SwitchItems(selectedInventory.Id, selectedSlot, targetItemId, targetItemAmount, targetInventory.Id, targetSlot, selectedItemId, selectedItemAmount, targetCustomData, selectedCustomData);
                            }
                        }

                    }
                    else
                    {
                        //there is no item on this slot

                        var maxAddedByWeight = freeTargetInventoryWeight / selectedItemData.Weight;

                        //_logger.Info("-----------------------");
                        //_logger.Info($"MaxAddedByWeight: {maxAddedByWeight}");
                        //_logger.Info($"selectedItemAmount: {selectedItem.Amount}");
                        //_logger.Info("-----------------------");


                        if (maxAddedByWeight >= selectedItem.Amount)
                        {
                            //everything fits
                            selectedInventory.InventoryItems.Remove(selectedSlot);
                            targetInventory.InventoryItems.Add(targetSlot, new LocalItem(selectedItem.ItemId, selectedItem.Amount, selectedItem.CustomItemData));
                            int amount = selectedItem.Amount;
                            int itemId = selectedItem.ItemId;
                            string[] data = selectedItem.CustomItemData;

                            await _inventoryHandler.AddItemAndRemoveItem(targetInventory.Id, itemId, targetSlot, amount, data, selectedInventory.Id, selectedSlot);

                        }
                        else
                        {
                            //just maxAddedByWeight fits

                            if (maxAddedByWeight == 0) return;

                            selectedInventory.InventoryItems[selectedSlot].Amount = selectedItem.Amount - maxAddedByWeight;
                            targetInventory.InventoryItems.Add(targetSlot, new LocalItem(selectedItem.ItemId, maxAddedByWeight));
                            await _inventoryHandler.AddItemAndChangeAmount(targetInventory.Id, selectedItem.ItemId, targetSlot, maxAddedByWeight, selectedInventory.Id, selectedSlot, selectedInventory.InventoryItems[selectedSlot].Amount);

                        }
                    }
                }
                else
                {
                    //something is really fucked up
                    return;
                }
            }




        }


        async void UseItem(IPlayer player, int inventoryId, int slotId)
        {
            LocalInventory? inventory = _inventoryHandler.GetLocalInventory(inventoryId);

            if (inventory == null) return;

            RPPlayer rpPlayer = (RPPlayer) player;

            if (IsInventoryAccessible(rpPlayer, inventory))
            {
                if (inventory.InventoryItems.TryGetValue(slotId, out LocalItem? item))
                {
                    if (item.Amount == 0) return;

                    bool used = await _itemHandler.TryUseItem(rpPlayer, item);

                    ItemData itemData = GetItemDataById(item.ItemId);

                    if (itemData.RemoveOnUse && used)
                    {
                        //REMOVE ITEM
                        await RemoveItemFromSlot(inventory, slotId);
                    }
                }
            }

        }

        void DropItem(IPlayer player, int inventoryId, int slotId)
        {
            LocalInventory? inventory = _inventoryHandler.GetLocalInventory(inventoryId);

            if (inventory == null) return;

            RPPlayer rpPlayer = (RPPlayer) player;

            if (IsInventoryAccessible(rpPlayer, inventory))
            {
                if (inventory.InventoryItems.TryGetValue(slotId, out LocalItem? item))
                {
                    RemoveItemFromSlot(inventory, slotId);
                }
            }
        }

        async void PackPhone(IPlayer player)
        {
            RPPlayer rpPlayer = (RPPlayer) player;

            if (rpPlayer.HasSmartphone)
            {
                rpPlayer.HasSmartphone = false;
                rpPlayer.SendNotification("Du hast dein Smartphone in die Tasche gepackt", RPPlayer.NotificationType.SUCCESS, "Inventar");
                await rpPlayer.SaveHasSmartphone();
                await AddItem(rpPlayer.Inventory, 19);
            }
        }



        async void PackBag(IPlayer player)
        {
            RPPlayer rpPlayer = (RPPlayer) player;

            //check if inventory is a player inventory
            if (rpPlayer.Inventory.InventoryTypeData.TypeId != 1) return;

            if (GetUsedWeight(rpPlayer.Inventory) > _inventoryTypeData[1].MaxWeight || rpPlayer.Inventory.InventoryItems.Count > _inventoryTypeData[1].MaxSlots)
            {
                rpPlayer.SendNotification("Der Rucksack kann nicht verstaut werden. Leere zuerst den Rucksack.", RPPlayer.NotificationType.ERROR, "Inventar");
                return;
            }



            if (rpPlayer.Inventory.InventoryTypeData.Id == 5)
            {
                await AddItem(rpPlayer.Inventory, 13);
            } else if (rpPlayer.Inventory.InventoryTypeData.Id == 6)
            {
                await AddItem(rpPlayer.Inventory, 14);
            }
            else
            {
                return;
            }

            await ResortInventory(rpPlayer.Inventory, _inventoryTypeData[1].MaxSlots);

            //back to normal inventory
            rpPlayer.Inventory.InventoryTypeData = _inventoryTypeData[1];
            await _inventoryHandler.UpdateInventoryTypeData(rpPlayer.InventoryId, 1);
            rpPlayer.SendNotification("Der Rucksack wurde verstaut", RPPlayer.NotificationType.SUCCESS, "Inventar");

        }

        public ItemData GetItemDataById(int itemDataId)
        {
            if (_itemData.TryGetValue(itemDataId, out ItemData? itemData))
            {
                return itemData;
            }
            return _itemData[0];
        }

        public ItemData? GetItemDataByName(string itemDataName)
        {
            return _itemData.Values.FirstOrDefault(d => d.Name.ToLower().Contains(itemDataName.ToLower()));
        }

        public InventoryTypeData GetInventoryTypeDataById(int inventoryTypeDataId)
        {
            if (_inventoryTypeData.TryGetValue(inventoryTypeDataId, out InventoryTypeData? itemInventoryTypeData))
            {
                return itemInventoryTypeData;
            }
            return _inventoryTypeData[0];
        }

        public async Task<bool> OnPressedI(IPlayer player)
        {
            RPPlayer rpPlayer = (RPPlayer) player;

            LocalInventory localInventory = _inventoryHandler.GetLocalInventory(rpPlayer.InventoryId);
            LocalInventory secondLocalInventory = null;
            if (localInventory == null) return false;


            if (rpPlayer.DimensionType == DimensionType.HOUSE)
            {
                if (_houseModule._houses.TryGetValue(rpPlayer.Dimension, out Models.House house))
                {
                    if (rpPlayer.GetHouseControlLevel(house) == 2)
                    {
                        HouseInteriorPosition houseInteriorPosition = house.HouseInteriorPosition.FirstOrDefault(d => d.InteriorPositionData.Position.Distance(rpPlayer.Position) < 2);

                        if (houseInteriorPosition != null)
                        {
                            secondLocalInventory = _inventoryHandler.GetLocalInventory(houseInteriorPosition.InventoryId);
                        }
                    }
                }
                    



            }
            else if (rpPlayer.DimensionType == DimensionType.STORAGEROOM)
            {
                if (_storageroomModule._storagerooms.TryGetValue(rpPlayer.Dimension, out Storageroom storageroom))
                {
                    if (rpPlayer.GetStorageroomControlLevel(storageroom) == 2)
                    {
                        StorageroomInteriorPosition storageroomInteriorPosition = storageroom.StorageroomInteriorPosition.FirstOrDefault(d => d.InteriorPositionData.Position.Distance(rpPlayer.Position) < 1.5f);
                        
                        if (storageroomInteriorPosition != null)
                        {
                            secondLocalInventory =
                                _inventoryHandler.GetLocalInventory(storageroomInteriorPosition.InventoryId);
                        }
                    }
                }
            }
            else if (rpPlayer.DimensionType == DimensionType.CAMPER)
            {
                if (rpPlayer.GetData("camperVehicleId", out int camperVehicleId) && rpPlayer.GetData("camperVehicleTeamId", out int camperVehicleTeamId))
                {
                    if (rpPlayer.TeamId == camperVehicleTeamId)
                    {
                        //TODO: ABFRAGE, OB TEAM DES SPIELERS GANG / MAFIA ETC. IST

                        if (rpPlayer.Position.Distance(Positions.CamperInputPosition) < 0.5f)
                        {
                            if (rpPlayer.CamperInputInventory == null)
                            {
                                secondLocalInventory =
                                    await _inventoryHandler.LoadInventoryTypeForPlayer(rpPlayer,
                                        PlayerHandler.InventoryType.CAMPERINPUT);
                                rpPlayer.CamperInputInventory = secondLocalInventory;
                            }
                            else
                            {
                                secondLocalInventory = rpPlayer.CamperInputInventory;
                            }
                        }
                        else if (rpPlayer.Position.Distance(Positions.CamperOutputPosition) < 0.5f)
                        {
                            if (rpPlayer.CamperOutputInventory == null)
                            {
                                secondLocalInventory =
                                    await _inventoryHandler.LoadInventoryTypeForPlayer(rpPlayer,
                                        PlayerHandler.InventoryType.CAMPEROUTPUT);
                                rpPlayer.CamperOutputInventory = secondLocalInventory;
                            }
                            else
                            {
                                secondLocalInventory = rpPlayer.CamperOutputInventory;
                            }
                        }
                    }
                }
            }
            else if (rpPlayer.DimensionType == DimensionType.WORLD)
            {
                RPVehicle rpVehicle = _vehicleHandler.GetClosestRpVehicle(rpPlayer.Position, 3);

                if (rpVehicle != null && !rpVehicle.Locked && rpVehicle.TrunkStatus)
                {
                    secondLocalInventory = _inventoryHandler.GetLocalInventory(rpVehicle.InventoryId);
                }
                else if (rpPlayer.Position.Distance(Positions.JailOutsidePosition) < 2.0)
                {
                    if (rpPlayer.LockerInventory == null)
                    {
                        secondLocalInventory = await _inventoryHandler.LoadInventoryTypeForPlayer(rpPlayer, PlayerHandler.InventoryType.SPIND);
                        rpPlayer.LockerInventory = secondLocalInventory;
                    }
                    else
                    {
                        secondLocalInventory = rpPlayer.LockerInventory;
                    }
                }
                else if (rpPlayer.Position.Distance(Positions.WareExportInventoryPosition) < 2.0)
                {
                    if (rpPlayer.WareExportInventory == null)
                    {
                        secondLocalInventory =
                            await _inventoryHandler.LoadInventoryTypeForPlayer(rpPlayer,
                                PlayerHandler.InventoryType.WAREEXPORT);
                        rpPlayer.WareExportInventory = secondLocalInventory;
                    }
                    else
                    {
                        secondLocalInventory = rpPlayer.WareExportInventory;
                    }
                }


            }


            player.Emit("ShowIF", "Inventory", new InventoryWriter(localInventory, secondLocalInventory));

            return false;
        }


        public int GetUsedWeight(LocalInventory inventory)
        {
            int usedWeight = 0;
            foreach (var item in inventory.InventoryItems.Values)
            {
                usedWeight += item.Amount * GetItemDataById(item.ItemId).Weight;
            }
            return usedWeight;
        }

        public int GetFreeWeight(LocalInventory inventory)
        {
            return inventory.InventoryTypeData.MaxWeight - GetUsedWeight(inventory);
        }

        public async Task RemoveItemFromSlot(LocalInventory inventory, int slotId, int amount = 1)
        {
            if (inventory.InventoryItems.TryGetValue(slotId, out LocalItem item))
            {
                if (item.Amount > 1)
                {
                    item.Amount -= amount;
                    if (item.Amount <= 0)
                    {
                        await _inventoryHandler.RemoveItemFromSlot(inventory.Id, slotId);
                    }
                    else
                    {
                        await _inventoryHandler.ChangeAmountOnSlot(inventory.Id, slotId, item.Amount);
                    }
                }
                else
                {
                    inventory.InventoryItems.Remove(slotId);
                    await _inventoryHandler.RemoveItemFromSlot(inventory.Id, slotId);
                }
            }
        }

        public bool HasItem(LocalInventory inventory, int itemId)
        {
            foreach (var item in inventory.InventoryItems.Values)
            {
                if (item.ItemId == itemId)
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasItems(LocalInventory inventory, List<int> itemIds)
        {
            foreach (var item in inventory.InventoryItems.Values)
            {
                foreach (int itemId in itemIds)
                {
                    if (item.ItemId == itemId)
                    {
                        itemIds.Remove(itemId);
                        break;
                    }
                }
            }

            return itemIds.Count == 0;
        }

        public bool HasItemAmount(LocalInventory inventory, int itemId, int amount)
        {
            int amountFound = 0;
            foreach (var item in inventory.InventoryItems.Values)
            {
                if (item.ItemId == itemId)
                {
                    amountFound += item.Amount;
                    if (amountFound >= amount)
                        return true;
                }
            }

            return false;
        }

        public bool HasItemsAmounts(LocalInventory inventory, Dictionary<int, int> itemAmounts)
        {
            foreach (var kvp in itemAmounts)
            {
                if (!HasItemAmount(inventory, kvp.Key, kvp.Value))
                    return false;
            }
            return true;
        }

        public async Task<int> GetItemAmountAndRemove(LocalInventory inventory, int itemId)
        {
            int amountFound = 0;
            foreach (var item in inventory.InventoryItems)
            {
                if (item.Value.ItemId == itemId)
                {
                    amountFound += item.Value.Amount;
                    inventory.InventoryItems.Remove(item.Key);
                    await _inventoryHandler.RemoveItemFromSlot(inventory.Id, item.Key);
                }
            }

            return amountFound;
        }

        public async Task<Dictionary<int,int>> GetItemsAmountAndRemove(LocalInventory inventory, List<int> itemIds)
        {
            Dictionary<int, int> removedItemds = new Dictionary<int, int>();
            foreach (var itemId in itemIds)
            {
                int amount = await GetItemAmountAndRemove(inventory, itemId);
                removedItemds.Add(itemId, amount);
            }
            return removedItemds;
        }

        /*public bool HasItemsAmounts(LocalInventory inventory, Dictionary<int, int> itemAmounts)
        {
            Dictionary<int, int> tempDictionary = new Dictionary<int, int>();
            foreach (KeyValuePair<int, int> kvp in itemAmounts)
            {
                tempDictionary
                foreach (var item in inventory.InventoryItems.Values)
                {
                    if (item.ItemId == kvp.Key && !foundedItems.Contains(kvp.Key))
                    {
                        itemAmounts[kvp.Key] = kvp.Value - item.Amount;
                        if (itemAmounts[kvp.Key] <= 0)
                            foundedItems.Add(kvp.Key);
                        break;

                    }
                }
            }
            return itemAmounts.Count == foundedItems.Count;
        }*/

        public bool CanItemAdded(LocalInventory inventory, int itemId, int amount = 1)
        {
            ItemData itemData = GetItemDataById(itemId);
            var localItems = inventory.InventoryItems.Values.Where(d => d.ItemId == itemId);

            int toBeAdded = amount;

            foreach (var localItem in localItems)
            {
                toBeAdded -= itemData.Stacksize - localItem.Amount;

                if (toBeAdded <= 0) break;
            }
            if (toBeAdded > 0)
            {
                int freeSlots = inventory.InventoryTypeData.MaxSlots - inventory.InventoryItems.Count;
                if (freeSlots <= 0) return false;

                if (freeSlots < toBeAdded / itemData.Stacksize)
                {
                    return false;
                }
            }
            return itemData.Weight * amount <= GetFreeWeight(inventory);
        }

        public bool CanItemsAdded(LocalInventory inventory, Dictionary<int, int> itemIdAmountPair)
        {
            /* !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
             * !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
             * !!!!!!!!!!                  !!!!!!!!!!!!!
             * !!!!!!!!!!   UNTESTED YET   !!!!!!!!!!!!!
             * !!!!!!!!!!                  !!!!!!!!!!!!!
             * !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
             * !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
             */
            int itemsWeight = 0;
            int freeSlots = inventory.InventoryTypeData.MaxSlots - inventory.InventoryItems.Count;
            foreach(KeyValuePair<int, int> kvp in itemIdAmountPair)
            {
                int itemId = kvp.Key;
                int toBeAdded = kvp.Value;
                ItemData itemData = GetItemDataById(kvp.Key);
                itemsWeight += itemData.Weight * toBeAdded;
                var localItems = inventory.InventoryItems.Values.Where(d => d.ItemId == itemId);
                foreach (var localItem in localItems)
                {
                    toBeAdded -= itemData.Stacksize - localItem.Amount;
                    if (toBeAdded <= 0) break;
                }
                if (toBeAdded > 0)
                {
                    if (freeSlots <= 0) return false;

                    if (freeSlots < toBeAdded / itemData.Stacksize)
                    {
                        return false;
                    }
                    freeSlots -= toBeAdded / itemData.Stacksize;
                }
            }
            return itemsWeight <= GetFreeWeight(inventory);
        }

        public async Task<bool> RemoveItem(LocalInventory inventory, int itemId, int amount = 1)
        {
            ItemData itemData = GetItemDataById(itemId);
            var localItems = inventory.InventoryItems.Where(d => d.Value.ItemId == itemId).ToDictionary(pair => pair.Key ).Values;

            int toBeRemoved = amount;
            foreach (var localItem in localItems)
            {
                if (localItem.Value.Amount > toBeRemoved)
                {
                    //just remove a bit and save
                    int newAmount = localItem.Value.Amount -= toBeRemoved;
                    toBeRemoved -= localItem.Value.Amount;
                    localItem.Value.Amount = newAmount;
                    await _inventoryHandler.ChangeAmountOnSlot(inventory.Id, localItem.Key, newAmount);
                }
                else
                {
                    //its not enough, remove full stack and save
                    toBeRemoved -= localItem.Value.Amount;
                    inventory.InventoryItems.Remove(localItem.Key);
                    await _inventoryHandler.RemoveItemFromSlot(inventory.Id, localItem.Key);
                }
                if (toBeRemoved <= 0) return true;
            }
            return false;
        }


        public async Task<bool> AddItem(LocalInventory inventory, int itemId, int amount = 1, string[]? customItemData = null)
        {
            ItemData itemData = GetItemDataById(itemId);
            var localItems = inventory.InventoryItems.Where(d => (d.Value.ItemId == itemId) && (d.Value.Amount < itemData.Stacksize) && d.Value.CustomItemData == customItemData).ToDictionary(pair => pair.Key).Values;

            int toBeAdded = amount;

            foreach (var localItem in localItems)
            {
                int oldAmount = localItem.Value.Amount;
                int newAmount = localItem.Value.Amount += toBeAdded;

                newAmount = newAmount >= itemData.Stacksize ? itemData.Stacksize : newAmount;

                localItem.Value.Amount = newAmount;

                toBeAdded -= (newAmount - oldAmount);

                await _inventoryHandler.ChangeAmountOnSlot(inventory.Id, localItem.Key, newAmount);
                if (toBeAdded <= 0) return true;
            }

            //Add new Stacks

            for (int i = 0; i < inventory.InventoryTypeData.MaxSlots; i++)
            {
                if (inventory.InventoryItems.Keys.Contains(i))continue;

                LocalItem item = new LocalItem(itemId, toBeAdded >= itemData.Stacksize ? itemData.Stacksize : toBeAdded, customItemData);
                toBeAdded -= itemData.Stacksize;
                inventory.InventoryItems.Add(i, item);
                await _inventoryHandler.AddItemOnSlot(inventory.Id, itemId, i, item.Amount, customItemData);

                if (toBeAdded <= 0)
                {
                    return true;
                }
            }
            return false;
        }
    
        public async Task<bool> AddItems(LocalInventory inventory, Dictionary<int, (int amount, string[]? customData)> itemsToAdd)
        {
            foreach(var keyValuePair in itemsToAdd)
            {
                if (await AddItem(inventory, keyValuePair.Key, keyValuePair.Value.amount, keyValuePair.Value.customData) == false)
                    return false;
            }
            return true;
        }
    
        public async Task<bool> AddItems(LocalInventory inventory, Dictionary<int, int> itemsToAdd)
        {
            foreach(var keyValuePair in itemsToAdd)
            {
                if (await AddItem(inventory, keyValuePair.Key, keyValuePair.Value) == false)
                    return false;
            }
            return true;
        }

        public async Task<bool> ResortInventory(LocalInventory inventory, int newInventorySlots)
        {
            //free slots
            List<int> freeSlots = new List<int>();
            for (int i = 0; i < inventory.InventoryTypeData.MaxSlots; i++)
            {
                if (!inventory.InventoryItems.ContainsKey(i)) freeSlots.Add(i);
            }
            //Find items which are on slots which will be removed after smaller backpack

            foreach (KeyValuePair<int, LocalItem> variable in inventory.InventoryItems.Where(d => d.Key > newInventorySlots).ToList())
            {
                inventory.InventoryItems.Add(freeSlots.First(), variable.Value);
                inventory.InventoryItems.Remove(variable.Key);
                await _inventoryHandler.AddItemAndRemoveItem(inventory.Id, variable.Value.ItemId, freeSlots.First(), variable.Value.Amount, variable.Value.CustomItemData, inventory.Id, variable.Key);
                freeSlots.RemoveAt(0);
            }
            return true;
        }
    }
}

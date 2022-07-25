using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GangRP_Server.Core;
using GangRP_Server.Handlers.Inventory;
using GangRP_Server.Modules.Inventor;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Modules.Inventory.Item
{
    public class Backpack : IItemScript
    {
        private readonly IInventoryHandler _inventoryHandler;

        public int[] ItemId => new[] { 13, 14 };

        public Backpack(IInventoryHandler inventoryHandler)
        {
            _inventoryHandler = inventoryHandler;
        }

        public async Task<bool> OnItemUse(RPPlayer rpPlayer, LocalItem item)
        {
            /*
             *  Wenn weitere Rucksäcke angelegt werden, dann unbedingt in der Funktion LoadInventoryTypeForPlayer bei if (type == .Spieler) ebenfalls hinzufügen!!!
             *  Sonst werden ggf. Inventare neu angelegt, obwohl schon eins vorhanden ist.
             */
            //check if player has no backpack
            if (rpPlayer.Inventory.InventoryTypeData.Id == 1)
            {
                //das ist der kleine Rucksack quasi
                int inventoryTypeDataId = 5;
                //Wenn aber großer Rucksack, dann halt großer Rucksack
                if (item.ItemId == 14) inventoryTypeDataId = 6;
                rpPlayer.Inventory.InventoryTypeData = InventoryModule.Instance.GetInventoryTypeDataById(inventoryTypeDataId);

                await _inventoryHandler.UpdateInventoryTypeData(rpPlayer.Inventory.Id, inventoryTypeDataId);

                rpPlayer.SendNotification("Rucksack angelegt", RPPlayer.NotificationType.SUCCESS, "Inventar");

                return true;
            }

            return false;
        }
    }
}

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
    public class Smartphone : IItemScript
    {

        public int[] ItemId => new[] { 19 };

        public async Task<bool> OnItemUse(RPPlayer rpPlayer, LocalItem item)
        {
            if (!rpPlayer.HasSmartphone)
            {
                rpPlayer.HasSmartphone = true;
                rpPlayer.SendNotification("Du hast das Smartphone ausgerüstet", RPPlayer.NotificationType.SUCCESS, "Inventar");
                await rpPlayer.SaveHasSmartphone();
                return true;
            }

            return false;
        }
    }
}

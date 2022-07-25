using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using GangRP_Server.Core;
using GangRP_Server.Models;
using GangRP_Server.Modules.Inventor;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Modules.Inventory.Item
{
    public class ProtectiveVest : IItemScript
    {
        public int[] ItemId => new[] { 1 };

        public async Task<bool> OnItemUse(RPPlayer rpPlayer, LocalItem item)
        {
            rpPlayer.PlayAnimation(Animation.KNEEL);
            bool status = await rpPlayer.StartTask(5000);

            if (status)
            {
                await rpPlayer.SetArmorAsync(100);
            }

            rpPlayer.StopAnimation(true);

            return status;
        }
    }
}

using GangRP_Server.Core;
using GangRP_Server.Modules.Inventor;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GangRP_Server.Utilities;
using GangRP_Server.Utilities.Scratchcard;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Modules.Inventory.Item
{
    public class Scratchcard : IItemScript
    {

        public int[] ItemId => new[] { 18 };

        public async Task<bool> OnItemUse(RPPlayer rpPlayer, LocalItem item)
        {
            MathUtils.RandomNumber(0, 1);
            rpPlayer.Emit("ShowIF", "Scratchcard", new ScratchcardWriter(MathUtils.RandomNumber(0, 1)));
            return true;
        }
    }
}

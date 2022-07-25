using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GangRP_Server.Core;
using GangRP_Server.Modules.Inventory;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Modules.Inventor
{
    public interface IItemScript
    {
        int[] ItemId { get; }
        Task<bool> OnItemUse(RPPlayer rpPlayer, LocalItem item);
    }
}

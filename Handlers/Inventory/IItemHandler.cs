using System.Threading.Tasks;
using GangRP_Server.Core;
using GangRP_Server.Modules.Inventory;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Handlers.Inventory
{
    public interface IItemHandler
    {
        Task<bool> TryUseItem(RPPlayer rpPlayer, LocalItem item);
    }
}
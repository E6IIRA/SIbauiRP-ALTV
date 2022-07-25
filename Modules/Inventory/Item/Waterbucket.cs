using System.Threading.Tasks;
using GangRP_Server.Core;
using GangRP_Server.Modules.Drug;
using GangRP_Server.Modules.Inventor;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Modules.Inventory.Item
{
    public class Waterbucket : IItemScript
    {
        public int[] ItemId => new[] { 8 };

        public async Task<bool> OnItemUse(RPPlayer rpPlayer, LocalItem item)
        {
            return await PlantModule.Instance.WaterPlant(rpPlayer);
        }
    }
}

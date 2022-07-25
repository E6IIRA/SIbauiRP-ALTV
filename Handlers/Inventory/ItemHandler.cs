using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GangRP_Server.Core;
using GangRP_Server.Events;
using GangRP_Server.Extensions;
using GangRP_Server.Modules.Inventor;
using GangRP_Server.Modules.Inventory;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Handlers.Inventory
{
    public class ItemHandler : IItemHandler, ILoadEvent
    {
        private readonly Dictionary<int, IItemScript> _scripts = new Dictionary<int, IItemScript>();
        private readonly IEnumerable<IItemScript> _itemScripts;

        public ItemHandler(IEnumerable<IItemScript> itemScripts)
        {
            _itemScripts = itemScripts;
        }

        public void OnLoad()
        {
            _itemScripts.ForEach(s =>
                    s.ItemId.ForEach(i => _scripts.Add(i, s)));
        }

        public async Task<bool> TryUseItem(RPPlayer rpPlayer, LocalItem item)
        {
            if (_scripts.TryGetValue(item.ItemId, out var script))
            {
                return await script.OnItemUse(rpPlayer, item);
            }

            return false;
        }
    }
}

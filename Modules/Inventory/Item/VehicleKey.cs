using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using GangRP_Server.Core;
using GangRP_Server.Models;
using GangRP_Server.Modules.Inventor;
using GangRP_Server.Modules.VehicleKey;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Modules.Inventory.Item
{
    public class VehicleKey : IItemScript
    {
        private readonly VehicleKeyModule _vehicleKeyModule;

        public int[] ItemId => new[] { 4 };

        public VehicleKey(VehicleKeyModule vehicleKeyModule)
        {
            _vehicleKeyModule = vehicleKeyModule;
        }

        public async Task<bool> OnItemUse(RPPlayer rpPlayer, LocalItem item)
        {
            return await _vehicleKeyModule.UseVehicleKey(rpPlayer, Convert.ToInt16(item.CustomItemData[1]), Convert.ToInt16(item.CustomItemData[2]));
        }
    }
}

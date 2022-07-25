using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using GangRP_Server.Core;
using GangRP_Server.Models;
using GangRP_Server.Modules.Door;
using GangRP_Server.Modules.Inventor;
using GangRP_Server.Modules.ServerScenario;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Modules.Inventory.Item
{
    public class WeldingDevice : IItemScript
    {
        private readonly DoorModule _doorModule;
        private readonly ServerScenarioModule _serverScenarioModule;

        public int[] ItemId => new[] { 12 };

        public WeldingDevice(DoorModule doorModule, ServerScenarioModule serverScenarioModule)
        {
            _doorModule = doorModule;
            _serverScenarioModule = serverScenarioModule;
        }

        public async Task<bool> OnItemUse(RPPlayer rpPlayer, LocalItem item)
        {
            bool foundObject = false;
            int timeInSeconds = 60;
            Door.Door door = _doorModule._doors.Values.FirstOrDefault(d => d.DoorData.Position.Distance(rpPlayer.Position) < 3);
            if (door != null)
            {
                foundObject = true;
                timeInSeconds = 60;
            }
            ServerScenarioData serverScenarioData =
                _serverScenarioModule.GetServerScenarioByPosition(rpPlayer.Position);
            if (serverScenarioData != null)
            {
                foundObject = true;
                timeInSeconds = 10;
            }

            if (!foundObject) return false;

            rpPlayer.PlayScenario("WORLD_HUMAN_WELDING");
            bool status = await rpPlayer.StartTask(timeInSeconds * 1000);

            if (status)
            {
                door = _doorModule._doors.Values.FirstOrDefault(d => d.DoorData.Position.Distance(rpPlayer.Position) < 3);
                if (door != null)
                {
                    door.DoorData.LastBreak = DateTime.Now;
                    if (door.DoorData.Locked)
                    {
                        _doorModule.ChangeDoorLockState(door.DoorData);
                    }

                }

                serverScenarioData =
                    _serverScenarioModule.GetServerScenarioByPosition(rpPlayer.Position);
                if (serverScenarioData != null && serverScenarioData.ServerScenarioLootData.Count > 0)
                {
                    _serverScenarioModule.StartScenario(serverScenarioData);
                }

            }

            rpPlayer.StopAnimation(true);

            return status;
        }
    }
}

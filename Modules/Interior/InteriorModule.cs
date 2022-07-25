using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AltV.Net.Data;
using GangRP_Server.Core;
using GangRP_Server.Events;
using GangRP_Server.Handlers.Logger;
using GangRP_Server.Models;
using GangRP_Server.Utilities;
using Microsoft.EntityFrameworkCore;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Modules.Interior
{

    public sealed class InteriorModule : ModuleBase, ILoadEvent
    {
        private readonly RPContext _rpContext;

        public Dictionary<int, InteriorData> _interiorDatas;
        public Dictionary<int, InteriorPositionData> _interiorPositionDatas;



        public InteriorModule(RPContext rpContext)
        {
            _rpContext = rpContext;
        }

        public void OnLoad()
        {
            _interiorDatas = _rpContext.InteriorData.Include(d => d.InteriorWarderobeData).ToDictionary(data => data.Id);
            _interiorPositionDatas = _rpContext.InteriorPositionData.ToDictionary(data => data.Id);
        }



        public InteriorData? GetInteriorDataById(int interiorDataId)
        {
            return _interiorDatas.TryGetValue(interiorDataId, out InteriorData interiorData) ? interiorData : null;
        }

        public Dictionary<int, InteriorPositionData> GetInteriorPositionDatasByInteriorId(int interiorDataId)
        {
            return _interiorPositionDatas.Values.Where(d => d.InteriorDataId == interiorDataId).ToDictionary(pair => pair.Id);
        }


        public InteriorPositionData GetInteriorPositionDataById(int InteriorPositionDataId)
        {
            if (_interiorPositionDatas.TryGetValue(InteriorPositionDataId, out InteriorPositionData interiorPositionData))
            {
                return interiorPositionData;
            }
            return null;
        }
    }
}

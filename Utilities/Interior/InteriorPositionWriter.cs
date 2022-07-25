using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using AltV.Net;
using GangRP_Server.Core;
using GangRP_Server.Models;
using GangRP_Server.Modules;
using GangRP_Server.Modules.Player;
using Microsoft.EntityFrameworkCore.Query;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Utilities.Interior
{
    public class InteriorPositionWriter : IWritable
    {
        private Dictionary<int, InteriorPositionData> _interiorPositionDatas;
        private ICollection<HouseInteriorPosition> _houseInteriorPosition;

        public InteriorPositionWriter(Dictionary<int, InteriorPositionData> interiorPositionDatas, ICollection<HouseInteriorPosition> houseInteriorPosition)
        {
            this._interiorPositionDatas = interiorPositionDatas;
            this._houseInteriorPosition = houseInteriorPosition;
        }


        public void OnWrite(IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("pos");
            writer.BeginArray();
            foreach (var value in _interiorPositionDatas.Values)
            {
                writer.BeginObject();
                writer.Name("i");
                writer.Value(value.Id);
                var houseInteriorPosition = _houseInteriorPosition.FirstOrDefault(d => d.InteriorPositionDataId == value.Id);
                if (houseInteriorPosition != null)
                {
                    writer.Name("a");
                    writer.Value(true);
                }
                writer.EndObject();
            }
            writer.EndArray();
            writer.EndObject();
        }
    }
}

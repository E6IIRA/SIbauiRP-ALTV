using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using AltV.Net;
using GangRP_Server.Models;
using GangRP_Server.Utilities.ClothProp;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using MySql.Data.MySqlClient;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Utilities.Cloth
{
    public class ClothShopDataWriter : IWritable
    {

        private readonly int _clothShopId;
        private readonly string _clothShopName;
        private readonly int _playerMoney;
        private readonly IEnumerable<ClothData> _clothData;

        public ClothShopDataWriter(int clothShopId, string clothShopName, int playerMoney, IEnumerable<ClothData> clothData)
        {
            this._clothShopId = clothShopId;
            this._clothShopName = clothShopName;
            this._playerMoney = playerMoney;
            this._clothData = clothData;
        }
        public void OnWrite(IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("i");
            writer.Value(_clothShopId);
            writer.Name("n");
            writer.Value(_clothShopName);
            writer.Name("m");
            writer.Value(_playerMoney);

            writer.Name("data");
            writer.BeginArray();
            Dictionary<byte, string> tempDictionary = new Dictionary<byte, string>();
            foreach (var clothData in _clothData.Where(d => d.ClothShopDataId == _clothShopId))
            {
                if (tempDictionary.ContainsKey(clothData.ClothTypeData.Value))continue;
                writer.BeginObject();
                writer.Name("i");
                writer.Value(clothData.ClothTypeData.Value);
                writer.Name("n");
                writer.Value(clothData.ClothTypeData.Name);
                writer.EndObject();

                tempDictionary.Add(clothData.ClothTypeData.Value, clothData.ClothTypeData.Name);
            }
            writer.EndArray();
            writer.EndObject();
        }
    }
}

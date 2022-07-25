using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using AltV.Net;
using GangRP_Server.Models;
using GangRP_Server.Utilities.ClothProp;
using Microsoft.EntityFrameworkCore;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Utilities.Cloth
{
    public class ClothDataWriter : IWritable
    {

        private readonly int _clothShopId;
        private readonly int _clothTypeId;
        private IEnumerable<ClothData> _clothData;
        private byte _gender;

        public ClothDataWriter(int clothShopId, int clothTypeId, IEnumerable<ClothData> clothData, byte gender)
        {
            this._clothShopId = clothShopId;
            this._clothTypeId = clothTypeId;
            this._clothData = clothData;
            this._gender = gender;
        }

        public void OnWrite(IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("values");
            writer.BeginArray();
            foreach (ClothData clothData in _clothData.Where(d => (d.ClothTypeDataId == _clothTypeId) && (d.ClothShopDataId == _clothShopId) && (d.Gender == _gender)))
            {
                writer.BeginObject();
                writer.Name("v");
                writer.Value(clothData.Value);
                writer.Name("n");
                writer.Value(clothData.Name);
                writer.Name("p");
                writer.Value(clothData.Price);
                writer.Name("values");
                writer.BeginArray();
                foreach (var variationData in clothData.ClothVariationData.OrderByDescending(d => d.Value))
                {
                    writer.BeginObject();
                    writer.Name("v");
                    writer.Value(variationData.Value);
                    writer.Name("n");
                    writer.Value(variationData.Name);
                    writer.EndObject();
                }
                writer.EndArray();
                writer.EndObject();

            }
            writer.EndArray();
            writer.EndObject();
        }
    }
}

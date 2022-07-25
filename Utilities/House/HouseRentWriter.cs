using System;
using System.Collections.Generic;
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
namespace GangRP_Server.Utilities.House
{
    public class HouseRentWriter : IWritable
    {
        private List<PlayerHouseRent> _temp;

        public HouseRentWriter(List<PlayerHouseRent> temp)
        {
            this._temp = temp;
        }


        public void OnWrite(IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("rents");
            writer.BeginArray();
            foreach (var value in _temp)
            {
                writer.BeginObject();
                writer.Name("i");
                writer.Value(value.Id);
                writer.Name("n");
                writer.Value(value.Player.Name);
                writer.Name("p");
                writer.Value(value.Cost);
                writer.Name("d");
                writer.Value(value.CreationDate.ToLongDateString() + " " + value.CreationDate.ToLongTimeString());
                writer.EndObject();
            }
            writer.EndArray();
            writer.EndObject();
        }
    }
}

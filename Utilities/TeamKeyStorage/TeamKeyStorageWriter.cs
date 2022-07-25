using System;
using System.Collections.Generic;
using System.Text;
using AltV.Net;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Utilities.TeamKeyStorage
{
    public class TeamKeyStorageWriter : IWritable
    {

        private readonly int _id;
        private readonly String _name;
        private readonly List<VehicleKeyInfo> _teamVehiclesKeyInfos;


        public TeamKeyStorageWriter(int id, String name, List<VehicleKeyInfo> teamVehiclesKeyInfos)
        {
            this._id = id;
            this._name = name;
            this._teamVehiclesKeyInfos = teamVehiclesKeyInfos;
        }

        public void OnWrite(IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("i");
            writer.Value(_id);
            writer.Name("n");
            writer.Value(_name);
            writer.Name("data");
            writer.BeginArray();
            foreach (var value in _teamVehiclesKeyInfos)
            {
                writer.BeginObject();
                writer.Name("i");
                writer.Value(value.Id);
                writer.Name("v");
                writer.Value(value.VehicleId);
                writer.Name("n");
                writer.Value(value.VehicleDataName);
                writer.Name("p");
                writer.Value(value.OwnerName);
                writer.Name("c");
                writer.Value(value.CreationDate.ToLongDateString() + " " + value.CreationDate.ToLongTimeString());
                writer.EndObject();
            }
            writer.EndArray();
            writer.EndObject();
        }


    }
}

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using AltV.Net;
using GangRP_Server.Core;
using GangRP_Server.Models;
using GangRP_Server.Modules;
using GangRP_Server.Modules.Crime;
using GangRP_Server.Modules.Player;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Utilities.Crime
{
    public class CrimePlayerTicketWriter : IWritable
    {
        private readonly CrimeModule _crimeModule;
        private RPPlayer _rpPlayer;

        public CrimePlayerTicketWriter(RPPlayer rpPlayer, CrimeModule crimeModule)
        {
            this._rpPlayer = rpPlayer;
            this._crimeModule = crimeModule;
        }


        public void OnWrite(IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("m");
            writer.Value(_rpPlayer.Money);
            writer.Name("crimes");
            writer.BeginArray();
            foreach (var crime in _rpPlayer.Crimes.Values)
            {
                var crimeData = _crimeModule.GetCrimeDataById(crime.CrimeDataId);
                writer.BeginObject();
                writer.Name("i");
                writer.Value(crime.Id);
                writer.Name("n");
                writer.Value(crimeData.Name);
                writer.Name("p");
                writer.Value(crimeData.Cost);
                writer.Name("d");
                writer.Value(crime.CreationDate.ToLongDateString() + " " + crime.CreationDate.ToLongTimeString());
                writer.EndObject();
            }
            writer.EndArray();
            writer.EndObject();
        }
    }
}

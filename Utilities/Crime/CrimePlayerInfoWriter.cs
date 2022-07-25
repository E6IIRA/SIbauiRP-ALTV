using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using AltV.Net;
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
    public class CrimePlayerInfoWriter : IWritable
    {

        private readonly PlayerOfflineInfo _playerInfo;
        private readonly PlayerLicence _playerLicence;
        private readonly Dictionary<int, PlayerCrime> _crimes;
        private readonly CrimeModule _crimeModule;

        public CrimePlayerInfoWriter(PlayerOfflineInfo playerInfo, PlayerLicence playerLicence, Dictionary<int, PlayerCrime> crimes, CrimeModule crimeModule)
        {
            this._playerInfo = playerInfo;
            this._playerLicence = playerLicence;
            this._crimes = crimes;
            this._crimeModule = crimeModule;
        }


        public void OnWrite(IMValueWriter writer)
        {
            Dictionary<int, int> playerLicences = new Dictionary<int, int>();
            if (_playerLicence.IdCard != 0) { playerLicences.Add(1, _playerLicence.IdCard); }
            if (_playerLicence.Car != 0) { playerLicences.Add(2, _playerLicence.Car); }
            if (_playerLicence.Truck != 0) { playerLicences.Add(3, _playerLicence.Truck); }
            if (_playerLicence.Motorcycle != 0) { playerLicences.Add(4, _playerLicence.Motorcycle); }
            if (_playerLicence.Boat != 0) { playerLicences.Add(5, _playerLicence.Boat); }
            if (_playerLicence.Helicopter != 0) { playerLicences.Add(6, _playerLicence.Helicopter); }
            if (_playerLicence.Plane != 0) { playerLicences.Add(7, _playerLicence.Plane); }
            writer.BeginObject();
            writer.Name("i");
            writer.Value(_playerInfo.playerId);
            writer.Name("n");
            writer.Value(_playerInfo.playerName);
            writer.Name("g");
            writer.Value(_playerInfo.gender);
            writer.Name("data");
            writer.BeginArray();
            foreach (var id in playerLicences)
            {
                writer.BeginObject();
                writer.Name("i");
                writer.Value(id.Key);
                writer.Name("v");
                writer.Value(id.Value);
                writer.EndObject();
            }
            writer.EndArray();

            writer.Name("crimes");
            writer.BeginArray();
            foreach (var crime in _crimes.Values)
            {
                writer.BeginObject();
                writer.Name("i");
                writer.Value(crime.CrimeDataId);
                var crimeData = _crimeModule.GetCrimeDataById(crime.CrimeDataId);
                writer.Name("n");
                writer.Value(crimeData.Name);
                writer.Name("j");
                writer.Value(crimeData.Jailtime);
                writer.Name("c");
                writer.Value(crimeData.Cost);
                writer.EndObject();
            }
            writer.EndArray();
            writer.EndObject();
        }
    }
}

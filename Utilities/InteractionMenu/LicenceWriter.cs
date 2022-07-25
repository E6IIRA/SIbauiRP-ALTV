using System;
using System.Collections.Generic;
using System.Text;
using AltV.Net;
using GangRP_Server.Models;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Utilities.InteractionMenu
{
    public class LicenceWriter : IWritable
    {
        private readonly PlayerLicence _playerLicence;
        public LicenceWriter(PlayerLicence playerLicence)
        {
            this._playerLicence = playerLicence;
        }

        public void OnWrite(IMValueWriter writer)
        {
            List<int> playerLicences = new List<int>();
            if (_playerLicence.IdCard == 1) { playerLicences.Add(1); }
            if (_playerLicence.Car == 1) { playerLicences.Add(2); }
            if (_playerLicence.Truck == 1) { playerLicences.Add(3); }
            if (_playerLicence.Motorcycle == 1) { playerLicences.Add(4); }
            if (_playerLicence.Boat == 1) { playerLicences.Add(5); }
            if (_playerLicence.Helicopter == 1) { playerLicences.Add(6); }
            if (_playerLicence.Plane == 1) { playerLicences.Add(7); }
            writer.BeginObject();
            writer.Name("data");
            writer.BeginArray();
            foreach (var id in playerLicences)
            {
                writer.BeginObject();
                writer.Name("i");
                writer.Value(id);
                writer.EndObject();
            }
            writer.EndArray();
            writer.EndObject();
        }
    }
}

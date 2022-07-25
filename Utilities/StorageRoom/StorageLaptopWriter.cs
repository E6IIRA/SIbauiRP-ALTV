using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using AltV.Net;
using GangRP_Server.Core;
using GangRP_Server.Models;
using GangRP_Server.Modules;
using GangRP_Server.Modules.Player;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Utilities.StorageRoom
{
    public class StorageLaptopWriter : IWritable
    {
        private readonly int _storageroomId;
        private readonly int _canControl;
        private readonly int _crates;
        private readonly int _typeUpgradeable;
        private readonly int _crateUpgradeable;

        public StorageLaptopWriter(int storageroomId, int canControl, int crates, int typeUpgradeable, int crateUpgradeable)
        {
            this._storageroomId = storageroomId;
            this._canControl = canControl;
            this._crates = crates;
            this._typeUpgradeable = typeUpgradeable;
            this._crateUpgradeable = crateUpgradeable;
        }


        public void OnWrite(IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("i");
            writer.Value(_storageroomId);
            writer.Name("o");
            writer.Value(_canControl);
            writer.Name("c");
            writer.Value(_crates);
            writer.Name("t");
            writer.Value(_typeUpgradeable);
            writer.Name("u");
            writer.Value(_crateUpgradeable);
            writer.EndObject();
        }
    }
}

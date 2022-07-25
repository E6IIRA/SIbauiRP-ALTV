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
namespace GangRP_Server.Utilities.House
{
    public class HouseInfoWriter : IWritable
    {
        private readonly int _houseId;
        private readonly int _canControl;
        private readonly String _doorBellSign;
        private readonly bool _locked;
        private int _money;
        private readonly bool _inside;

        public HouseInfoWriter(int houseId, int canControl, String doorBellSign, bool locked,int money = 0, bool inside = false)
        {
            this._houseId = houseId;
            this._canControl = canControl;
            this._doorBellSign = doorBellSign;
            this._locked = locked;
            this._money = money;
            this._inside = inside;
        }


        public void OnWrite(IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("i");
            writer.Value(_houseId);
            writer.Name("n");
            writer.Value(_doorBellSign);
            writer.Name("o");
            writer.Value(_canControl);
            writer.Name("m");
            writer.Value(_money);
            writer.Name("l");
            writer.Value(_locked);
            writer.Name("in");
            writer.Value(_inside);
            writer.EndObject();
        }
    }
}

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
namespace GangRP_Server.Utilities.Drug
{
    public class DrugCamperWriter : IWritable
    {
        private readonly int _state;
        private readonly float _temperature;
        private readonly float _humidity;
        private readonly int _ventilation;
        private readonly int _dildogroeße;

        public DrugCamperWriter(int state, float temperature, float humidity, int ventilation, int dildogroeße)
        {
            this._state = state;
            _temperature = temperature;
            _humidity = humidity;
            _ventilation = ventilation;
            _dildogroeße = dildogroeße;
        }


        public void OnWrite(IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("s");
            writer.Value(_state);
            writer.Name("t");
            writer.Value(_temperature);
            writer.Name("h");
            writer.Value(_humidity);
            writer.Name("v");
            writer.Value(_ventilation);
            writer.Name("d");
            writer.Value(_dildogroeße);
            writer.EndObject();
        }
    }
}
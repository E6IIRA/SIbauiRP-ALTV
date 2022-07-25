using System.Collections.Generic;
using AltV.Net;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Utilities.SpeedCam
{
    public class SpeedCamItemWriter : IWritable
    {
        private Modules.SpeedCam.SpeedCam _speedCam;

        public SpeedCamItemWriter(Modules.SpeedCam.SpeedCam speedCam)
        {
            this._speedCam = speedCam;
        }


        public void OnWrite(IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("i");
            writer.Value(_speedCam.camStationId);
            writer.Name("items");
            writer.BeginArray();
            foreach (var value in _speedCam.SpeedCamItems)
            {
                writer.BeginObject();
                writer.Name("s");
                writer.Value(value.speed);
                writer.Name("l");
                writer.Value(value.licensePlate);
                writer.EndObject();
            }
            writer.EndArray();
            writer.EndObject();
        }
    }
}

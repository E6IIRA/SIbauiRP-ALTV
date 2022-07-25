using System.Collections.Generic;
using AltV.Net;
using GangRP_Server.Modules.SpeedCam;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Utilities.SpeedCam
{
    public class SpeedCamItemAddWriter : IWritable
    {
        private readonly SpeedCamItem _speedCamItem;

        public SpeedCamItemAddWriter(SpeedCamItem speedCamItem)
        {
            this._speedCamItem = speedCamItem;
        }


        public void OnWrite(IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("s");
            writer.Value(_speedCamItem.speed);
            writer.Name("l");
            writer.Value(_speedCamItem.licensePlate);
            writer.EndObject();
        }
    }
}

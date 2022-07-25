using System;
using System.Collections.Generic;
using System.Text;
using AltV.Net.Elements.Entities;
using GangRP_Server.Core;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Modules.SpeedCam
{
    public class SpeedCam
    {
        public int camStationId { get; set; }
        public IColShape colShape { get; set; }

        public List<int> camStationPlayers { get; set; }

        public RPVehicle camStationVehicle { get; set; }

        public List<SpeedCamItem> SpeedCamItems { get; set; }

        public SpeedCam(int camStationId, IColShape colShape, RPPlayer rpPlayer, RPVehicle rpVehicle)
        {
            this.camStationId = camStationId;
            this.colShape = colShape;
            this.camStationPlayers = new List<int>();
            this.camStationVehicle = rpVehicle;
            this.SpeedCamItems = new List<SpeedCamItem>();
        }
    }

    public class SpeedCamItem
    {
        public int speed { get; set; }
        public String licensePlate { get; set; }

        public SpeedCamItem(int speed, String licensePlate)
        {
            this.speed = speed;
            this.licensePlate = licensePlate;
        }
    }


}

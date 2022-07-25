using System;
using System.Collections.Generic;
using System.Text;
using GangRP_Server.Handlers.Vehicle;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Commands
{
    public class DebugCommands
    {
#if DEBUG
        //[Command]
        //public void veh(Client client)
        //{
        //    client.SendNotification("spawn veh");

        //    VehicleHandler.Instance.CreateVehicle(850991848, client.Position, client.Heading, 1, 1);
        //}

        //[Command]
        //public void veh2(Client client)
        //{
        //    client.SendNotification("spawn veh2");

        //    NAPI.Task.Run(() =>
        //    {
        //        NAPI.Vehicle.CreateVehicle(850991848, client.Position, client.Heading, 1, 1);
        //    });
        //}

        //[Command]
        //public void veh3(Client client)
        //{
        //    client.SendNotification("spawn veh3");

        //    NAPI.Task.Run(() =>
        //    {
        //        NAPI.Vehicle.CreateVehicle(-1216765807, client.Position, client.Heading, 1, 1);
        //    });
        //}

#endif
    }
}

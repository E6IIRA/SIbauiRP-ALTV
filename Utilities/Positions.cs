using System;
using System.Collections.Generic;
using System.Text;
using AltV.Net.Data;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Utilities
{
    public class Positions
    {

        public static Position JailOutsidePosition = new Position(1846.11f, 2586.04f, 45.6578f);

        public static Position JailInputPosition = new Position(1690.81f, 2591.42f, 45.91f);


        public static Position JailInsidePosition = new Position(1691.62f, 2565.84f, 45.5568f);

        public static Position JailProbationLeavePosition = new Position(1775.64f, 2551.9f, 45.55f);

        public static Position CamperInputPosition = new Position(1975.49f, 3818.35f, 33.4249f);
        public static Position CamperOutputPosition = new Position(1976.22f, 3820.84f, 33.4418f);
        public static Position CamperDoorPosition = new Position(1972.96f, 3816.33f, 33.5f);

        public static Position WareExportPosition = new Position(1242.75f, -3113.75f, 6.0271f);
        public static Position WareExportInventoryPosition = new Position(1247.34f, -3114.41f, 5.0471f);
    }
}

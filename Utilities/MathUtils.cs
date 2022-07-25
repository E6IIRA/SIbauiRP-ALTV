using System;
using System.Collections.Generic;
using System.Text;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Utilities
{
    public class MathUtils
    {
        private static Random _Random = new Random();
        public static int RandomNumber(int min, int max)
        {
            return _Random.Next(min, max + 1);
        }
        public static float RandomChance(int decimals = 2)
        {
            //Nur bis Nachkommastellen = 5 getestet. Höhere Werte können Probleme bringen, da irgendwann das erste Math.Pow(...) außerhalb des Speicherbereiches von int läuft.
            return _Random.Next(0, (int)Math.Pow(10, decimals + 2)) / (float)Math.Pow(10, decimals);
        }
    }
}

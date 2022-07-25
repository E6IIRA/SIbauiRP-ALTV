using System.Numerics;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Utilities.Blip
{
    public class BlipData
    {
        public Vector3 Position;
        public string Name;

        public BlipData(Vector3 position, string name)
        {
            this.Position = position;
            this.Name = name;
        }
    }
}

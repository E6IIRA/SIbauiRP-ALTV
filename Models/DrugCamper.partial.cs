using System;
using System.Collections.Generic;
using System.Text;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class DrugCamper
    {
        public Dictionary<int, int> NeededItems = new Dictionary<int, int>();
        public Dictionary<int, (int amount, string[]? customData)> OutputItems = new Dictionary<int, (int amount, string[]? customData)>();
    }
}

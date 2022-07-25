using System;
using System.Collections.Generic;
using System.Text;
using AltV.Net.Data;
using GangRP_Server.Utilities;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class House
    {
        public bool Locked = true;
        public bool ChangeHideStatus = false;

        public List<PlayerLabel> HideLabels = new List<PlayerLabel>();

    }
}

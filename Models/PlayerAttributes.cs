using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class PlayerAttributes
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int Level { get; set; }
        public int Experience { get; set; }
        public int Strength { get; set; }
        public int Vitality { get; set; }
        public int Dexterity { get; set; }
        public int Intelligence { get; set; }
        public int MaximumAttributes { get; set; }

        public virtual Player Player { get; set; }
    }
}

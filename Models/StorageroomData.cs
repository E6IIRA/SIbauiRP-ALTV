using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class StorageroomData
    {
        public StorageroomData()
        {
            Storageroom = new HashSet<Storageroom>();
        }

        public int Id { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }

        public virtual ICollection<Storageroom> Storageroom { get; set; }
    }
}

using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class PlayerCrime
    {
        public int Id { get; set; }
        public int CrimeDataId { get; set; }
        public int PlayerId { get; set; }
        public int OfficerId { get; set; }
        public DateTime CreationDate { get; set; }

        public virtual CrimeData CrimeData { get; set; }
        public virtual Player Officer { get; set; }
        public virtual Player Player { get; set; }
    }
}

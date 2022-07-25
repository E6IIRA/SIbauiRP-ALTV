using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class SmsChatParticipant
    {
        public SmsChatParticipant()
        {
            SmsChatMessage = new HashSet<SmsChatMessage>();
        }

        public int Id { get; set; }
        public int SmsChatId { get; set; }
        public int Number { get; set; }

        public virtual SmsChat SmsChat { get; set; }
        public virtual ICollection<SmsChatMessage> SmsChatMessage { get; set; }
    }
}

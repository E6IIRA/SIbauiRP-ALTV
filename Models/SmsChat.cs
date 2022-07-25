using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class SmsChat
    {
        public SmsChat()
        {
            SmsChatParticipant = new HashSet<SmsChatParticipant>();
        }

        public int Id { get; set; }

        public virtual ICollection<SmsChatParticipant> SmsChatParticipant { get; set; }
    }
}

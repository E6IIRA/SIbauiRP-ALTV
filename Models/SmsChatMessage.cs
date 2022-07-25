using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class SmsChatMessage
    {
        public int Id { get; set; }
        public int SmsChatParticipantId { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }

        public virtual SmsChatParticipant SmsChatParticipant { get; set; }
    }
}

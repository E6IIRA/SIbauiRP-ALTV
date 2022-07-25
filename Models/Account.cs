using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class Account
    {
        public Account()
        {
            Player = new HashSet<Player>();
        }

        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public DateTime Creationdate { get; set; }
        public int RankId { get; set; }
        public string SocialClubName { get; set; }

        public virtual Rank Rank { get; set; }
        public virtual ICollection<Player> Player { get; set; }
    }
}

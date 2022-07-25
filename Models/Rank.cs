using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class Rank
    {
        public Rank()
        {
            Account = new HashSet<Account>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int Power { get; set; }
        public int Payday { get; set; }

        public virtual ICollection<Account> Account { get; set; }
    }
}

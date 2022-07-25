using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class Bank
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }
        public int BankTypeId { get; set; }
        public int? CurrentMoney { get; set; }
        public int? MaxMoney { get; set; }

        public virtual Banktype BankType { get; set; }
    }
}

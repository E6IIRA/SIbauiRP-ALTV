using System;
using System.Collections.Generic;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Models
{
    public partial class BankTypeData
    {
        public BankTypeData()
        {
            BankData = new HashSet<BankData>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public float WithdrawFee { get; set; }
        public float AccountFee { get; set; }
        public float DepositFee { get; set; }
        public int WithdrawFeeMinimum { get; set; }
        public int DepositFeeMinimum { get; set; }
        public int WithdrawFeeMaximum { get; set; }
        public int AccountFeeMaximum { get; set; }
        public int DepositFeeMaximum { get; set; }

        public virtual ICollection<BankData> BankData { get; set; }
    }
}

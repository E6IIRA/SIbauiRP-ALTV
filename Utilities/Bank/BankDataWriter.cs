using System.Collections.Generic;
using AltV.Net;
using GangRP_Server.Utilities.ClothProp;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Utilities.Bank
{
    public class BankDataWriter : IWritable
    {
        private readonly int _bankId;
        private readonly string _bankName;

        private readonly int _money;
        private readonly int _bankMoney;

        private readonly float _withdrawFeePer;
        private readonly float _depositFeePer;
        private readonly int _withdrawFeeMin;
        private readonly int _depositFeeMin;
        private readonly int _withdrawFeeMax;
        private readonly int _depositFeeMax;

        public BankDataWriter(int bankId, string bankName, int money, int bankMoney, int withdrawFeeMin, float withdrawFeePer, int withdrawFeeMax, int depositFeeMin, float depositFeePer, int depositFeeMax)
        {
            this._bankId = bankId;
            this._bankName = bankName;
            this._money = money;
            this._bankMoney = bankMoney;
            this._withdrawFeePer = withdrawFeePer;
            this._depositFeePer = depositFeePer;
            this._withdrawFeeMin = withdrawFeeMin;
            this._depositFeeMin = depositFeeMin;
            this._withdrawFeeMax = withdrawFeeMax;
            this._depositFeeMax = depositFeeMax;
        }

        public void OnWrite(IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("i");
            writer.Value(_bankId);
            writer.Name("n");
            writer.Value(_bankName);
            writer.Name("m");
            writer.Value(_money);
            writer.Name("b");
            writer.Value(_bankMoney);
            writer.Name("wp");
            writer.Value(_withdrawFeePer);
            writer.Name("dp");
            writer.Value(_depositFeePer);
            writer.Name("wm");
            writer.Value(_withdrawFeeMin);
            writer.Name("dm");
            writer.Value(_depositFeeMin);
            writer.Name("wx");
            writer.Value(_withdrawFeeMax);
            writer.Name("dx");
            writer.Value(_depositFeeMax);


            //TODO BankHistory amk


            //writer.Name("data");
            //writer.BeginArray();
            //    foreach (var value in _garageVehicles)
            //    {
            //        writer.BeginObject();
            //            writer.Name("i");
            //            writer.Value(value.VehicleId);
            //            writer.Name("n");
            //            writer.Value(value.Name);
            //        writer.EndObject();
            //    }
            //    writer.EndArray();
            writer.EndObject();
        }
    }
}

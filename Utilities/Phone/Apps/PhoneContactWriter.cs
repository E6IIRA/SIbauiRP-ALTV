using System.Collections.Generic;
using AltV.Net;
using GangRP_Server.Models;
using GangRP_Server.Utilities.ClothProp;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Utilities.Phone.Apps
{
    public class PhoneContactWriter : IWritable
    {
        private readonly IEnumerable<PlayerPhoneContact> _phoneContacts;


        public PhoneContactWriter(IEnumerable<PlayerPhoneContact> phoneContacts)
        {
            _phoneContacts = phoneContacts;
        }

        public void OnWrite(IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("d");
            writer.BeginArray();
                foreach (var value in _phoneContacts)
                {
                    writer.BeginObject();
                        writer.Name("i");
                        writer.Value(value.Number);
                        writer.Name("n");
                        writer.Value(value.Name);
                        writer.Name("t");
                        writer.Value(value.Note);
                    writer.EndObject();
                }
                writer.EndArray();
            writer.EndObject();
        }
    }
}

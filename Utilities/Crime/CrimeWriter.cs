using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AltV.Net;
using GangRP_Server.Models;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Utilities.Crime
{
    public class CrimeWriter : IWritable
    {
        public Dictionary<int, CrimeData> _crimeData;
        public Dictionary<int, CrimeCategoryData> _crimeCategoryData;


        public CrimeWriter(Dictionary<int, CrimeData> crimeData, Dictionary<int, CrimeCategoryData> crimeCategoryData)
        {
            this._crimeCategoryData = crimeCategoryData;
            this._crimeData = crimeData;
        }

        public void OnWrite(IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("values");
            writer.BeginArray();
            foreach (var category in _crimeCategoryData.Values)
            {
                writer.BeginObject();
                writer.Name("i");
                writer.Value(category.Id);
                writer.Name("n");
                writer.Value(category.Name);
                writer.Name("values");
                writer.BeginArray();
                foreach (var value in _crimeData.Values.Where(d => d.CrimeCategoryDataId == category.Id))
                {
                    writer.BeginObject();
                    writer.Name("i");
                    writer.Value(value.Id);
                    writer.Name("n");
                    writer.Value(value.Name);
                    writer.Name("p");
                    writer.Value(value.Cost);
                    writer.Name("j");
                    writer.Value(value.Jailtime);
                    writer.EndObject();
                }
                writer.EndArray();
                writer.EndObject();
            }
            writer.EndArray();
            writer.EndObject();

        }


    }
}

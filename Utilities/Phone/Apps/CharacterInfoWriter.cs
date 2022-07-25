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
    public class CharacterInfoWriter : IWritable
    {
        private readonly string _name;
        private readonly int _level;
        private readonly int _experience;
        private readonly int _strength;
        private readonly int _vitality;
        private readonly int _dexterity;
        private readonly int _intelligence;



        public CharacterInfoWriter(string name, int level, int experience, int strength, int vitality, int dexterity, int intelligence)
        {
            _name = name;
            _level = level;
            _experience = experience;
            _strength = strength;
            _vitality = vitality;
            _dexterity = dexterity;
            _intelligence = intelligence;
        }

        public void OnWrite(IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("n");
            writer.Value(_name);
            writer.Name("l");
            writer.Value(_level);
            writer.Name("e");
            writer.Value(_experience);
            writer.Name("s");
            writer.Value(_strength);
            writer.Name("v");
            writer.Value(_vitality);
            writer.Name("d");
            writer.Value(_dexterity);
            writer.Name("i");
            writer.Value(_intelligence);
            writer.EndObject();
        }
    }
}
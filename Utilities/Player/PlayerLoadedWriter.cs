using System;
using System.Collections.Generic;
using System.Text;
using AltV.Net;
using GangRP_Server.Core;
using GangRP_Server.Models;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Utilities.Player
{
    public class PlayerLoadedWriter : IWritable
    {
        private readonly RPPlayer _rpPlayer;
        public PlayerLoadedWriter(RPPlayer rpPlayer)
        {
            this._rpPlayer = rpPlayer;
        }

        public void OnWrite(IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("name");
            writer.Value(_rpPlayer.Name);
            writer.Name("isCuffed");
            writer.Value(_rpPlayer.IsCuffed);
            writer.Name("isTied");
            writer.Value(_rpPlayer.IsTied);
            writer.Name("jail");
            writer.Value(_rpPlayer.JailTime);
            writer.Name("maxHealth");
            writer.Value(_rpPlayer.MaximumHealth);
            writer.Name("maxArmor");
            writer.Value(_rpPlayer.MaximumArmor);
            writer.Name("cloth");
            writer.BeginArray();
            foreach (var value in _rpPlayer.EquippedClothes)
            {
                writer.BeginObject();
                writer.Name("c");
                writer.Value(value.Value.component);
                writer.Name("d");
                writer.Value(value.Value.drawable);
                writer.Name("t");
                writer.Value(value.Value.texture);
                writer.EndObject();
            }
            writer.EndArray();
            writer.EndObject();
        }
    }
}

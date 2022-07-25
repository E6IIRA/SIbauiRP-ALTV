using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using GangRP_Server.Core;
using GangRP_Server.Events;
using GangRP_Server.Handlers.Logger;
using GangRP_Server.Models;
using GangRP_Server.Modules.Injury;
using GangRP_Server.Utilities;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Handlers.Player
{
    public class PlayerDeadHandler : IPlayerDeadHandler, IPlayerDeadEvent
    {
        private readonly ILogger _logger;
        private readonly InjuryModule _injuryModule;
        public PlayerDeadHandler(ILogger logger, InjuryModule injuryModule)
        {
            _logger = logger;
            _injuryModule = injuryModule;
        }


        public async void OnPlayerDead(IPlayer player, IEntity killer, uint weapon)
        {
            //This is because of /sethp 0 on join
            if (weapon == 0) return;
            RPPlayer rpPlayer = (RPPlayer) player;
            //check if player is already injured
            if (rpPlayer.InjuryStatus != null) return;





            InjuryTypeData injuryTypeData = _injuryModule.GetRandomInjuryTypeDataByDeathCause(weapon);


            int lower = injuryTypeData.Time - (int) (injuryTypeData.Time * 0.2);
            int higher = injuryTypeData.Time + (int)(injuryTypeData.Time * 0.2);
            int timeLeft = MathUtils.RandomNumber(lower, higher);

            _logger.Info($"timeLeft new {timeLeft} min{lower} high {higher} original {injuryTypeData.Time}");


            rpPlayer.InjuryStatus = new InjuryStatus(injuryTypeData.Id, timeLeft,injuryTypeData.TreatmentType);

            rpPlayer.SetBlurIn(10000);





            if (killer is IPlayer killerPlayer)
            {
                RPPlayer killerRpPlayer = (RPPlayer)killerPlayer;
            }
        }
    }
}

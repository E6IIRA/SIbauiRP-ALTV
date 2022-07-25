using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltV.Net.Elements.Entities;
using Autofac.Core.Activators;
using GangRP_Server.Core;
using GangRP_Server.Events;
using GangRP_Server.Handlers.Logger;
using GangRP_Server.Handlers.Player;
using GangRP_Server.Models;
using GangRP_Server.Modules.Garage;
using GangRP_Server.Modules.Player;
using GangRP_Server.Modules.VehicleData;
using GangRP_Server.Utilities;
using GangRP_Server.Utilities.Crime;
using GangRP_Server.Utilities.Injury;
using GangRP_Server.Utilities.InteractionMenu;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Modules.Injury
{
    public enum TreatmentType {
        SELF = 1,
        PLAYER = 2,
        MEDIC = 3,
        DIE = 4
    }


    public class InjuryStatus
    {
        public int InjuryTypeDataId { get; set; }
        public bool Stabilized { get; set; }
        public bool IdentifiedInjury { get; set; }
        public int TimeLeft { get; set; }

        public TreatmentType TreatmentType { get; set; }
        public InjuryStatus(int injuryTypeDataId, int timeLeft, int treatmentTypeId, bool stabilized = false)
        {
            this.InjuryTypeDataId = injuryTypeDataId;
            this.TimeLeft = timeLeft;
            this.TreatmentType = (TreatmentType) treatmentTypeId;
            this.Stabilized = stabilized;
            this.IdentifiedInjury = false;
        }
    }



    public sealed class InjuryModule : ModuleBase
    {
        private readonly ILogger _logger;
        private readonly RPContext _rpContext;
        private readonly IPlayerHandler _playerHandler;
        private readonly InjuryDataModule _injuryDataModule;

        public static InjuryModule Instance { get; private set; }




        public InjuryModule(ILogger logger, RPContext rpContext, IPlayerHandler playerHandler, InjuryDataModule injuryDataModule)
        {
            _logger = logger;
            _rpContext = rpContext;
            _playerHandler = playerHandler;
            _injuryDataModule = injuryDataModule;
            AddClientEvent<int>("IdentifyInjury", IdentifyInjury);
            AddClientEvent<int>("HelpInjury", HelpInjury);
            AddClientEvent<int>("StabilizeInjury", StabilizeInjury);
            Instance = this;

        }

        public InjuryTypeData GetRandomInjuryTypeDataByDeathCause(uint deathCause)
        {
            Dictionary<int, InjuryTypeData> injuryTypeDatas = _injuryDataModule.GetValues().Where(d => d.InjuryDeathCauseData.Hash == deathCause).ToDictionary(data => data.Id);
            int max = injuryTypeDatas.Values.Sum(d => d.Percentage);
            _logger.Info("max: " + max);
            int randomNumber = MathUtils.RandomNumber(1, max);
            _logger.Info("randomNumber: " + randomNumber);
            int actual = 0;
            foreach (var injuryTypeData in injuryTypeDatas.Values.OrderBy(d => d.Percentage))
            {
                actual += injuryTypeData.Percentage;
                if (randomNumber < actual)
                {
                    _logger.Info($"type {injuryTypeData.Id}:{injuryTypeData.Name} actual {actual}" );
                    return injuryTypeData;
                }
            }
            return null;
        }

        public async void IdentifyInjury(IPlayer player, int targetPlayerId)
        {
            RPPlayer rpPlayer = (RPPlayer) player;

            RPPlayer? targetRpPlayer = _playerHandler.GetOnlineRPPlayerByPlayerId(targetPlayerId);

            if (targetRpPlayer?.InjuryStatus != null)
            {
                if (!targetRpPlayer.InjuryStatus.IdentifiedInjury)
                {
                    rpPlayer.PlayAnimation(Animation.KNEEL);
                    bool status = await rpPlayer.StartTask(30 * 1000);

                    if (status)
                    {
                        if (targetRpPlayer?.InjuryStatus != null)
                        {
                            targetRpPlayer.InjuryStatus.IdentifiedInjury = true;
                            InjuryTypeData injuryTypeData = _injuryDataModule.GetById(targetRpPlayer.InjuryStatus.InjuryTypeDataId);
                            String injuryName = injuryTypeData.Name;
                            rpPlayer.Emit("UpdateView", "UpdateInjuryIdentifier", injuryName, injuryTypeData.TreatmentType);
                        }
                    }
                    rpPlayer.StopAnimation(true);
                }
            }
        }

        public async void HelpInjury(IPlayer player, int targetPlayerId)
        {
            RPPlayer rpPlayer = (RPPlayer)player;

            RPPlayer? targetRpPlayer = _playerHandler.GetOnlineRPPlayerByPlayerId(targetPlayerId);

            if (targetRpPlayer?.InjuryStatus != null)
            {
                if (targetRpPlayer.InjuryStatus.IdentifiedInjury && (targetRpPlayer.InjuryStatus.TreatmentType == TreatmentType.SELF || targetRpPlayer.InjuryStatus.TreatmentType == TreatmentType.PLAYER))
                {
                    rpPlayer.PlayAnimation(Animation.KNEEL);
                    bool status = await rpPlayer.StartTask(30 * 1000);

                    if (status)
                    {
                        if (targetRpPlayer?.InjuryStatus != null)
                        {
                            targetRpPlayer.InjuryStatus = null;
                            targetRpPlayer.Revive();
                            
                        }
                        rpPlayer.Emit("UpdateView", "FinishInjury");

                    }
                    rpPlayer.StopAnimation(true);
                }
            }
        }

        public async void StabilizeInjury(IPlayer player, int targetPlayerId)
        {
            RPPlayer rpPlayer = (RPPlayer)player;

            RPPlayer? targetRpPlayer = _playerHandler.GetOnlineRPPlayerByPlayerId(targetPlayerId);

            if (targetRpPlayer?.InjuryStatus != null)
            {
                if (targetRpPlayer.InjuryStatus.IdentifiedInjury && !targetRpPlayer.InjuryStatus.Stabilized)
                {
                    rpPlayer.PlayAnimation(Animation.KNEEL);
                    bool status = await rpPlayer.StartTask(30 * 1000);

                    if (status)
                    {
                        if (targetRpPlayer.InjuryStatus != null && !targetRpPlayer.InjuryStatus.Stabilized)
                        {
                            targetRpPlayer.InjuryStatus.Stabilized = true;
                            targetRpPlayer.InjuryStatus.TimeLeft += _injuryDataModule.GetById(targetRpPlayer.InjuryStatus.InjuryTypeDataId).AdditionalTime;
                        }
                        rpPlayer.Emit("UpdateView", "UpdateInjuryStabilized");

                    }
                    rpPlayer.StopAnimation(true);
                }
            }
        }

        public void OpenFirstAidWindow(RPPlayer rpPlayer, RPPlayer targetRpPlayer)
        {
            if (targetRpPlayer.InjuryStatus == null) return;
            InjuryTypeData injuryTypeData = _injuryDataModule.GetById(targetRpPlayer.InjuryStatus.InjuryTypeDataId);

            string injuryName = "Verletzung nicht identifiziert";
            int status = 0;
            if (targetRpPlayer.InjuryStatus.IdentifiedInjury)
            {
                injuryName = injuryTypeData.Name;
                status = injuryTypeData.TreatmentType;
            }
            rpPlayer.Emit("ShowIF", "Injury", new InjuryWriter(targetRpPlayer.PlayerId, injuryName, status, targetRpPlayer.InjuryStatus.Stabilized));
        }
    }
}

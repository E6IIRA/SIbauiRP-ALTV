using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using GangRP_Server.Core;
using GangRP_Server.Events;
using GangRP_Server.Models;
using GangRP_Server.Utilities;
using GangRP_Server.Utilities.Confirm;
using GangRP_Server.Utilities.Crime;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Modules.Crime
{
    public sealed class CrimeModule : ModuleBase, ILoadEvent, IPressedEEvent
    {
        private readonly RPContext _rpContext;

        public Dictionary<int, CrimeData> _crimeData;
        public Dictionary<int, CrimeCategoryData> _crimeCategoryData;

        public List<TicketMachineData> _ticketMachines;


        public CrimeModule(RPContext rpContext)
        {
            _rpContext = rpContext;
            AddClientEvent<int>("PayForCrime", PayForCrime);
            AddClientEvent("RequestProbation", RequestProbation);
        }


        public void OnLoad()
        {
            _crimeCategoryData = new Dictionary<int, CrimeCategoryData>();
            foreach (var crimeCategoryData in _rpContext.CrimeCategoryData)
            {
                _crimeCategoryData.Add(crimeCategoryData.Id, crimeCategoryData);
            }
            _crimeData = new Dictionary<int, CrimeData>();
            foreach (var crimeData in _rpContext.CrimeData)
            {
                _crimeData.Add(crimeData.Id, crimeData);
            }

            _ticketMachines = _rpContext.TicketMachineData.ToList();
        }


        async void PayForCrime(IPlayer player, int playerCrimeId)
        {
            RPPlayer rpPlayer = (RPPlayer) player;

            if (rpPlayer.Crimes.TryGetValue(playerCrimeId, out PlayerCrime playerCrime))
            {
                var crimeData = GetCrimeDataById(playerCrime.CrimeDataId);
                if (rpPlayer.Money >= crimeData.Cost)
                {
                    await rpPlayer.TakeMoney(crimeData.Cost);
                    await RemoveCrimeFromPlayer(rpPlayer, playerCrime);
                }
            }
        }

        async void RequestProbation(IPlayer player)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            if (rpPlayer.JailTime > 0)
            {
                if (rpPlayer.ProbationTime == 0)
                {
                    int cost = GetProbationCost(rpPlayer.JailTime);

                    if (rpPlayer.BankMoney >= cost)
                    {
                        if (await rpPlayer.TakeBankMoney(cost))
                        {
                            rpPlayer.JailTime = 0;
                            rpPlayer.SendNotification($"Deine Bewährung wurde angenommen. Du kannst das Staatsgefängnis verlassen. Die Kaution beträgt {cost} $", RPPlayer.NotificationType.INFO, "Staatsgefängnis", 20000);
                        }
                    }
                }
            }
        }

        public CrimeCategoryData GetCrimeCategoryDataById(int crimeCategoryId)
        {
            if (_crimeCategoryData.TryGetValue(crimeCategoryId, out CrimeCategoryData crimeCategoryData)) 
                return crimeCategoryData;

            return null;
        }

        public CrimeData GetCrimeDataById(int crimeDataId)
        {
            if (_crimeData.TryGetValue(crimeDataId, out CrimeData crimeData))
                return crimeData;

            return null;
        }

        public async Task RemoveCrimeFromPlayer(RPPlayer rpPlayer, PlayerCrime playerCrime)
        {
            rpPlayer.Crimes.Remove(playerCrime.Id);
            await using RPContext rpContext = new RPContext();
            rpContext.PlayerCrime.Remove(playerCrime);
            await rpContext.SaveChangesAsync();
        }


        public async Task AddCrimesToPlayer(List<PlayerCrime> playerCrimes)
        {
            await using RPContext rpContext = new RPContext();
            await rpContext.PlayerCrime.AddRangeAsync(playerCrimes);
            await rpContext.SaveChangesAsync();

        }

        public Task<bool> OnPressedE(IPlayer player)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            if (rpPlayer.Position.Distance(Positions.JailProbationLeavePosition) < 3)
            {
                if (rpPlayer.JailTime <= 0)
                {
                    //player can leave jail
                    UnJail(rpPlayer);
                }
                else
                {
                    //player has jailtime, show probationWindow amk
                    rpPlayer.Emit("ShowIF", "Confirm", new ConfirmWriter($"Haftzeit: {rpPlayer.JailTime} \n Bewährung würde {GetProbationCost(rpPlayer.JailTime)} $ kosten", "Bewährung beantragen", "Abbrechen", "RequestProbation"));
                }


            }
            else
            {

                var ticketMachineData = _ticketMachines.FirstOrDefault(d => d.Position.Distance(rpPlayer.Position) < 3);
                if (ticketMachineData != null)
                {
                    var jailData = GetJailtimeAndCost(rpPlayer);

                    if (jailData.Cost >= 30000 || jailData.Time > 0)
                    {
                        rpPlayer.Emit("ShowIF", "TicketMachine");
                        return Task.FromResult(true);
                    }
                    else
                    {
                        rpPlayer.Emit("ShowIF", "TicketMachine", new CrimePlayerTicketWriter(rpPlayer, this));
                        return Task.FromResult(true);
                    }
                }
            }





            return Task.FromResult(false);
        }

        public (int Cost, int Time) GetJailtimeAndCost(RPPlayer rpPlayer)
        {
            int cost = 0;
            int time = 0;
            foreach (var crime in rpPlayer.Crimes)
            {
                var crimeData = GetCrimeDataById(crime.Value.CrimeDataId);
                cost += crimeData.Cost;
                time += crimeData.Jailtime;
            }
            return (cost, time);
        }

        public async void Jail(RPPlayer rpPlayer, RPPlayer targetRpPlayer)
        {
            var jailData = GetJailtimeAndCost(targetRpPlayer);
            if (jailData.Time == 0) return;
            await targetRpPlayer.TakeMoney(jailData.Cost);
            targetRpPlayer.JailTime = jailData.Time;
            await targetRpPlayer.SetPositionAsync(Positions.JailInsidePosition);
            targetRpPlayer.SetJailClothes();
            //TODO Message for Team etc.
            rpPlayer.SendNotification($"Du hast {targetRpPlayer.Name} inhaftiert.", RPPlayer.NotificationType.SUCCESS, "Staatsgefängnis");
            targetRpPlayer.SendNotification($"Du wurdest für {jailData.Time} Einheiten inhaftiert. Strafe: {jailData.Cost} $", RPPlayer.NotificationType.INFO, "Staatsgefängnis", 20000);
        }

        public async Task UnJail(RPPlayer rpPlayer)
        {
            await rpPlayer.SetPositionAsync(Positions.JailOutsidePosition);
            rpPlayer.SetEquippedClothes();
            rpPlayer.SendNotification("Du hast deine Haftzeit abgesessen. Genieß die Freiheit", RPPlayer.NotificationType.INFO, "Staatsgefängnis");
        }


        public int GetProbationCost(int jailTime)
        {
            return 5000 +  jailTime * 500;
        }

    }
}

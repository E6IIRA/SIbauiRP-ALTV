using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Resources.Chat.Api;
using GangRP_Server.Core;
using GangRP_Server.Events;
using GangRP_Server.Handlers.Logger;
using GangRP_Server.Models;
using GangRP_Server.Utilities;
using GangRP_Server.Utilities.Bank;
using GangRP_Server.Utilities.Blip;
using Microsoft.EntityFrameworkCore;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Modules.Bank
{
    public sealed class BankModule : ModuleBase, ILoadEvent, IPlayerConnectEvent, IPressedEEvent
    {
        private readonly ILogger _logger;

        private readonly RPContext _rpContext;

        private Dictionary<int, BankTypeData> _bankTypes;

        private Dictionary<int, BankData> _banks;

        public BankModule(ILogger logger, RPContext rpContext)
        {
            _logger = logger;
            _rpContext = rpContext;
        }

        public void OnLoad()
        {
            _banks = AddTableLoadEvent<BankData>(_rpContext.BankData.Include(d => d.BankType), OnItemLoad).ToDictionary(bank => bank.Id);
            _bankTypes = AddTableLoadEvent<BankTypeData>(_rpContext.BankTypeData).ToDictionary(bankType => bankType.Id);
            AddClientEvent<int, int>("DepositMoney", DepositMoney);
            AddClientEvent<int, int>("WithdrawMoney", WithdrawMoney);
        }

        async void DepositMoney(IPlayer player, int bankId, int money)
        {
            RPPlayer rpPlayer = (RPPlayer) player;

            BankData bank = GetBankById(bankId);
            if (bank == null) return;
            if (bank.Position.Distance(rpPlayer.Position) > 3) return;

            int depositFee = 0;

            if (rpPlayer.BankType != bank.BankTypeId)
            {
                if (_bankTypes.TryGetValue(rpPlayer.BankType, out BankTypeData bankTypeData)) return;
                if (bankTypeData.DepositFee * money > bankTypeData.DepositFeeMaximum)
                    depositFee = bankTypeData.DepositFeeMaximum;
                else if (bankTypeData.DepositFee * money < bankTypeData.DepositFeeMinimum)
                    depositFee = bankTypeData.DepositFeeMinimum;
                else
                    depositFee = (int)bankTypeData.DepositFee * money;
            }

            if (await rpPlayer.MoneyToBank(money))
            {
                rpPlayer.SendChatMessage($"{money} eingezahlt amk. (Gebühren: {depositFee}");
            }
        }

        async void WithdrawMoney(IPlayer player, int bankId, int money)
        {
            RPPlayer rpPlayer = (RPPlayer)player;
            BankData bank = GetBankById(bankId);
            if (bank == null) return;
            if (bank.Position.Distance(rpPlayer.Position) > 3) return;

            int withdrawFee = 0;
            if (rpPlayer.BankType != bank.BankTypeId)
            {
                if (_bankTypes.TryGetValue(rpPlayer.BankType, out BankTypeData bankTypeData)) return;
                if (bankTypeData.WithdrawFee * money > bankTypeData.WithdrawFeeMaximum)
                    withdrawFee = bankTypeData.WithdrawFeeMaximum;
                else if (bankTypeData.WithdrawFee * money < bankTypeData.WithdrawFeeMinimum)
                    withdrawFee = bankTypeData.WithdrawFeeMinimum;
                else
                    withdrawFee = (int) bankTypeData.WithdrawFee * money;
            }

            if (await rpPlayer.BankToMoney(money + withdrawFee))
            {
                rpPlayer.SendChatMessage($"{money} ausgezahlt amk. (Gebühren: {withdrawFee})");
            }
        }

        private void OnItemLoad(BankData bank)
        {
#if DEBUG
            MarkerStreamer.Create(MarkerTypes.MarkerTypeDallorSign, bank.Position, new Vector3(1), color: new Rgba(255, 255, 0, 255));
            TextLabelStreamer.Create($"Bank Id: {bank.Id}", bank.Position, color: new Rgba(255, 255, 0, 255));
#endif

        }

        public void OnPlayerConnect(IPlayer player, string reason)
        {
            List<BlipData> blipDataList = new List<BlipData>();
            foreach (var shop in _banks.Values.Where(d => d.Class == 2))
            {
                blipDataList.Add(new BlipData(shop.Position, shop.Name));
            }
            player.Emit("SetPlayerBlips", new BlipDataWriter(blipDataList, 207, 69));
            var staatsBank = _banks.Values.FirstOrDefault(b => b.Class == 3);
            if (staatsBank == null) return;
            player.Emit("SetBlip", staatsBank.PositionX, staatsBank.PositionY, staatsBank.PositionZ, 207, 46, staatsBank.Name);
        }

        public Task<bool> OnPressedE(IPlayer player)
        {
            RPPlayer rpPlayer = (RPPlayer) player;

            var bank = _banks.Values.FirstOrDefault(b => rpPlayer.Position.Distance(b.Position) < 3);
            if (bank == null) return Task.FromResult(false);
            _logger.Info($"{rpPlayer.Name} opened Bank {bank.Id}");
            player.Emit("ShowIF", "Bank", new BankDataWriter(
                bank.Id, bank.Name, rpPlayer.Money, rpPlayer.BankMoney, bank.BankType.WithdrawFeeMinimum, 
                bank.BankType.WithdrawFee, bank.BankType.WithdrawFeeMaximum, 
                bank.BankType.DepositFeeMinimum, bank.BankType.DepositFee, bank.BankType.DepositFeeMaximum));
            return Task.FromResult(true);
        }


        public BankData GetBankById(int bankId)
        {
            if (_banks.TryGetValue(bankId, out BankData bank)) return bank;
            return null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using AltV.Net;
using AltV.Net.Elements.Entities;
using GangRP_Server.Core;
using GangRP_Server.Events;
using GangRP_Server.Extensions;
using GangRP_Server.Handlers.Logger;
using GangRP_Server.Handlers.Player;
using GangRP_Server.Models;
using GangRP_Server.Utilities.Phone.Apps;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Modules.Phone.Apps
{
    public class SmsChatObject
    {
        public SmsChatParticipant ChatParticipant1 { get; set; }
        public SmsChatParticipant ChatParticipant2 { get; set; }
        public List<IWritable> SmsMessages { get; set; }
        public SmsChatObject()
        {
        }
    }

    public sealed class SmsAppModule : ModuleBase, ILoadEvent
    {
        private readonly ILogger _logger;
        private readonly RPContext _rpContext;
        private readonly IPlayerHandler _playerHandler;
        private Dictionary<int, SmsChatObject> SmsChats { get; set; }
        public SmsAppModule(ILogger logger, RPContext rpContext, IPlayerHandler playerHandler)
        {
            _logger = logger;
            _rpContext = rpContext;
            _playerHandler = playerHandler;
        }

        public void OnLoad()
        {
            SmsChats = new Dictionary<int, SmsChatObject>();
            AddClientEvent("RqChats", RqChats);
            AddClientEvent<int>("RqChat", RqChat);
            AddClientEvent<int>("RmChat", RmChat);
            AddClientEvent<int, string>("SendChat", SendChat);
            AddClientEvent<int, string>("CreateChat", CreateChat);
        }

        public async void RqChats(IPlayer player)
        {
            RPPlayer rpPlayer = (RPPlayer)player;
            await using RPContext rpContext = new RPContext();
            List<int> ints = await rpContext.SmsChatParticipant.Where(d => d.Number == rpPlayer.PhoneNumber).Include(d => d.SmsChat).Select(participant => participant.SmsChatId).ToListAsync();
            List<IWritable> test = new List<IWritable>();
            foreach (var smsChatId in ints)
            {
                SmsChatParticipant chatParticipant = await rpContext.SmsChatParticipant.Where(d => (d.SmsChatId == smsChatId) && (d.Number != rpPlayer.PhoneNumber)).FirstOrDefaultAsync();
                test.Add(new SmsChatWriter(chatParticipant.SmsChatId, chatParticipant.Number));
            }
            rpPlayer.Emit("UpdateView", "RsChats", test);
        }

        public async void RqChat(IPlayer player, int chatId)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            List<IWritable> chatMessages = new List<IWritable>();
            //Check if chat is already loaded
            if (SmsChats.TryGetValue(chatId, out SmsChatObject smsChatObject))
            {
                //chat is loaded
                chatMessages = smsChatObject.SmsMessages;
            }
            else
            {
                //chat is not loaded
                await using RPContext rpContext = new RPContext();
                var smsChatParticipants = await rpContext.SmsChatParticipant.Where(d => d.SmsChatId == chatId).ToArrayAsync();
                if (smsChatParticipants.Count() == 2)
                {
                    var chatParticipant1 = smsChatParticipants[0];
                    var chatParticipant2 = smsChatParticipants[1];
                    IQueryable<SmsChatMessage> smsChatMessages = rpContext.SmsChatMessage.Where(d => (d.SmsChatParticipantId == chatParticipant1.Id) || (d.SmsChatParticipantId == chatParticipant2.Id));
                    foreach (var chatMessage in smsChatMessages)
                    {
                        int number = chatMessage.SmsChatParticipantId == chatParticipant1.Id ? chatParticipant1.Number : chatParticipant2.Number;
                        IWritable writable = new SmsChatMessageWriter(number, chatMessage.Message, chatMessage.Date);
                        chatMessages.Add(writable);
                    }
                    SmsChatObject newSmsChatObject = new SmsChatObject
                    {
                        ChatParticipant1 = chatParticipant1,
                        ChatParticipant2 = chatParticipant2,
                        SmsMessages = chatMessages
                    };
                    SmsChats.Add(chatId, newSmsChatObject);
                }
                else
                {
                    //There are not 2 People in this chat???? Wow something is really fucked up...
                    _logger.Error($"SMS CHAT ID {chatId} THERE ARE NOT 2 PARTICIPANTS...");
                    return;
                }
            }
            rpPlayer.Emit("UpdateView", "RsChat", chatMessages);
        }

        public async void RmChat(IPlayer player, int chatId)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            //Check if Chat is loaded (if not loaded this must mean there is an error lel)
            //Because, if you´re clicking on a chat in client, this will load the chat on serverside and give it to the player
            if (SmsChats.TryGetValue(chatId, out SmsChatObject smsChatObject))
            { 
                SmsChats.Remove(chatId);
                await using RPContext rpContext = new RPContext();
                SmsChat smsChat = await rpContext.SmsChat.Where(d => d.Id == chatId).Include(d => d.SmsChatParticipant).ThenInclude(d => d.SmsChatMessage).FirstOrDefaultAsync();
                foreach (var participant in smsChat.SmsChatParticipant)
                {
                    await participant.SmsChatMessage.ForEach(message => rpContext.SmsChatMessage.Remove(message));
                }
                await smsChat.SmsChatParticipant.ForEach(participant => rpContext.SmsChatParticipant.RemoveRange(participant));
                rpContext.SmsChat.Remove(smsChat);
                await rpContext.SaveChangesAsync();
            }
            else
            {
                //Chat is not loaded there is an error???
                _logger.Error($"Remove Chat - SMS CHAT ID {chatId} IS NOT LOADED?? WHY");
            }
        }

        public async void SendChat(IPlayer player, int chatId, string message)
        {
            RPPlayer rpPlayer = (RPPlayer) player;
            //Check if Chat is loaded (if not loaded this must mean there is an error lel)
            //Because, if you´re clicking on a chat in client, this will load the chat on serverside and give it to the player
            if (SmsChats.TryGetValue(chatId, out SmsChatObject smsChatObject))
            {
                //Chat is loaded 
                smsChatObject.SmsMessages.Add(new SmsChatMessageWriter(rpPlayer.PhoneNumber, message, DateTime.Now));
                await using RPContext rpContext = new RPContext();
                SmsChatParticipant sender = smsChatObject.ChatParticipant1;
                SmsChatParticipant target = smsChatObject.ChatParticipant2;
                if (smsChatObject.ChatParticipant2.Number == rpPlayer.PhoneNumber)
                {
                    sender = smsChatObject.ChatParticipant2;
                    target = smsChatObject.ChatParticipant1;
                }
                await rpContext.SmsChatMessage.AddAsync(new SmsChatMessage() {SmsChatParticipantId = sender.Id, Message = message});
                await rpContext.SaveChangesAsync();
                //TODO Change that
                RPPlayer targetRpPlayer = _playerHandler.GetRpPlayers().FirstOrDefault(d => d.PhoneNumber == target.Number);
                targetRpPlayer?.Emit("UpdateView", "UpdateChat", chatId, sender.Number, message);
            }
            else
            {
                //Chat is not loaded there is an error???
                _logger.Error($"Send Chat - SMS CHAT ID {chatId} IS NOT LOADED?? WHY");
            }
        }

        public async void CreateChat(IPlayer player, int number, string message)
        {
            //chat doesnt exist => create a new chat
            RPPlayer rpPlayer = (RPPlayer)player;
            await using RPContext rpContext = new RPContext();
            SmsChat smsChat = new SmsChat();
            await rpContext.SmsChat.AddAsync(smsChat);
            await rpContext.SaveChangesAsync();
            SmsChatParticipant senderParticipant = new SmsChatParticipant
            {
                Number = rpPlayer.PhoneNumber,
                SmsChatId = smsChat.Id
            };
            SmsChatParticipant targetParticipant = new SmsChatParticipant
            {
                Number = number,
                SmsChatId = smsChat.Id
            };
            await rpContext.SmsChatParticipant.AddAsync(senderParticipant);
            await rpContext.SmsChatParticipant.AddAsync(targetParticipant);
            await rpContext.SaveChangesAsync();
            SmsChatObject smsChatObject = new SmsChatObject
            {
                ChatParticipant1 = senderParticipant,
                ChatParticipant2 = targetParticipant,
                SmsMessages = new List<IWritable>
                {
                    new SmsChatMessageWriter(rpPlayer.PhoneNumber, message, DateTime.Now)
                }
            };

            await rpContext.SmsChatMessage.AddAsync(new SmsChatMessage() {SmsChatParticipantId = senderParticipant.Id, Message = message});
            await rpContext.SaveChangesAsync();


            SmsChats.Add(smsChat.Id, smsChatObject);
            //TODO Change that
            RPPlayer targetRpPlayer = _playerHandler.GetRpPlayers().FirstOrDefault(d => d.PhoneNumber == targetParticipant.Number);
            targetRpPlayer?.Emit("UpdateView", "AddChat", smsChat.Id, senderParticipant.Number, message);
        }
    }
}
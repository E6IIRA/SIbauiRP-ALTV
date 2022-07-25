using System.Linq;
using AltV.Net.Elements.Entities;
using GangRP_Server.Core;
using GangRP_Server.Events;
using GangRP_Server.Handlers.Logger;
using GangRP_Server.Models;
using GangRP_Server.Utilities.Phone.Apps;
using Microsoft.EntityFrameworkCore;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Modules.Phone.Apps
{
    public sealed class ContactAppModule : ModuleBase, ILoadEvent
    {
        private readonly ILogger _logger;
        private readonly RPContext _rpContext;

        public ContactAppModule(ILogger logger, RPContext rpContext)
        {
            _logger = logger;
            _rpContext = rpContext;
        }

        public void OnLoad()
        {
            AddClientEvent("RqContacts", RequestContacts);
            AddClientEvent<int, string, string>("AddContact", AddContact);
            AddClientEvent<int>("RemoveContact", RemoveContact);
            AddClientEvent<int, int, string, string>("EditContact", EditContact);
        }

        public async void RequestContacts(IPlayer player)
        {
            RPPlayer rpPlayer = (RPPlayer)player;
            await using RPContext rpContext = new RPContext();
            IQueryable<PlayerPhoneContact> playerPhoneContacts = rpContext.PlayerPhoneContact.Where(d => d.PlayerId == rpPlayer.PlayerId);
            rpPlayer.Emit("UpdateView", "RsContacts", new PhoneContactWriter(playerPhoneContacts));
        }

        public async void AddContact(IPlayer player, int number, string name, string note)
        {
            RPPlayer rpPlayer = (RPPlayer)player;
            await using RPContext rpContext = new RPContext();
            await rpContext.PlayerPhoneContact.AddAsync(new PlayerPhoneContact() {PlayerId = rpPlayer.PlayerId, Number = number, Name = name, Note = note});
            await rpContext.SaveChangesAsync();
        }

        public async void RemoveContact(IPlayer player, int number)
        {
            RPPlayer rpPlayer = (RPPlayer)player;
            await using RPContext rpContext = new RPContext();
            PlayerPhoneContact playerPhoneContact = await rpContext.PlayerPhoneContact.Where(d => (d.PlayerId == rpPlayer.PlayerId) && (d.Number == number)).FirstOrDefaultAsync();
            if (playerPhoneContact != null)
            {
                rpContext.PlayerPhoneContact.Remove(playerPhoneContact);
                await rpContext.SaveChangesAsync();
            }
        }

        public async void EditContact(IPlayer player, int oldNumber, int newNumber, string name, string note)
        {
            RPPlayer rpPlayer = (RPPlayer)player;
            await using RPContext rpContext = new RPContext();
            PlayerPhoneContact playerPhoneContact = await rpContext.PlayerPhoneContact.Where(d => (d.PlayerId == rpPlayer.PlayerId) && (d.Number == oldNumber)).FirstOrDefaultAsync();
            if (playerPhoneContact != null)
            {
                playerPhoneContact.Number = newNumber;
                playerPhoneContact.Name = name;
                playerPhoneContact.Note = note;
                await rpContext.SaveChangesAsync();
            }
        }
    }
}
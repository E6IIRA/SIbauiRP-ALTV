using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Core
{
    public abstract class ModuleBase
    {
        protected void AddClientEvent(string eventName, Action<IPlayer> action)
        {
            AltAsync.OnClient(eventName, action);
        }

        protected void AddClientEvent<T>(string eventName, Action<IPlayer, T> action)
        {
            AltAsync.OnClient(eventName, action);
        }

        protected void AddClientEvent<T1, T2>(string eventName, Action<IPlayer, T1, T2> action)
        {
            AltAsync.OnClient(eventName, action);
        }
        protected void AddClientEvent<T1, T2, T3>(string eventName, Action<IPlayer, T1, T2, T3> action)
        {
            AltAsync.OnClient(eventName, action);
        }

        protected void AddClientEvent<T1, T2, T3, T4>(string eventName, Action<IPlayer, T1, T2, T3, T4> action)
        {
            AltAsync.OnClient(eventName, action);
        }

        protected IEnumerable<T> AddTableLoadEvent<T>(IQueryable queryable, Action<T>? action = null) where T : class
        {
            List<T> items = new List<T>();
            foreach (T? item in queryable)
            {
                if (item == null) continue;
                action?.Invoke(item);
                items.Add(item);
            }

            return items;
        }
    }

    public abstract class ModuleBase<T> : ModuleBase where T : ModuleBase<T>
    {
#pragma warning disable CS8618
        public static T Instance { get; private set; }
#pragma warning restore CS8618

        public ModuleBase()
        {
            Instance = (T)this;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GangRP_Server.Models;
using Microsoft.EntityFrameworkCore;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Core
{
    public abstract class DataModuleBase<T, V> : ModuleBase<T> where T : ModuleBase<T> where V : class
    {
        private readonly Dictionary<int, V> _items = new Dictionary<int, V>();

        public DataModuleBase()
        {
            using var rpContext = new RPContext();
            foreach (V? item in GetData(rpContext.Set<V>()))
            {
                if (item == null) continue;
                _items.Add(GetId(item), item);
                OnItemLoaded(item);
            }

            OnItemsLoaded();
        }

        internal abstract int GetId(V item);
        internal abstract IQueryable GetData(DbSet<V> dbSet);
        internal virtual void OnItemLoaded(V item) { }
        internal virtual void OnItemsLoaded() { }

        internal V GetById(int id)
        {
            if (_items.TryGetValue(id, out var value))
            {
                return value;
            }

            return _items[0];
        }

        internal IEnumerable<V> GetValues()
        {
            return _items.Values;
        }
    }
}

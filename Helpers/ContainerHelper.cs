using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AltV.Net.Async;
using Autofac;
using GangRP_Server.Core;
using GangRP_Server.Models;
using GangRP_Server.Modules.Inventor;
using Microsoft.EntityFrameworkCore;

namespace RageMP_Server.Helpers
{
    internal class ContainerHelper : IDisposable
    {
        private readonly Type[] _loadedTypes = Assembly.GetExecutingAssembly().GetTypes();

        private IContainer _container;
        private ILifetimeScope _scope;

        private List<Type> _handlerTypes = new List<Type>();
        private List<Type> _moduleTypes = new List<Type>();
        private List<Type> _itemTypes = new List<Type>();

        internal void RegisterTypes()
        {
            var builder = new ContainerBuilder();

            LogStartup("Load types");
            LoadTypes();

            LogStartup("Register gamemode");
            builder.RegisterType<Gamemode>()
                .As<IGamemode>();

            LogStartup("Register handlers");
            foreach (var handler in _handlerTypes)
            {
                builder.RegisterTypes(handler)
                //.As(t => t.GetInterfaces().First(i => i.Name.Equals("I" + t.Name)))
                .AsImplementedInterfaces()
                .SingleInstance();
            }

            LogStartup("Register modules");
            foreach (var module in _moduleTypes)
            {
                builder.RegisterType(module)
                .AsImplementedInterfaces()
                .AsSelf()
                .SingleInstance();
            }

            LogStartup("Register itemscripts");
            foreach (var item in _itemTypes)
            {
                builder.RegisterType(item)
                .As<IItemScript>()
                .SingleInstance();
            }

            LogStartup("Register database context");
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<RPContext>()
                .UseMySql("server=db.sibaui;database=sibauirp;user=root;password=Keksi09!!;treattinyasboolean=true",
                    b => b.ServerVersion("10.4.11-mariadb")
                        .EnableRetryOnFailure());

            
             //dotnet ef dbcontext scaffold "server=db.sibauirp.de;database=sibauirp;user=Sibaui;password=XPYfMKEUMN9wqXcS!yDtHAw4qc?Nh?Bz3wF7r-SxgnXQ-q8Yy-faDMd8F7C_8BV6;treattinyasboolean=true" "Pomelo.EntityFrameworkCore.MySql" -o Models -c RPContext -f

             builder.RegisterType<RPContext>()
                .WithParameter("options", dbContextOptionsBuilder.Options)
                .InstancePerLifetimeScope();

            _container = builder.Build();
        }

        internal void ResolveTypes()
        {
            _scope = _container.BeginLifetimeScope();
            foreach (var type in _moduleTypes)
            {
                //AltAsync.Log("Resolve module " + type.FullName);
                _scope.Resolve(type);
            }
        }

        internal T Resolve<T>()
        {
            return _scope.Resolve<T>();
        }

        private void LoadTypes()
        {
            foreach (Type type in _loadedTypes)
            {
                if (IsHandlerType(type))
                {
                    _handlerTypes.Add(type);
                }
                else if (IsModuleType(type))
                {
                    _moduleTypes.Add(type);
                }
                else if (IsItemType(type))
                {
                    _itemTypes.Add(type);
                }
            }
        }

        private bool IsHandlerType(Type type)
        {
            if (type.Namespace == null) return false;
            return type.Namespace.StartsWith("GangRP_Server.Handlers") &&
                                            !type.Name.StartsWith("<");
        }

        //private Type[] GetEventTypes()
        //{
        //    return _currentTypes.Where(t => (t.Namespace.StartsWith("SanAndreas_Life.Events.Client") ||
        //                                    t.Namespace.StartsWith("SanAndreas_Life.Events.Script")) &&
        //                                    !t.Name.StartsWith("<")).ToArray();
        //}

        private bool IsModuleType(Type type)
        {
            if (type.Namespace == null) return false;
            return type.Namespace.StartsWith("GangRP_Server.Modules") &&
                                            type.BaseType != null &&
                                            (type.BaseType == typeof(ModuleBase) ||
                                            type.BaseType.IsGenericType) &&
                                            !type.Name.StartsWith("<");
        }

        private bool IsItemType(Type type)
        {
            if (type.Namespace == null) return false;
            return type.Namespace.StartsWith("GangRP_Server.Modules.Inventory.Item") &&
                                            !type.Name.StartsWith("<");
        }

        private static void LogStartup(string text)
        {
            AltAsync.Log($"[STARTUP] {text}");
        }

        public void Dispose()
        {
            _container.Dispose();
            _scope.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}

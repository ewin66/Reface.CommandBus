﻿using Autofac;
using Reface.CommandBus.Core;
using System;
using System.Collections.Generic;

namespace Reface.CommandBus
{
    /// <summary>
    /// 基于 autofac 的 <see cref="ICommandHandlerFactory"/>
    /// </summary>
    public class AutofacCommandHandlerFactory : ICommandHandlerFactory
    {
        private readonly ILifetimeScope lifetimeScope;
        private readonly Type commandHandlerTypeBase;

        public AutofacCommandHandlerFactory(ILifetimeScope lifetimeScope)
        {
            this.lifetimeScope = lifetimeScope;
            this.commandHandlerTypeBase = typeof(ICommandHandler<>);
        }

        public IEnumerable<ICommandHandler> GetHandlers(Type commandType)
        {
            IEnumerable<ICommandHandler> handlers = this.lifetimeScope
                .Resolve<IEnumerable<ICommandHandler>>();
            List<ICommandHandler> result = new List<ICommandHandler>();
            foreach (var handler in handlers)
            {
                Type thisCommandType = GetCommandType(handler.GetType());
                if (!thisCommandType.IsAssignableFrom(commandType)) continue;
                result.Add(handler);
            }
            return result;
        }

        private Type GetCommandType(Type commandHandlerType)
        {
            Type baseType = commandHandlerType.GetInterface(this.commandHandlerTypeBase.FullName);
            return baseType.GetGenericArguments()[0];
        }
    }
}

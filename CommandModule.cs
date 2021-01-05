using Discord.Commands;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Discord.Bot
{
    /// <summary>
    /// An extended <see cref="ModuleBase{SocketCommandContext}"/>
    /// </summary>
    public abstract class CommandModule : ModuleBase<SocketCommandContext>
    {
        /// <summary>
        /// Reference to the <see cref="IServiceProvider"/> that was injected
        /// </summary>
        public IServiceProvider InjectedProvider { get; set;  }

        /// <summary>
        /// Returns the <see cref="DiscordBot"/> passed into <see cref="InjectedProvider"/>, <see langword="null"/> if empty
        /// </summary>
        public IDiscordBot InjectedBot { get => InjectedProvider.GetService<IDiscordBot>(); }
    }

    /// <summary>
    /// An extended <see cref="ModuleBase{SocketCommandContext}"/> using a generic bot
    /// </summary>
    public abstract class CommandModule<TBot> : CommandModule where TBot : IDiscordBot
    {
        /// <summary>
        /// Returns the <typeparamref name="TBot"/> passed into <see cref="InjectedProvider"/>, <see langword="null"/> if empty or incorrect type
        /// </summary>
        public new TBot InjectedBot { get => (TBot)InjectedProvider.GetService<IDiscordBot>(); }
    }
}

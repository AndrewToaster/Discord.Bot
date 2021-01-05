using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Discord.Bot
{
    /// <summary>
    /// An Interface used for command handlers
    /// </summary>
    public interface ICommandHandler
    {
        /// <summary>
        /// A collection of all loaded modules
        /// </summary>
        ModuleInfo[] Modules { get; }

        /// <summary>
        /// Callback used for handling a inbound command
        /// </summary>
        /// <param name="message">The command's message</param>
        /// <returns></returns>
        Task HandleCommandAsync(SocketUserMessage message);

        /// <summary>
        /// Callback used for registering <see cref="ModuleBase"/>
        /// </summary>
        /// <param name="t">The type to register</param>
        /// <returns></returns>
        Task RegisterModuleAsync(Type t);

        /// <summary>
        /// Callback used for unregistering <see cref="ModuleBase"/>
        /// </summary>
        /// <param name="t">The type to unregister</param>
        /// <returns></returns>
        Task UnregisterModuleAsync(Type t);
    }
}

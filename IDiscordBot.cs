using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;
using Discord.Bot.PreBuilt;
using System;

namespace Discord.Bot
{
    /// <summary>
    /// The base class of Discord bots, using a generic command handler
    /// </summary>
    /// <typeparam name="TCommandHandler">A command handler used for commands</typeparam>
    public interface IDiscordBot<out TCommandHandler> where TCommandHandler : ICommandHandler
    {
        /// <summary>
        /// The base client
        /// </summary>
        public DiscordSocketClient Client { get; }

        /// <summary>
        /// The logger used for logging messages
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// The command handler used for commands
        /// </summary>
        public TCommandHandler CommandHandler { get; }

        /// <summary>
        /// Starts the client with a specified token
        /// </summary>
        /// <param name="token">Discord's Bot's Appliction token</param>
        /// <returns></returns>
        public async Task StartAsync(string token)
        {
            await Client.LoginAsync(TokenType.Bot, token);
            await Client.StartAsync();
        }
    }
}

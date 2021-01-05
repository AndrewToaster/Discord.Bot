using Discord.Bot;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Discord.Bot
{
    /// <summary>
    /// Class used for creating <see cref="IServiceProvider"/> for injection
    /// </summary>
    internal static class ServiceCreator
    {
        /// <summary>
        /// Creates a <see cref="IServiceProvider"/> containing a singleton <see cref="DiscordBot"/> <paramref name="bot"/>
        /// </summary>
        /// <param name="bot">The bot to add as a singleton</param>
        /// <returns>The <see cref="IServiceProvider"/> containing the DiscordBot Singleton</returns>
        public static IServiceProvider BuildCmdService(IDiscordBot bot)
        {
            return new ServiceCollection()
                .AddSingleton(bot)
                .BuildServiceProvider();
        }
    }
}

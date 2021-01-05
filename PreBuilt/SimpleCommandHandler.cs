using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Discord.Bot.PreBuilt
{
    /// <summary>
    /// A default implementation of an <see cref="ICommandHandler"/>
    /// </summary>
    public sealed class SimpleCommandHandler : ICommandHandler
    {
        private IDiscordBot _bot;
        private readonly CommandService _service;
        private IServiceProvider _serviceProvider;
        private readonly Assembly _modAssembly;
        private bool _initialized = false;

        private DiscordSocketClient _client { get => _bot.Client; }

        /// <summary>
        /// A prefix to check messages against
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// A collection of all loaded modules
        /// </summary>
        public ModuleInfo[] Modules { get => _service.Modules.ToArray(); }

        /// <summary>
        /// Constructs a <see cref="SimpleCommandHandler"/> using a prefix, <see cref="Assembly"/> 
        /// where <see cref="ModuleBase"/>s are stored and a <see cref="DiscordBot"/>
        /// </summary>
        /// <param name="prefix">The prefix used for commands</param>
        /// <param name="moduleAssembly">Assembly where command modules are stored, this will mostly be <see cref="Assembly.GetExecutingAssembly"/></param>
        /// <param name="client">The client to register event callbacks</param>
        /// <param name="config">The configuration for <see cref="CommandService"/>, leave null for default</param>
        public SimpleCommandHandler(string prefix, IDiscordBot bot, Assembly moduleAssembly, CommandServiceConfig config = null)
        {
            Prefix = prefix;
            config ??= new CommandServiceConfig { IgnoreExtraArgs = true, CaseSensitiveCommands = false, DefaultRunMode = RunMode.Async };
            _modAssembly = moduleAssembly;
            _service = new CommandService(config);
            InitializeAsync(bot).Wait();
        }

        /// <summary>
        /// Handles a command message
        /// </summary>
        /// <param name="message">The command's message</param>
        /// <returns></returns>
        public async Task HandleCommandAsync(SocketUserMessage message)
        {
            if (!_initialized)
                throw new Exception("Cannot execute unless initialized");

            int prefixOffset = -1;

            if (!Utils.MessagePrefixCheck(message, _client, Prefix, ref prefixOffset))
            {
                _bot.Logger.Log("Failed prefix check on message '" + message.Content + "'", LogSeverity.Verbose);
                return;
            }

            var MsgContext = new SocketCommandContext(_client, message);
            var result = await _service.ExecuteAsync(MsgContext, prefixOffset, _serviceProvider);

            if (!result.IsSuccess)
            {
                switch (result.Error)
                {
                    case CommandError.UnknownCommand:
                        _bot.Logger.Log("Unknown command '" + message.Content + "'", LogSeverity.Verbose);
                        return;

                    default:
                        await MsgContext.ReplyAsync("Oh uh, somebody made a serius fucky wucky. Now i have to to get in the forever box!");
                        return;
                }
            }
        }

        /// <summary>
        /// Initializes the command handler, creating callbacks and registering <see cref="ModuleBase"/>s
        /// </summary>
        /// <returns></returns>
        private async Task InitializeAsync(IDiscordBot bot)
        {
            _bot = bot;
            _serviceProvider = ServiceCreator.BuildCmdService(_bot);

            _client.MessageReceived += SafeHandle;
            _service.CommandExecuted += CommandExecuted;
            _service.Log += (log) => Task.Run(() => _bot.Logger.Log(log));

            await _service.AddModulesAsync(_modAssembly, _serviceProvider);

            _initialized = true;
        }

        /// <summary>
        /// Callback used for logging command execution
        /// </summary>
        /// <param name="optCmdInfo">The command info, can be null</param>
        /// <param name="context">The context of the execution</param>
        /// <param name="result">The result of the execution</param>
        /// <returns></returns>
        internal Task CommandExecuted(Optional<CommandInfo> optCmdInfo, ICommandContext context, IResult result)
        {
            string successFormat = "User '{0}' executed command '{1}' ({2})";
            string failFormat = "User '{0}' tried to execute command '{1}' with error '{2}' ({3})";

            if (result.IsSuccess)
            {
                string logMessage = string.Format(successFormat, context.User.GetFullName(), optCmdInfo.GetValueOrDefault()?.Name, context.Message);
                _bot.Logger.Log(logMessage, LogSeverity.Info);
            }
            else
            {
                string logMessage = string.Format(failFormat, context.User.GetFullName(), optCmdInfo.GetValueOrDefault()?.Name, result.GetFullError(), context.Message);
                LogSeverity severity;

                switch (result.Error.Value)
                {
                    case CommandError.UnknownCommand:
                        severity = LogSeverity.Verbose;
                        break;

                    case CommandError.ParseFailed:
                    case CommandError.BadArgCount:
                    case CommandError.ObjectNotFound:
                        severity = LogSeverity.Info;
                        break;

                    case CommandError.MultipleMatches:
                    case CommandError.Unsuccessful:
                    case CommandError.Exception:
                        severity = LogSeverity.Error;
                        break;

                    case CommandError.UnmetPrecondition:
                    default:
                        severity = LogSeverity.Warning;
                        break;
                }

                _bot.Logger.Log(logMessage, severity);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Intermediate method used to check if a <see cref="SocketMessage"/> is not a system message
        /// </summary>
        /// <param name="msg">The message to check</param>
        /// <returns></returns>
        internal async Task SafeHandle(SocketMessage msg)
        {
            if (!(msg is SocketUserMessage uMsg))
            {
                return;
            }

            await HandleCommandAsync(uMsg);
        }

        /// <summary>
        /// Method used to register modules
        /// </summary>
        /// <param name="t">The module to register</param>
        /// <returns></returns>
        public async Task RegisterModuleAsync(Type t)
        {
            await _service.AddModuleAsync(t, null);
        }

        /// <summary>
        /// Method used to unregister modules
        /// </summary>
        /// <param name="t">The module to unregister</param>
        /// <returns></returns>
        public async Task UnregisterModuleAsync(Type t)
        {
            await _service.RemoveModuleAsync(t);
        }
    }
}

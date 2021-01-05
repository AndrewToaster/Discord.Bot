using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Bot;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Discord.Bot
{
    /// <summary>
    /// Class contatining utility methods
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Sends a message to the source channel of this contenxt
        /// </summary>
        /// <param name="context">Actual context of the message</param>
        /// <param name="text">Text of the message</param>
        /// <param name="tts">Should the message be Text-To-Speech</param>
        /// <param name="embed">Attached Embed</param>
        /// <returns></returns>
        public static async Task ReplyAsync(this ICommandContext context, string text, bool tts = false, Discord.Embed embed = null)
        {
            await context.Channel.SendMessageAsync(text, tts, embed);
        }

        /// <summary>
        /// Checks if a message meets specific parameters to be considered a command
        /// </summary>
        /// <param name="msg">The message to check</param>
        /// <param name="client">The bot's client</param>
        /// <param name="prefix">Prefix to check</param>
        /// <param name="prefixOffset">Reference to where the command begins (offset by prefix)</param>
        /// <returns>Whether the message is or isn't validly prefixed</returns>
        public static bool MessagePrefixCheck(SocketUserMessage msg, DiscordSocketClient client, string prefix, ref int prefixOffset)
        {
            bool hasStringPrefix = msg.HasStringPrefix(prefix, ref prefixOffset);
            bool hasMentionPrefix = msg.HasMentionPrefix(client.CurrentUser, ref prefixOffset);
            bool isBot = msg.Author.IsBot;

            return !isBot && (hasStringPrefix || hasMentionPrefix);
        }

        /// <summary>
        /// Checks if an Error occured and is different then <see cref="CommandError.UnknownCommand"/>
        /// </summary>
        /// <param name="result">The result to check</param>
        /// <returns>If an error occured and is not <see cref="CommandError.UnknownCommand"/></returns>
        internal static bool IsActualError(this IResult result)
        {
            return !result.IsSuccess && result.Error != CommandError.UnknownCommand;
        }

        /// <summary>
        /// Registers a Generic Module
        /// </summary>
        /// <typeparam name="T">Module to register</typeparam>
        /// <param name="handler">Command handler to register it to</param>
        /// <returns></returns>
        public static async Task RegisterModuleAsync<T>(this ICommandHandler handler) where T : ModuleBase
        {
            await handler.RegisterModuleAsync(typeof(T));
        }

        /// <summary>
        /// Helper method to simplify formating of log messages into 1 line
        /// </summary>
        /// <param name="log">The message to format</param>
        /// <returns>Formatted string</returns>
        internal static string LogFormat(LogMessage log)
        {
            DateTime dateTime = DateTime.Now;
            string date = dateTime.ToShortDateString();
            string time = dateTime.ToShortTimeString();
            LogSeverity severity = log.Severity;
            string message = log.Message;

            if (log.Exception != null)
            {
                return string.Format("[{0} - {1}] [{2} - {3}]: {4} --> {5}", date, time, log.Source, severity, message, log.Exception);
            }
            else
            {
                return string.Format("[{0} - {1}] [{2} - {3}]: {4}", date, time, log.Source, severity, message);
            }
        }

        /// <summary>
        /// Helper method to simplify formating of log messages into 1 line
        /// </summary>
        /// <param name="log">The message to format</param>
        /// <returns>Formatted string</returns>
        internal static string LogFormat(string message)
        {
            DateTime dateTime = DateTime.Now;
            string date = dateTime.ToShortDateString();
            string time = dateTime.ToShortTimeString();

            return string.Format("[{0} - {1}] [Application - Custom]: {2}", date, time, message);
        }

        /// <summary>
        /// Helper method that returns <see cref="IUser.Username"/> and <see cref="IUser.Discriminator"/> together
        /// </summary>
        /// <param name="user">The user to return full name of</param>
        /// <returns>&lt;<see cref="IUser.Username"/>&gt;#&lt;<see cref="IUser.Discriminator"/>&gt;</returns>
        internal static string GetFullName(this IUser user)
        {
            return string.Format("{0}#{1}", user.Username, user.Discriminator);
        }

        /// <summary>
        /// Helper method that returns <see cref="IResult.Error"/> and <see cref="IResult.ErrorReason"/> together
        /// </summary>
        /// <param name="result">The result to return the full error message</param>
        /// <returns>&lt;<see cref="IResult.Error"/>&gt;: &lt;<see cref="IResult.ErrorReason"/>&gt;</returns>
        internal static string GetFullError(this IResult result)
        {
            return string.Format("{0}: {1}", result.Error, result.ErrorReason);
        }

        /// <summary>
        /// Strip illegal chars and reserved words from a candidate filename (should not include the directory path)
        /// </summary>
        /// <remarks>
        /// http://stackoverflow.com/questions/309485/c-sharp-sanitize-file-name
        /// </remarks>
        internal static string CleanFileName(string filename)
        {
            var invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            var invalidReStr = string.Format(@"[{0}]+", invalidChars);

            var reservedWords = new[]
            {
                "CON", "PRN", "AUX", "CLOCK$", "NUL", "COM0", "COM1", "COM2", "COM3", "COM4",
                "COM5", "COM6", "COM7", "COM8", "COM9", "LPT0", "LPT1", "LPT2", "LPT3", "LPT4",
                "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
            };

            var sanitisedNamePart = Regex.Replace(filename, invalidReStr, "_");
            foreach (var reservedWord in reservedWords)
            {
                var reservedWordPattern = string.Format("^{0}\\.", reservedWord);
                sanitisedNamePart = Regex.Replace(sanitisedNamePart, reservedWordPattern, "_reservedWord_.", RegexOptions.IgnoreCase);
            }

            return sanitisedNamePart;
        }

        /// <summary>
        /// Turns first letter into a capital one
        /// </summary>
        /// <param name="s">string to operate on</param>
        /// <param name="lowercase">Turn all of the string into lowercase except the first one</param>
        /// <returns>Capitalized string</returns>
        /// <remarks>Modified version of 'https://www.dotnetperls.com/uppercase-first-letter'</remarks>
        internal static string UpperCaseFirst(this string s, bool lowercase = false)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }

            char[] a = lowercase ? s.ToLowerInvariant().ToCharArray() : s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }
    }
}

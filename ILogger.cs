using System;
using System.Threading.Tasks;

namespace Discord.Bot
{
    /// <summary>
    /// An Interface for custom loggers for <see cref="IDiscordBot"/>s
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// The the level which to log messages
        /// </summary>
        LogSeverity LogLevel { get; set; }

        /// <summary>
        /// Logs a Discord's LogMessage
        /// </summary>
        /// <param name="msg">The message to log</param>
        /// <returns></returns>
        void Log(LogMessage msg);

        /// <summary>
        /// Method to log a message with specified severity
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="severity">The severity of the message</param>
        /// <returns></returns>
        void Log(string message, LogSeverity severity);

        /// <summary>
        /// Method to log a message with specified color
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="color">The color of the message</param>
        /// <returns></returns>
        void Log(string message, ConsoleColor color);

        /// <summary>
        /// Method to write the content of the log to a file
        /// </summary>
        /// <param name="directory">Directory where to put log files</param>
        /// <returns></returns>
        Task WriteToFileAsync(string directory);
    }
}
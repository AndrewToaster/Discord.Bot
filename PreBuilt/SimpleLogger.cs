using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

namespace Discord.Bot.PreBuilt
{
    /// <summary>
    /// The default implementation of a logger
    /// </summary>
    public sealed class SimpleLogger : ILogger
    {
        /// <summary>
        /// The list of logged messages
        /// </summary>
        public List<string> Messages { get; }

        /// <summary>
        /// The level at which to write log messages
        /// </summary>
        public LogSeverity LogLevel { get; set; }

        /// <summary>
        /// Constructs a new Logger with specified LogLevel
        /// </summary>
        /// <param name="logLevel">The level at which to log message</param>
        public SimpleLogger(LogSeverity logLevel) : this()
        {
            LogLevel = logLevel;
        }

        /// <summary>
        /// Constructs a new Logger with default LogLevel
        /// </summary>
        public SimpleLogger()
        {
            Messages = new List<string>();
        }

        /// <summary>
        /// Callback for when <see cref="Client"/> receives a Log event
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <returns></returns>
        public void Log(LogMessage message)
        {
            if (message.Severity > LogLevel)
                return;

            ConsoleColor clr = Console.ForegroundColor;

            switch (message.Severity)
            {
                case LogSeverity.Critical:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
            }

            string logMsg = Utils.LogFormat(message);
            Messages.Add(logMsg);

            Console.WriteLine(logMsg);
            Console.ForegroundColor = clr;
        }

        /// <summary>
        /// Method for logging custom messages using Discord's <see cref="LogSeverity"/>
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="severity">The severity of the message</param>
        /// <returns></returns>
        public void Log(string message, LogSeverity severity)
        {
            Log(new LogMessage(severity, "Application", message));
        }

        /// <summary>
        /// Method to log a message with specified color
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="color">The color of the message</param>
        /// <returns></returns>
        public void Log(string message, ConsoleColor color)
        {
            var clr = Console.ForegroundColor;
            string logMsg = Utils.LogFormat(message);
            Messages.Add(logMsg);

            Console.WriteLine(logMsg);
            Console.ForegroundColor = clr;
        }

        /// <summary>
        /// Writes the content of the log into a file
        /// </summary>
        /// <param name="directory">The path to which to write</param>
        /// <returns></returns>
        public async Task WriteToFileAsync(string directory)
        {
            if (string.IsNullOrEmpty(directory?.Trim()))
            {
                directory = Directory.GetCurrentDirectory() + "/Logs";

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }

            if (!Directory.Exists(directory))
            {
                throw new DirectoryNotFoundException("Could not find specified directory: " + directory);
            }
            DateTime now = DateTime.Now;
            string fileName = string.Format("LOG_{0}", now.ToString(Globals.TimeFormat));

            using StreamWriter writer = new StreamWriter(directory.TrimEnd('/', '\\') + "/" + Utils.CleanFileName(fileName) + ".txt");

            await writer.WriteAsync(string.Join('\n', Messages));
        }
    }
}

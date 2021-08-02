using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FairPlayTube.CustomLoggers
{
    /// <summary>
    /// 
    /// </summary>
    public class CustomDbLogger : ILogger
    {
        private LogLevel[] LogLevels { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logLevels"></param>
        public CustomDbLogger(LogLevel[] logLevels)
        {
            this.LogLevels = logLevels; ;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="state"></param>
        /// <returns></returns>
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logLevel"></param>
        /// <returns></returns>
        public bool IsEnabled(LogLevel logLevel)
        {
            return this.LogLevels.Contains(logLevel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="logLevel"></param>
        /// <param name="eventId"></param>
        /// <param name="state"></param>
        /// <param name="exception"></param>
        /// <param name="formatter"></param>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!this.IsEnabled(logLevel))
                    return;
            var formattedMessage = formatter(state, exception);
            System.Diagnostics.Debug.WriteLine($"Custom Logger!: {formattedMessage}");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CustomDbLoggerProvider : ILoggerProvider
    {
        private ConcurrentDictionary<string, CustomDbLogger> Loggers { get; } = new();
        private LogLevel[] LogLevels { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logLevels"></param>
        public CustomDbLoggerProvider(LogLevel[] logLevels)
        {
            this.LogLevels = logLevels;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public ILogger CreateLogger(string categoryName)
        {
            return Loggers.GetOrAdd(categoryName, name => new CustomDbLogger(this.LogLevels));
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            this.Loggers.Clear();
        }
    }
}

using Microsoft.Extensions.Logging;

namespace ProcessMonitoring.ConsoleUtils
{
    public class ConsoleLogger
    {
        private static volatile ILogger? _logger;
        private static readonly object syncRoot = new();

        private ConsoleLogger() { }

        public static ILogger Logger
        {
            get
            {
                if (_logger == null)
                {
                    lock (syncRoot)
                    {
                        _logger ??= CreateLogger();
                    }
                }

                return _logger;
            }
        }

        public static ILogger CreateLogger()
        {
            using ILoggerFactory factory = LoggerFactory.Create(builder =>
                builder.AddConsole(c =>
                {
                    c.TimestampFormat = "[dd.MM.yy] [HH:mm:ss] ";
                }));

            return factory.CreateLogger<Program>();
        }
    }
}

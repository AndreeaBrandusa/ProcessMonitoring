using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace ProcessMonitoring
{
    public class MyProcess(string? name, int maxLifetime, int monitoringFrequency, ILogger logger)
    {
        private readonly string? name = name;
        private readonly int maxLifetime = maxLifetime;
        private readonly int monitoringFrequency = monitoringFrequency;
        private readonly ILogger logger = logger;

        public Process[] GetProcesses()
        {
            // Get the process with the specified name
            Process[] processes = Process.GetProcessesByName(name);

            if (processes == null)
            {
                logger.LogWarning("No process with the name {} was found.\n", name);
                return [];
            }

            logger.LogInformation("Process {} was found.\n", name);
            return processes;
        }

        public void VerifyProcesses()
        {
            Process[] processes = GetProcesses();

            foreach (var p in processes)
            {
                int runtime = Convert.ToInt32((DateTime.Now - p.StartTime).ToString("mm"));
                if (runtime >= maxLifetime)
                {
                    p.Kill();
                    logger.LogInformation("Process {} has been killed\n", p.ProcessName);
                }
                else
                {
                    logger.LogInformation("Process {} has been running for {} minutes\n", 
                        p.ProcessName, (DateTime.Now - p.StartTime).ToString("mm"));
                }
            }
        }

        private async Task KeyPressed()
        {
            logger.LogInformation("Press q to stop listening");
            do
            {
                await Task.Delay(1);
            } while (Console.ReadKey(true).Key != ConsoleKey.Q);
        }

        public async Task MonitorProcessesAsync()
        {
            var timer = new PeriodicTimer(TimeSpan.FromMinutes(monitoringFrequency));

            do
            {
                VerifyProcesses();
            } while (await timer.WaitForNextTickAsync());
        }

        public static ILogger CreateLogger()
        {
            using ILoggerFactory factory = LoggerFactory.Create(builder =>
                builder.AddConsole(c =>
                {
                    c.TimestampFormat = "[dd.MM.yy] [HH:mm:ss] ";
                }));

            return factory.CreateLogger<MyProcess>();
        }

        static async Task Main(string[] args)
        {
            ILogger logger = CreateLogger();
            MyProcess myProcess = new(args[0], Convert.ToInt32(args[1]), Convert.ToInt32(args[2]), logger);

            if (args.Length < 3)
            {
                logger.LogError("Not enough args");
                return;
            }

            var keyTask = myProcess.KeyPressed();
            var processTask = myProcess.MonitorProcessesAsync();

            await Task.WhenAny(keyTask, processTask);
        }
    }
}
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace ProcessMonitoring
{
    public class MyProcess(string name, int maxLifetime, int monitoringFrequency, ILogger logger, IProcessHandler processHandler, IConsoleWrapper consoleWrapper)
    {
        private readonly string name = name;
        private readonly int maxLifetime = maxLifetime;
        private readonly int monitoringFrequency = monitoringFrequency;
        private readonly ILogger logger = logger;
        private readonly IProcessHandler processHandler = processHandler;
        private readonly IConsoleWrapper consoleWrapper = consoleWrapper;

        public IProcessWrapper[] GetProcesses()
        {
            // Get the process with the specified name
            IProcessWrapper[] processes = processHandler.GetProcessesByName(name);

            if (processes == null || processes.Length == 0)
            {
                logger.LogWarning("No process with the name {} was found.\n", name);
                return [];
            }

            logger.LogInformation("Process {} was found {} times.\n", name, processes.Length);
            return processes;
        }

        public void VerifyProcesses(IProcessWrapper[] processes)
        {
            foreach (var p in processes)
            {
                int runtime = Convert.ToInt32((DateTime.Now - p.ProcessStartTime).ToString("mm"));
                if (runtime >= maxLifetime)
                {
                    p.Kill();
                    logger.LogInformation("Process {} has been killed\n", p.ProcessName);
                }
                else
                {
                    logger.LogInformation("Process {} has been running for {} minutes\n", 
                        p.ProcessName, (DateTime.Now - p.ProcessStartTime).ToString("mm"));
                }
            }
        }

        public async Task KeyPressed()
        {
            logger.LogInformation("Press q to stop listening");
            do
            {
                await Task.Delay(1);
            } while (consoleWrapper.ReadKey(true).Key != ConsoleKey.Q);
        }

        public async Task MonitorProcessesAsync()
        {
            var timer = new PeriodicTimer(TimeSpan.FromMinutes(monitoringFrequency));
            
            do
            {
                IProcessWrapper[] processes = GetProcesses();
                VerifyProcesses(processes);
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

        public async Task StartProcessesAsync()
        {
            var keyTask = KeyPressed();
            var processTask = MonitorProcessesAsync();

            await Task.WhenAny(keyTask, processTask);
        }

        public static async Task Main(string[] args)
        {
            ILogger logger = CreateLogger();

            if (args.Length < 3)
            {
                logger.LogError("Not enough args");
                return;
            }

            ProcessHandler handler = new();
            ConsoleWrapper consoleWrapper = new();
            MyProcess myProcess = new(args[0], Convert.ToInt32(args[1]), Convert.ToInt32(args[2]), logger, handler, consoleWrapper);

            await myProcess.StartProcessesAsync();
        }
    }
}
using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace ProcessMonitoring
{
    class MyProcess
    {
        private string? name;
        private int maxLifetime;
        private int monitoringFrequency;
        private static ILogger logger;

        void BindToRunningProcesses()
        {
            // Get the process with the specified name
            Process? process = Process.GetProcessesByName(name).FirstOrDefault();

            if (process == null)
            {
                logger.LogWarning("No process with the name {} was found.\n", name);
                return;
            }else
            {
                logger.LogInformation("Process {} was found.\n", name);
            }
        }

        void GetProcess()
        {
            BindToRunningProcesses();

            foreach (var p in Process.GetProcessesByName(name))
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

        async Task KeyPressed()
        {
            logger.LogInformation("Press q to stop listening");
            do
            {
                await Task.Delay(1);
            } while (Console.ReadKey(true).Key != ConsoleKey.Q);
        }

        async Task ListenForProcessAsync()
        {
            var timer = new PeriodicTimer(TimeSpan.FromMinutes(monitoringFrequency));

            do
            {
                GetProcess();
            } while (await timer.WaitForNextTickAsync());
        } 

        static async Task Main(string[] args)
        {
            using ILoggerFactory factory = LoggerFactory.Create(builder => 
                builder.AddConsole(c =>
                {
                    c.TimestampFormat = "[dd.MM.yy] [HH:mm:ss] ";
                }));

            logger = factory.CreateLogger<MyProcess>();

            MyProcess myProcess = new()
            {
                // Reading process details
                name = args[0],
                maxLifetime = Convert.ToInt32(args[1]),
                monitoringFrequency = Convert.ToInt32(args[2])
            };

            if (args.Length < 3)
            {
                logger.LogError("Not enough args");
                return;
            }
            

            var keyTask = myProcess.KeyPressed();
            var processTask = myProcess.ListenForProcessAsync();
            await Task.WhenAny(keyTask, processTask);
        }
    }
}
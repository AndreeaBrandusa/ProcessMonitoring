using System;
using System.Diagnostics;
using System.ComponentModel;
using System.ServiceProcess;

namespace ProcessMonitoring
{
    class MyProcess
    {
        private string? name;
        private int maxLifetime;
        private int monitoringFrequency;

        void BindToRunningProcesses()
        {
            // Get the process with the specified name
            Process? process = Process.GetProcessesByName(name).FirstOrDefault();

            foreach (var p in Process.GetProcessesByName(name))
            {
                if (process is not null)
                {
                    Console.Write("Process " + name + " was found.\n");
                }
                else
                {
                    Console.Write("No process with the name " + name + " was found.\n");
                }
            }
        }

        void GetProcess()
        {
            foreach (var p in Process.GetProcessesByName(name))
            {
                int runtime = Convert.ToInt32((DateTime.Now - p.StartTime).ToString("mm"));
                if (runtime > maxLifetime)
                {
                    p.Kill();
                    Console.Write("Process " + p.ProcessName + " has been killed\n");
                }
                else
                {
                    Console.Write("Process " + p.ProcessName + " has been running for " + (DateTime.Now - p.StartTime).ToString("mm") + "minutes\n");
                }
            }
        }

        async Task ListenForProcessAsync()
        {
            Console.WriteLine("Press q to stop listening");
            do
            {
                while (!Console.KeyAvailable)
                {
                    var timer = new PeriodicTimer(TimeSpan.FromMinutes(monitoringFrequency));

                    do
                    {
                        GetProcess();
                    } while (await timer.WaitForNextTickAsync());
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.Q);
        } 

        static async Task Main()
        {
            MyProcess myProcess = new();
           
            // Reading process details
            Console.Write("Enter Process Name: ");
            myProcess.name = Console.ReadLine();

            Console.Write("Enter maximum lifetime (in minutes): ");
            myProcess.maxLifetime = Convert.ToInt32(Console.ReadLine());

            Console.Write("Enter monitoring frequency (in minutes): ");
            myProcess.monitoringFrequency = Convert.ToInt32(Console.ReadLine());

            //myProcess.BindToRunningProcesses();
            await myProcess.ListenForProcessAsync();
        }
    }
}
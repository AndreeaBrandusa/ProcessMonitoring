using Microsoft.Extensions.Logging;
using ProcessMonitoring.ConsoleUtils;
using ProcessMonitoring.Monitor.Data;

namespace ProcessMonitoring.Monitor
{
    public class ProcessesMonitor(MonitorInputData monitorInputData, IProcessHandler processHandler)
    {
        private readonly MonitorInputData monitorInputData = monitorInputData;
        private readonly ProcessContainer processContainer = new(processHandler);

        public void VerifyProcesses(IProcessWrapper[] processes)
        {
            foreach (var p in processes)
            {
                int runtime = Convert.ToInt32((DateTime.Now - p.ProcessStartTime).ToString("mm"));
                if (runtime >= monitorInputData.MaxLifetime)
                {
                    p.Kill();
                    ConsoleLogger.Logger.LogInformation("Process {} has been killed\n", p.ProcessName);
                }
                else
                {
                    ConsoleLogger.Logger.LogInformation("Process {} has been running for {} minutes\n",
                        p.ProcessName, (DateTime.Now - p.ProcessStartTime).ToString("mm"));
                }
            }
        }

        public async Task MonitorProcessesAsync()
        {
            var timer = new PeriodicTimer(TimeSpan.FromMinutes(monitorInputData.MonitoringFrequency));

            do
            {
                IProcessWrapper[] processes = processContainer.GetProcesses(monitorInputData.Name);
                VerifyProcesses(processes);

                await Task.Delay(1);
            } while (await timer.WaitForNextTickAsync());
        }
    }
}

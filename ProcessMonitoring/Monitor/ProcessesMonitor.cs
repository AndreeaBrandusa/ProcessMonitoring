using Microsoft.Extensions.Logging;
using ProcessMonitoring.ConsoleUtils;
using ProcessMonitoring.Monitor.Data;

namespace ProcessMonitoring.Monitor
{
    public class ProcessesMonitor(MonitorInputData monitorInputData, IProcessContainer processContainer)
    {
        private readonly MonitorInputData monitorInputData = monitorInputData;
        private readonly IProcessContainer processContainer = processContainer;
        private CancellationToken cancellationToken;

        public CancellationToken Token { get => cancellationToken; set => cancellationToken = value; }


        public void VerifyProcesses(IProcessWrapper[] processes)
        {
            foreach (var process in processes)
            {
                int runtime = Convert.ToInt32((DateTime.Now - process.ProcessStartTime).ToString("mm"));
                if (runtime >= monitorInputData.MaxLifetime)
                {
                    process.Kill();
                    ConsoleLogger.Logger.LogInformation("Process {} has been killed\n", process.ProcessName);
                }
                else
                {
                    ConsoleLogger.Logger.LogInformation("Process {} has been running for {} minutes\n",
                        process.ProcessName, (DateTime.Now - process.ProcessStartTime).ToString("mm"));
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
            } while (await timer.WaitForNextTickAsync(cancellationToken));
        }
    }
}

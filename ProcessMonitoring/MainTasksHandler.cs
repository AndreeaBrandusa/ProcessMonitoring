using Microsoft.Extensions.Logging;
using ProcessMonitoring.ConsoleUtils;
using ProcessMonitoring.Monitor;
using ProcessMonitoring.Monitor.Data;

namespace ProcessMonitoring
{
    public class MainTasksHandler
    {
        private ConsoleListener? consoleListener;
        private ProcessesMonitor? processesMonitor;

        public async Task StartProcessesAsync(MonitorInputData monitorInputData)
        {
            ConsoleWrapper consoleWrapper = new();
            ProcessHandler processHandler = new();
            ProcessContainer processContainer = new(processHandler);

            consoleListener = new(consoleWrapper);
            processesMonitor = new(monitorInputData, processContainer);

            var keyTask = consoleListener.ListenForCloseKeyAsync();
            var processTask = processesMonitor.MonitorProcessesAsync();

            await Task.WhenAny(keyTask, processTask);
        }

        public async Task Start(string[] args)
        {
            if (args.Length < 3)
            {
                ConsoleLogger.Logger.LogError("Not enough args");
                return;
            }

            MonitorInputData monitorInputData = new(args[0], Convert.ToInt32(args[1]), Convert.ToInt32(args[2]));

            await StartProcessesAsync(monitorInputData);
        }
    }
}

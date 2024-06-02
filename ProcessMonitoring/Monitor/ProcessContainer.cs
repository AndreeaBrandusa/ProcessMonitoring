using Microsoft.Extensions.Logging;
using ProcessMonitoring.ConsoleUtils;
using ProcessMonitoring.Monitor.Data;

namespace ProcessMonitoring.Monitor
{
    public class ProcessContainer(IProcessHandler processHandler)
    {
        private readonly IProcessHandler processHandler = processHandler;
        public IProcessWrapper[] GetProcesses(string name)
        {
            // Get the process with the specified name
            IProcessWrapper[] processes = processHandler.GetProcessesByName(name);

            if (processes == null || processes.Length == 0)
            {
                ConsoleLogger.Logger.LogWarning("No process with the name {} was found.\n", name);
                return [];
            }

            ConsoleLogger.Logger.LogInformation("Process {} was found {} times.\n", name, processes.Length);
            return processes;
        }
    }
}

using System.Diagnostics;
using ProcessMonitoring.Monitor.Data;

namespace ProcessMonitoring.Monitor
{
    public class ProcessHandler : IProcessHandler
    {
        public IProcessWrapper[] GetProcessesByName(string name)
        {
            return Array.ConvertAll(Process.GetProcessesByName(name), process => new ProcessWrapper(process));
        }
    }
}

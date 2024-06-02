using ProcessMonitoring.Monitor.Data;

namespace ProcessMonitoring.Monitor
{
    public interface IProcessHandler
    {
        IProcessWrapper[] GetProcessesByName(string name);
    }
}

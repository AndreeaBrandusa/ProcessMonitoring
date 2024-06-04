using ProcessMonitoring.Monitor.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessMonitoring.Monitor
{
    public interface IProcessesContainer
    {
        IProcessWrapper[] GetProcesses(string name);
    }
}

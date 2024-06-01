using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessMonitoring
{
    public interface IProcessHandler
    {
        IProcessWrapper[] GetProcessesByName(string name);
    }
}

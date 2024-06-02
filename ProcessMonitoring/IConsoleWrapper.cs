using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessMonitoring
{
    public interface IConsoleWrapper
    {
        ConsoleKeyInfo ReadKey(bool intercept);
    }
}

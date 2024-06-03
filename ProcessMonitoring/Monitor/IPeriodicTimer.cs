using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessMonitoring.Monitor
{
    public interface IPeriodicTimer
    {
        ValueTask<bool> WaitForNextTickAsync(CancellationToken cancellationToken = default);
    }
}

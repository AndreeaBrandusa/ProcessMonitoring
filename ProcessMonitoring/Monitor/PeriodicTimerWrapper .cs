using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessMonitoring.Monitor
{
    public class PeriodicTimerWrapper(TimeSpan period) : IPeriodicTimer
    {
        private readonly PeriodicTimer _timer = new(period);

        public ValueTask<bool> WaitForNextTickAsync(CancellationToken cancellationToken = default)
        {
            return _timer.WaitForNextTickAsync(cancellationToken);
        }
    }
}

using System.Diagnostics;

namespace ProcessMonitoring
{
    public interface IProcessWrapper
    {
        string ProcessName { get; set; }
        DateTime ProcessStartTime { get; set;  }
        void Kill();
    }
}

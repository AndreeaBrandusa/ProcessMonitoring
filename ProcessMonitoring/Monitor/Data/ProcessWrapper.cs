using System.Diagnostics;

namespace ProcessMonitoring.Monitor.Data
{
    public class ProcessWrapper(Process process) : IProcessWrapper
    {
        private readonly Process _process = process;
        public string ProcessName = process.ProcessName;
        public DateTime ProcessStartTime = process.StartTime;

        DateTime IProcessWrapper.ProcessStartTime { get => ProcessStartTime; set => ProcessStartTime = value; }
        string IProcessWrapper.ProcessName { get => ProcessName; set => ProcessName = value; }
        public void Kill() => _process.Kill();
    }
}

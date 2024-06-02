namespace ProcessMonitoring.Monitor.Data
{
    public interface IProcessWrapper
    {
        string ProcessName { get; set; }
        DateTime ProcessStartTime { get; set; }
        void Kill();
    }
}

namespace ProcessMonitoring.Monitor.Data
{
    public class MonitorInputData(string name, int maxLifetime, int monitoringFrequency)
    {
        private readonly string name = name;
        private readonly int maxLifetime = maxLifetime;
        private readonly int monitoringFrequency = monitoringFrequency;

        public string Name { get => name; }
        public int MaxLifetime { get => maxLifetime; }
        public int MonitoringFrequency { get => monitoringFrequency; }
    }
}

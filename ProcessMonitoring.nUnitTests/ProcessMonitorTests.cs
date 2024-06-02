using Moq;
using ProcessMonitoring.Monitor;
using ProcessMonitoring.Monitor.Data;

namespace ProcessMonitoring.nUnitTests
{
    public class ProcessMonitorTests
    {
        
        private Mock<IProcessWrapper> mockProcessWrapper;
        private Mock<IProcessHandler> mockProcessHandler;

        [SetUp]
        public void Setup()
        {
            mockProcessWrapper = new Mock<IProcessWrapper>();
            mockProcessHandler = new Mock<IProcessHandler>();
        }

        [Test]
        public void VerifyProcesses_KillsProcessesExceedingMaxLifetime()
        {
            var processName = "notepad";
            int maxLifetime = 5;
            int monitoringFrequency = 1;
            var mockProcesses = new List<IProcessWrapper> { mockProcessWrapper.Object };
            var monitorInputData = new MonitorInputData(processName, maxLifetime, monitoringFrequency);

            mockProcessWrapper.Setup(p => p.ProcessStartTime).Returns(DateTime.Now.Subtract(new TimeSpan(0, 6, 0)));
            mockProcessWrapper.Setup(p => p.ProcessName).Returns(processName);
            mockProcessWrapper.Setup(p => p.Kill()).Verifiable();

            ProcessesMonitor processesMonitor = new(monitorInputData, mockProcessHandler.Object);
            processesMonitor.VerifyProcesses([.. mockProcesses]);

            mockProcessWrapper.Verify(p => p.Kill(), Times.Once);
        }

        [Test]
        public void VerifyProcesses_ProcessesRunningWithinMaxLifetime()
        {
            var processName = "notepad";
            int maxLifetime = 5;
            int monitoringFrequency = 1;
            var mockProcesses = new List<IProcessWrapper> { mockProcessWrapper.Object };
            var monitorInputData = new MonitorInputData(processName, maxLifetime, monitoringFrequency);

            mockProcessWrapper.Setup(p => p.ProcessStartTime).Returns(DateTime.Now.Subtract(new TimeSpan(0, 2, 0)));
            mockProcessWrapper.Setup(p => p.ProcessName).Returns(processName);
            mockProcessWrapper.Setup(p => p.Kill()).Verifiable();

            ProcessesMonitor processesMonitor = new(monitorInputData, mockProcessHandler.Object);
            processesMonitor.VerifyProcesses([.. mockProcesses]);

            mockProcessWrapper.Verify(p => p.Kill(), Times.Never);
        }

        public void MonitorProcessesAsync()
        {

        }
    }
}

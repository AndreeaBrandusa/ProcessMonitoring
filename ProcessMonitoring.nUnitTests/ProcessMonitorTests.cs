using Moq;
using ProcessMonitoring.Monitor;
using ProcessMonitoring.Monitor.Data;
using System.Diagnostics;

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

            var mockProcessContainer = new Mock<IProcessContainer>();
            var mockProcesses = new List<IProcessWrapper> { mockProcessWrapper.Object };
            var monitorInputData = new MonitorInputData(processName, maxLifetime, monitoringFrequency);

            mockProcessWrapper.Setup(p => p.ProcessStartTime).Returns(DateTime.Now.Subtract(new TimeSpan(0, 6, 0)));
            mockProcessWrapper.Setup(p => p.ProcessName).Returns(processName);
            mockProcessWrapper.Setup(p => p.Kill()).Verifiable();

            ProcessesMonitor processesMonitor = new(monitorInputData, mockProcessContainer.Object);
            processesMonitor.VerifyProcesses([.. mockProcesses]);

            mockProcessWrapper.Verify(p => p.Kill(), Times.Once);
        }

        [Test]
        public void VerifyProcesses_ProcessesRunningWithinMaxLifetime()
        {
            var processName = "notepad";
            int maxLifetime = 5;
            int monitoringFrequency = 1;

            var mockProcessContainer = new Mock<IProcessContainer>();
            var mockProcesses = new List<IProcessWrapper> { mockProcessWrapper.Object };
            var monitorInputData = new MonitorInputData(processName, maxLifetime, monitoringFrequency);

            mockProcessWrapper.Setup(p => p.ProcessStartTime).Returns(DateTime.Now.Subtract(new TimeSpan(0, 2, 0)));
            mockProcessWrapper.Setup(p => p.ProcessName).Returns(processName);
            mockProcessWrapper.Setup(p => p.Kill()).Verifiable();

            ProcessesMonitor processesMonitor = new(monitorInputData, mockProcessContainer.Object);
            processesMonitor.VerifyProcesses([.. mockProcesses]);

            mockProcessWrapper.Verify(p => p.Kill(), Times.Never);
        }

        [Test]
        public async Task MonitorProcessesAsync_VerifiesProcessesPeriodically()
        {
            var processName = "notepad";
            int maxLifetime = 5;
            int monitoringFrequency = 1;

            var mockProcessContainer = new Mock<IProcessContainer>();
            var mockProcesses = new List<IProcessWrapper> { mockProcessWrapper.Object };

            MonitorInputData monitorInputData = new(processName, maxLifetime, monitoringFrequency);
            mockProcessContainer.Setup(p => p.GetProcesses(processName)).Returns([.. mockProcesses]).Verifiable();

            CancellationTokenSource tokenSource = new();
            ProcessesMonitor processMonitor = new(monitorInputData, mockProcessContainer.Object)
            {
                Token = tokenSource.Token
            };

            var monitorProcessTask = processMonitor.MonitorProcessesAsync();
            await Task.Delay(1000);
            tokenSource.Cancel();

            mockProcessContainer.Verify(p => p.GetProcesses(processName), Times.AtLeastOnce());
        }
    }
}
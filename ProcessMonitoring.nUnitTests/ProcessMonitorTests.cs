using Moq;
using ProcessMonitoring.Monitor;
using ProcessMonitoring.Monitor.Data;
using System.Diagnostics;

namespace ProcessMonitoring.nUnitTests
{
    public class ProcessMonitorTests
    {
        
        private Mock<IProcessWrapper> mockProcessWrapper;

        [SetUp]
        public void Setup()
        {
            mockProcessWrapper = new Mock<IProcessWrapper>();
        }

        [Test]
        public async Task MonitorProcessesAsync_ProcessRunningWithinMaxLifetime()
        {
            var processName = "notepad";
            int maxLifetime = 5;
            int monitoringFrequency = 1;

            var mockProcessContainer = new Mock<IProcessesContainer>();
            var mockProcesses = new List<IProcessWrapper> { mockProcessWrapper.Object };

            MonitorInputData monitorInputData = new(processName, maxLifetime, monitoringFrequency);
            mockProcessWrapper.Setup(p => p.ProcessStartTime).Returns(DateTime.Now.Subtract(new TimeSpan(0, 2, 0)));
            mockProcessContainer.Setup(p => p.GetProcesses(processName)).Returns([.. mockProcesses]).Verifiable();

            CancellationTokenSource tokenSource = new();
            ProcessesMonitor processesMonitor = new(monitorInputData, mockProcessContainer.Object)
            {
                Token = tokenSource.Token
            };

            var monitorProcessTask = processesMonitor.MonitorProcessesAsync();
            await Task.Delay(1000);
            tokenSource.Cancel();

            mockProcessContainer.Verify(p => p.GetProcesses(processName), Times.AtLeastOnce());
            mockProcessWrapper.Verify(p => p.Kill(), Times.Never);
        }

        [Test]
        public async Task MonitorProcessesAsync_KillsProcessExceedingMaxLifetime()
        {
            var processName = "notepad";
            int maxLifetime = 5;
            int monitoringFrequency = 1;

            var mockProcessContainer = new Mock<IProcessesContainer>();
            var mockProcesses = new List<IProcessWrapper> { mockProcessWrapper.Object };

            MonitorInputData monitorInputData = new(processName, maxLifetime, monitoringFrequency);
            mockProcessWrapper.Setup(p => p.ProcessStartTime).Returns(DateTime.Now.Subtract(new TimeSpan(0, 6, 0)));
            mockProcessContainer.Setup(p => p.GetProcesses(processName)).Returns([.. mockProcesses]).Verifiable();

            CancellationTokenSource tokenSource = new();
            ProcessesMonitor processesMonitor = new(monitorInputData, mockProcessContainer.Object)
            {
                Token = tokenSource.Token
            };

            var monitorProcessTask = processesMonitor.MonitorProcessesAsync();
            await Task.Delay(1000);
            tokenSource.Cancel();

            mockProcessContainer.Verify(p => p.GetProcesses(processName), Times.AtLeastOnce());
            mockProcessWrapper.Verify(p => p.Kill(), Times.Once());
        }

        [Test]
        public async Task MonitorProcessesAsync_NoProcessFoundInitially()
        {
            var processName = "notepad";
            int maxLifetime = 5;
            int monitoringFrequency = 1;

            var mockProcessContainer = new Mock<IProcessesContainer>();
            var mockProcessWrapper2 = new Mock<IProcessWrapper>();
            var mockProcessWrapper3 = new Mock<IProcessWrapper>();
            var mockProcesses = new List<IProcessWrapper> 
            {
                mockProcessWrapper.Object,
                mockProcessWrapper2.Object,
                mockProcessWrapper3.Object,
            };

            MonitorInputData monitorInputData = new(processName, maxLifetime, monitoringFrequency);
            mockProcessWrapper.Setup(p => p.ProcessStartTime).Returns(DateTime.Now.Subtract(new TimeSpan(0, 6, 0)));
            mockProcessWrapper2.Setup(p2 => p2.ProcessStartTime).Returns(DateTime.Now.Subtract(new TimeSpan(0, 5, 0)));
            mockProcessWrapper3.Setup(p3 => p3.ProcessStartTime).Returns(DateTime.Now.Subtract(new TimeSpan(0, 3, 0)));

            mockProcessContainer.SetupSequence(p => p.GetProcesses(It.IsAny<String>()))
                .Returns([])
                .Returns([.. mockProcesses]);

            CancellationTokenSource tokenSource = new();
            ProcessesMonitor processesMonitor = new(monitorInputData, mockProcessContainer.Object)
            {
                Token = tokenSource.Token
            };

            var monitorProcessTask = processesMonitor.MonitorProcessesAsync();
            await Task.Delay(61000);
            tokenSource.Cancel();

            mockProcessContainer.Verify(p => p.GetProcesses(It.IsAny<String>()), Times.AtLeast(2));
            mockProcessWrapper.Verify(p => p.Kill(), Times.Once);
            mockProcessWrapper2.Verify(p2 => p2.Kill(), Times.Once);
            mockProcessWrapper3.Verify(p3 => p3.Kill(), Times.Never);
        }
    }
}
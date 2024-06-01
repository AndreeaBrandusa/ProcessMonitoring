using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Diagnostics;



namespace ProcessMonitoring.nUnitTests
{
    public class Tests
    {
        private Mock<ILogger> mockLogger;
        private Mock<IProcessHandler> mockProcessHandler;

        [SetUp]
        public void Setup()
        {
            mockLogger = new Mock<ILogger>();
            mockProcessHandler = new Mock<IProcessHandler>();
        }

        [Test]
        public void GetProcesses_ProcessesFound()
        {
            // Assign
            var processName = "notepad";
            var maxLifetime = 1;
            var monitoringFrequency = 1;

            var mockProcessWrapper = new Mock<IProcessWrapper>();
            var mockProcesses = new List<IProcessWrapper> { mockProcessWrapper.Object };
            var process = new MyProcess(processName, maxLifetime, monitoringFrequency, mockLogger.Object, mockProcessHandler.Object);
            mockProcessHandler.Setup(p => p.GetProcessesByName(processName)).Returns([.. mockProcesses]);

            // Act
            var result = process.GetProcesses();

            Assert.That(result, Is.Not.Empty);
        }

        [Test]
        public void GetProcesses_NoProcessFound()
        {
            // Assign
            var processName = "notepad";
            var maxLifetime = 1;
            var monitoringFrequency = 1;

            var process = new MyProcess(processName, maxLifetime, monitoringFrequency, mockLogger.Object, mockProcessHandler.Object);
            mockProcessHandler.Setup(p => p.GetProcessesByName(processName)).Returns([]);

            // Act
            var result = process.GetProcesses();

            Assert.That(result, Is.Empty);
        }


        [Test]
        public void VerifyProcesses_KillsProcessesExceedingMaxLifetime()
        {
            // Assign
            var processName = "notepad";
            var maxLifetime = 1;
            var monitoringFrequency = 1;

            var mockProcessWrapper = new Mock<IProcessWrapper>();
            var mockProcesses = new List<IProcessWrapper> { mockProcessWrapper.Object };
            var process = new MyProcess(processName, maxLifetime, monitoringFrequency, mockLogger.Object, mockProcessHandler.Object);

            mockProcessWrapper.Setup(p => p.ProcessStartTime).Returns(DateTime.Now.Subtract(new TimeSpan(0, 2, 0)));
            mockProcessWrapper.Setup(p => p.ProcessName).Returns(processName);
            mockProcessWrapper.Setup(p => p.Kill()).Verifiable();

            // Act
            process.VerifyProcesses([.. mockProcesses]);

            // Assert
            mockProcessWrapper.Verify(p => p.Kill(), Times.Once);
        }

        [Test]
        public void VerifyProcesses_ProcessesRunningWithinMaxLifetime()
        {
            // Assign
            var processName = "notepad";
            var maxLifetime = 5;
            var monitoringFrequency = 1;

            var mockProcessWrapper = new Mock<IProcessWrapper>();
            var mockProcesses = new List<IProcessWrapper> { mockProcessWrapper.Object };
            var process = new MyProcess(processName, maxLifetime, monitoringFrequency, mockLogger.Object, mockProcessHandler.Object);

            mockProcessWrapper.Setup(p => p.ProcessStartTime).Returns(DateTime.Now.Subtract(new TimeSpan(0, 2, 0)));
            mockProcessWrapper.Setup(p => p.ProcessName).Returns(processName);
            mockProcessWrapper.Setup(p => p.Kill()).Verifiable();

            // Act
            process.VerifyProcesses([.. mockProcesses]);

            // Assert
            mockProcessWrapper.Verify(p => p.Kill(), Times.Never);
        }

        [Test]
        public void KeyPressed_StoppesOnQPressed()
        {

        }
    }
}
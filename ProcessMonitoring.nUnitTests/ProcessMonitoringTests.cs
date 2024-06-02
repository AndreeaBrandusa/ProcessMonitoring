using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Moq;
using System;
using System.Diagnostics;



namespace ProcessMonitoring.nUnitTests
{
    public class Tests
    {
        private Mock<ILogger> mockLogger;
        private Mock<IProcessHandler> mockProcessHandler;
        private Mock<IConsoleWrapper> mockConsoleWrapper;

        [SetUp]
        public void Setup()
        {
            mockLogger = new Mock<ILogger>();
            mockProcessHandler = new Mock<IProcessHandler>();
            mockConsoleWrapper = new Mock<IConsoleWrapper>();
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
            var process = new MyProcess(processName, maxLifetime, monitoringFrequency, mockLogger.Object, mockProcessHandler.Object, mockConsoleWrapper.Object);
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

            var process = new MyProcess(processName, maxLifetime, monitoringFrequency, mockLogger.Object, mockProcessHandler.Object, mockConsoleWrapper.Object);
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
            var process = new MyProcess(processName, maxLifetime, monitoringFrequency, mockLogger.Object, mockProcessHandler.Object, mockConsoleWrapper.Object);

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
            var process = new MyProcess(processName, maxLifetime, monitoringFrequency, mockLogger.Object, mockProcessHandler.Object, mockConsoleWrapper.Object);

            mockProcessWrapper.Setup(p => p.ProcessStartTime).Returns(DateTime.Now.Subtract(new TimeSpan(0, 2, 0)));
            mockProcessWrapper.Setup(p => p.ProcessName).Returns(processName);
            mockProcessWrapper.Setup(p => p.Kill()).Verifiable();

            // Act
            process.VerifyProcesses([.. mockProcesses]);

            // Assert
            mockProcessWrapper.Verify(p => p.Kill(), Times.Never);
        }

        [Test]
        public async Task KeyPressed_ListensForCorrectKey()
        {
            var processName = "notepad";
            var maxLifetime = 5;
            var monitoringFrequency = 1;

            var process = new MyProcess(processName, maxLifetime, monitoringFrequency, mockLogger.Object, mockProcessHandler.Object, mockConsoleWrapper.Object);

            mockConsoleWrapper.SetupSequence(c => c.ReadKey(true))
                              .Returns(new ConsoleKeyInfo('a', ConsoleKey.Q, false, false, false))
                              .Returns(new ConsoleKeyInfo('q', ConsoleKey.Q, false, false, false));
            await process.KeyPressed();

            mockConsoleWrapper.Verify(p => p.ReadKey(true), Times.Once);
        }
    }
}
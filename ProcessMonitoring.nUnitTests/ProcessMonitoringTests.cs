using Microsoft.Extensions.Logging;
using System.Diagnostics;



namespace ProcessMonitoring.nUnitTests
{
    public class Tests
    {
        private readonly string name = "notepad";
        private Process watchedProcess;

        [SetUp]
        public void Setup()
        {
            watchedProcess = Process.Start(name);
        }

        [TearDown]
        public void Teardown()
        {
            watchedProcess.Kill();
            watchedProcess.Dispose();
        }

        [Test]
        public void VerifyProcesses_KillsProcessesExceedingMaxLifetime()
        {
            // Assign
            int maxLifetime = 1;
            int monitoringFrequency = 1;

            ILogger logger = MyProcess.CreateLogger();
            MyProcess process = new(name, maxLifetime, monitoringFrequency, logger);

            // Act
            Thread.Sleep(60000);
            process.VerifyProcesses();

            // Assert
            Assert.That(watchedProcess.HasExited, Is.True);
        }

        [Test]
        public void VerifyProcesses_ProcessesNotExceedingMaxLifetime()
        {
            // Assign
            int maxLifetime = 5;
            int monitoringFrequency = 1;

            ILogger logger = MyProcess.CreateLogger();
            MyProcess process = new(name, maxLifetime, monitoringFrequency, logger);

            // Act
            Thread.Sleep(1000);
            process.VerifyProcesses();

            // Assert
            Assert.That(watchedProcess.HasExited, Is.False);
        }
    }
}
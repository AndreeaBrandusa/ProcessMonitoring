using Moq;
using ProcessMonitoring.ConsoleUtils;
using ProcessMonitoring.Monitor;
using ProcessMonitoring.Monitor.Data;

namespace ProcessMonitoring.nUnitTests
{
    public class ProcessContainerTests
    {
        private Mock<IProcessHandler> mockProcessHandler;
        private Mock<IProcessWrapper> mockProcessWrapper;

        [SetUp]
        public void Setup()
        {
            mockProcessHandler = new Mock<IProcessHandler>();
            mockProcessWrapper = new Mock<IProcessWrapper>();
        }

        [Test]
        public void GetProcesses_ProcessesFound()
        {
            var processName = "notepad";
            var mockProcesses = new List<IProcessWrapper> { mockProcessWrapper.Object };
            
            mockProcessHandler.Setup(p => p.GetProcessesByName(processName)).Returns([.. mockProcesses]);
            ProcessesContainer processContainer = new(mockProcessHandler.Object);

            var result = processContainer.GetProcesses(processName);

            Assert.That(result, Is.Not.Empty);
        }

        [Test]
        public void GetProcesses_NoProcessFound()
        {
            var processName = "notepad";

            mockProcessHandler.Setup(p => p.GetProcessesByName(processName)).Returns([]);
            ProcessesContainer processContainer = new(mockProcessHandler.Object);

            var result = processContainer.GetProcesses(processName);

            Assert.That(result, Is.Empty);
        }
    }
}

using Moq;
using ProcessMonitoring.ConsoleUtils;

namespace ProcessMonitoring.nUnitTests
{
    public class Tests
    {
        private Mock<IConsoleWrapper> mockConsoleWrapper;

        [SetUp]
        public void Setup()
        {
            mockConsoleWrapper = new Mock<IConsoleWrapper>();
        }
        [Test]
        public async Task ListenForCloseKeyAsync_WhenCloseKeyIsPressed_TaskFinish()
        {
            mockConsoleWrapper.SetupSequence(c => c.ReadKey(true))
                              .Returns(new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false))
                              .Returns(new ConsoleKeyInfo('s', ConsoleKey.S, false, false, false))
                              .Returns(new ConsoleKeyInfo('q', ConsoleKey.Q, false, false, false));

            ConsoleListener consoleListener = new(mockConsoleWrapper.Object);
            var listenTask = consoleListener.ListenForCloseKeyAsync();

            int timeout = 1000;
            var finishedTask = await Task.WhenAny(listenTask, Task.Delay(timeout));

            Assert.That(finishedTask, Is.EqualTo(listenTask));
        }

        [Test]
        public async Task ListenForCloseKeyAsync_WhenOtherKeyIsPressed_TaskDoesNotFinish()
        {
            mockConsoleWrapper.SetupSequence(c => c.ReadKey(true))
                              .Returns(new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false));

            ConsoleListener consoleListener = new(mockConsoleWrapper.Object);
            var listenTask = consoleListener.ListenForCloseKeyAsync();

            int timeout = 1000;
            var delayTask = Task.Delay(timeout);
            var finishedTask = await Task.WhenAny(listenTask, delayTask);

            Assert.That(finishedTask, Is.EqualTo(delayTask));
        }
    }
}
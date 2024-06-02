using Microsoft.Extensions.Logging;

namespace ProcessMonitoring.ConsoleUtils
{
    public class ConsoleListener(IConsoleWrapper consoleWrapper)
    {
        private readonly IConsoleWrapper consoleWrapper = consoleWrapper;

        public async Task ListenForCloseKeyAsync()
        {
            ConsoleLogger.Logger.LogInformation("Press q to stop listening");
            do
            {
                await Task.Delay(1);
            } while (consoleWrapper.ReadKey(true).Key != ConsoleKey.Q);
        }
    }
}

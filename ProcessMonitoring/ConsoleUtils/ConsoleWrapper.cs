namespace ProcessMonitoring.ConsoleUtils
{
    public class ConsoleWrapper() : IConsoleWrapper
    {
        public ConsoleKeyInfo ReadKey(bool intercept)
        {
            return Console.ReadKey(intercept);
        }
    }
}

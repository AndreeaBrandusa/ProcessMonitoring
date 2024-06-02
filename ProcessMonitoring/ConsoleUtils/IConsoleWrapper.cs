namespace ProcessMonitoring.ConsoleUtils
{
    public interface IConsoleWrapper
    {
        ConsoleKeyInfo ReadKey(bool intercept);
    }
}

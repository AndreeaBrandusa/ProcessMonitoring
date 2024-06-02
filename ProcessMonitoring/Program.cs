namespace ProcessMonitoring
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            MainTasksHandler mainTasksHandler = new();
            await mainTasksHandler.Start(args);
        }
    }
}
namespace RetryMachine;

public interface IRetryMachineRunner
{
    Task CreateTasks<T>(string actionName, T value, bool runImmediately = false);
    Task CreateTasks(string actionName, string value, bool runImmediately = false);
    Task CreateTasks(RetryCreate task);
    Task CreateTasks(List<RetryCreate> tasks);

    Task<int> PerformTasks();
}

public class RetrySettings
{
    public int DelayInSeconds { get; set; }
}

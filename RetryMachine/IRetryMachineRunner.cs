namespace RetryMachine;

public interface IRetryMachineRunner
{
    Task CreateTasks<T>(string actionName, T value, bool runImmediately = false,
        string taskName = "", string? taskId = null);
    Task CreateTasks(string actionName, string value, bool runImmediately = false,
        string taskName = "", string? taskId = null);
    Task CreateTasks(RetryAction tasks, string taskName = "", string? taskId = null);
    Task CreateTasks(List<RetryAction> tasks, string taskName = "", string? taskId = null);

    Task<int> PerformTasks();

    Task<int> PerformTasks(List<RetryFlow> flows);
}

public class RetrySettings
{
    public int DelayInSeconds { get; set; }
}

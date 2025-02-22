namespace RetryMachine;

public interface IRetryMachine
{
    Task CreateTasks<T>(string actionName, T value, bool runImmediately = false, RetrySettings? settings = null);
    Task CreateTasks(string actionName, string value, bool runImmediately = false, RetrySettings? settings = null);

    Task CreateTasks(RetryCreate task, RetrySettings? settings = null);
    Task CreateTasks(List<RetryCreate> tasks, RetrySettings? settings = null);


    Task<int> PerformTasks();
}

public class RetrySettings
{
    public int DelayInSeconds { get; set; }
}

namespace RetryMachine;

public interface IRetryMachine
{
    Task CreateTasks<T>( string actionName, T value);
    Task CreateTasks( string actionName, string value);

    Task CreateTasks(RetryCreate task);
    Task CreateTasks(List<RetryCreate> tasks);

    //Task CreateTasks(Dictionary<string, object> tasks);
    //Task CreateTasks(Dictionary<string, string> tasks);

    Task PerformTasks();
}
 
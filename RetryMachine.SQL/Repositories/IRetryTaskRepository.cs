using RetryMachine.SQL.Models;

namespace RetryMachine.SQL.Repositories;

public interface IRetryTaskRepository
{
    Task<List<RetryTask>> FindTasksToRun();
    Task<RetryTask?> GetSingleAsync(long id);
    Task Create(RetryTask taskModel);
    Task Edit(RetryTask taskModel);
}
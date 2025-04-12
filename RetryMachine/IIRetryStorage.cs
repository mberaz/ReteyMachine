namespace RetryMachine;

public interface IRetryStorage
{
    public Task Save(RetryTask retryTaskModel);
    public Task Update(RetryTask retryTaskModel);
    public Task<List<RetryTask>> Get();
}


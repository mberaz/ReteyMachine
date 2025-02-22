namespace RetryMachine;

public interface IRetryStorage
{
    public Task Save(RetryTaskModel retryTaskModel);
    public Task Update(RetryTaskModel retryTaskModel);
    public Task<List<RetryTaskModel>> Get();
}


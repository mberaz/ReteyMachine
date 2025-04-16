namespace RetryMachine;

public interface IRetryStorage
{
    public Task Save(RetryFlow retryFlowModel);
    public Task Update(RetryFlow retryFlowModel);
    public Task<List<RetryFlow>> Get();
}


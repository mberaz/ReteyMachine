namespace RetryMachine.Api.RetryMachine;

public interface IRetryStorage
{
    public Task Save(RetryTask retryTask);
    public Task Update(RetryTask retryTask);
    public Task<List<RetryTask>> Get();
}

public class RetryStorage : IRetryStorage
{
    public async Task Save(RetryTask retryTask)
    {
        
    }

    public async Task Update(RetryTask retryTask)
    {
        
    }

    public Task<List<RetryTask>> Get()
    {
        //query where(t=>t.Status!= (int)RetryStatus.Done && t.ActionOn<DateTime.Now
        var list = new List<RetryTask>();
        return Task.FromResult(list);
    }
}
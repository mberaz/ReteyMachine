namespace RetryMachine.Api.Sevices;

public interface IMonitoringService
{
    Task Monitor(int actionId );
}

public class MonitoringService:IMonitoringService
{
    public Task Monitor(int actionId)
    {
       return Task.CompletedTask;
    }
}
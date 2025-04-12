namespace RetryMachine.Api.Sevices
{
    public interface ILogService
    {
        Task WriteLog(int accountId, string template);
    }
    public class LogService:ILogService
    {
        public Task WriteLog(int accountId, string template)
        {
            return Task.CompletedTask;
        }
    }
}

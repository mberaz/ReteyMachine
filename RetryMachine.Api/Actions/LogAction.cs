using Newtonsoft.Json;
using RetryMachine.Api.Sevices;

namespace RetryMachine.Api.Actions
{
    public class LogSettings
    {
        public int AccountId { get; set; }
        public string Template { get; set; }
    }

    public class LogAction : IRetryable
    {
        private readonly ILogService _logService;

        public LogAction(ILogService logService)
        {
            _logService = logService;
        }

        public const string ActionName = "LogAction";
        public string Name()
        {
            return ActionName;
        }

        public async Task<(bool isOk, string? error)> Perform(string value, string taskName = "", string? taskId = null)
        {
            var settings = JsonConvert.DeserializeObject<LogSettings>(value);

            try
            {
                await _logService.WriteLog(settings.AccountId, settings.Template);
                return (true, null);
            }
            catch (Exception e)
            {
                return (false, e.Message);
            }
        }
    }
}

using Newtonsoft.Json;

namespace RetryMachine.Api.Actions
{
    public class AutoLogSettings
    {
        public int AccountHolderId { get; set; }
        public string Template { get; set; }
    }

    public class AutoLogAction : IRetryable
    {
        public const string ActionName = "AutoLogAction";
        public string Name()
        {
            return ActionName;
        }

        public Task<(bool isOk, string? error)> Perform(string value, string taskName = "", string? taskId = null)
        {
            var settings = JsonConvert.DeserializeObject<AutoLogSettings>(value);

            try
            {
                return Task.FromResult<(bool isOk, string? error)>((true, null));
            }
            catch (Exception e)
            {
                return Task.FromResult<(bool isOk, string? error)>((true, e.Message));
            }
        }
    }
}

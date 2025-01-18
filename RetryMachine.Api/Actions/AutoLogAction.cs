using Newtonsoft.Json;
using RetryMachine.Api.RetryMachine;

namespace RetryMachine.Api.Actions
{
    public class AutoLogSettings
    {
        public int AccountHolderId { get; set; }
        public string Template { get; set; }
    }

    public class AutoLogAction:IRetryable
    {
        public const string ActionName = "AutoLogAction";
        public string Name()
        {
            return ActionName;
        }

        public Task<(bool isOk, string error)> Perform(string value)
        {
            var settings = JsonConvert.DeserializeObject<AutoLogSettings>(value);

            try
            {
                return Task.FromResult<(bool isOk, string error)>((true, null));
            }
            catch (Exception e)
            {
                return Task.FromResult((false, e.Message));
            }
        }
    }
}

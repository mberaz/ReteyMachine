using Newtonsoft.Json;

namespace RetryMachine.Api.Actions;

public class UserActionLogSettings
{
    public int ActionId { get; set; }
}

public class UserActionLogAction:IRetryable
{
    public const string ActionName = "UserActionLogAction";
    public string Name()
    {
        return ActionName;
    }

    public Task<(bool isOk, string? error)> Perform(string value)
    {
        var settings = JsonConvert.DeserializeObject<UserActionLogSettings>(value);

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
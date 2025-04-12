using Newtonsoft.Json;
using RetryMachine.Api.Sevices;

namespace RetryMachine.Api.Actions;

public class MonitoringSettings
{
    public int ActionId { get; set; }
}

public class MonitoringAction : IRetryable
{
    private readonly IMonitoringService _monitoringService;

    public MonitoringAction(IMonitoringService monitoringService)
    {
        _monitoringService = monitoringService;
    }

    public const string ActionName = "MonitoringAction";
    public string Name()
    {
        return ActionName;
    }

    public async Task<(bool isOk, string? error)> Perform(string value, string taskName = "", string? taskId = null)
    {

        var settings = JsonConvert.DeserializeObject<MonitoringSettings>(value);

        try
        {
            await _monitoringService.Monitor(settings.ActionId);
            return (true, null);
        }
        catch (Exception e)
        {
            return (false, e.Message);
        }
    }
}
using Newtonsoft.Json;

namespace RetryMachine;

public class RetryAction
{
    public RetryAction(string taskName, object value, int order, bool runImmediately = false) :
        this(taskName, JsonConvert.SerializeObject(value), order, runImmediately)
    {
    }

    public RetryAction(string taskName, string settings, int order, bool runImmediately = false)
    {
        TaskName = taskName;
        Settings = settings;
        Order = order;
        RunImmediately = runImmediately;
    }

    public string TaskName { get; set; }
    public string Settings { get; set; }
    public int Order { get; set; }
    public bool RunImmediately { get; set; }
}
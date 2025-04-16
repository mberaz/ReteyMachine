namespace RetryMachine;

public class RetryFlow
{
    public int RetryCount { get; set; }
    public int Status { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
    public string ActionOrder { get; set; }
    //actions that we need to execute
    public string NextActions { get; set; }
    //actions that are completed
    public string CompletedActions { get; set; }
    //actions that failed to run
    public string FailedActions { get; set; }
    public string TaskName {get;set;}
    public string TaskId {get;set;}

    public List<string> RequiredTasks {get;set;}

    //RetryFlow id in our selected method of storage
    public string? ExternalId {get;set;}
}
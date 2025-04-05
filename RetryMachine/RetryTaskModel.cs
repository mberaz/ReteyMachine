namespace RetryMachine;

public class RetryTaskModel
{
    public int RetryCount { get; set; }
    public int Status { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
    public DateTime ActionOn { get; set; }
    public string ActionOrder { get; set; }
    public string NextActions { get; set; }
    public string CompletedActions { get; set; }
    public string FailedActions { get; set; }

    public string TaskName {get;set;}
    public string TaskId {get;set;}

    public string? ExternalId {get;set;}
}
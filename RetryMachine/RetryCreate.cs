using Newtonsoft.Json;

namespace RetryMachine
{
    public class RetryCreate
    {
        public RetryCreate(string taskName, object value, int order, bool runImmediately = false) :
            this(taskName, JsonConvert.SerializeObject(value), order, runImmediately)
        {
        }

        public RetryCreate(string taskName, string value, int order, bool runImmediately = false)
        {
            TaskName = taskName;
            Value = value;
            Order = order;
            RunImmediately = runImmediately;
        }

        public string TaskName { get; set; }
        public string Value { get; set; }
        public int Order { get; set; }
        public bool RunImmediately { get; set; }
    }
}

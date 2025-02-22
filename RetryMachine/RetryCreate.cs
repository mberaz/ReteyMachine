using Newtonsoft.Json;

namespace RetryMachine
{
    public class RetryCreate
    {
        public RetryCreate(string taskName, object value, int order) : this(taskName, JsonConvert.SerializeObject(value), order)
        {
        }

        public RetryCreate(string taskName, string value, int order)
        {
            TaskName = taskName;
            Value = value;
            Order = order;
        }

        public string TaskName { get; set; }
        public string Value { get; set; }
        public int Order { get; set; }
    }
}

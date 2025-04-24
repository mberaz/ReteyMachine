using System.Collections.Generic;
using System.Threading.Tasks;

namespace RetryMachine.Framework
{
    public interface IRetryMachineRunner
    {
        Task CreateTasks<T>(string actionName, T value, bool runImmediately = false, string taskName = "", string taskId = null);
        Task CreateTasks(string actionName, string value, bool runImmediately = false, string taskName = "", string taskId = null);
        Task CreateTasks(RetryCreate tasks, string taskName = "", string taskId = null);
        Task CreateTasks(List<RetryCreate> tasks, string taskName = "", string taskId = null);

        Task<int> PerformTasks();
    }

    public class RetrySettings
    {
        public int DelayInSeconds { get; set; }
    }
}

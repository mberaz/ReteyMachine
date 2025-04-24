using System.Threading.Tasks;

namespace RetryMachine.Framework
{
    public interface IRetryable
    {
        string Name();
        Task<(bool isOk, string error)> Perform(string value, string taskName = "", string taskId = null);
    }
}

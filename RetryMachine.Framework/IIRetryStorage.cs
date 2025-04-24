using System.Collections.Generic;
using System.Threading.Tasks;

namespace RetryMachine.Framework
{
    public interface IRetryStorage
    {
        Task Save(RetryTaskModel retryTaskModel);
        Task Update(RetryTaskModel retryTaskModel);
        Task<List<RetryTaskModel>> Get();
    }
}


using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RetryMachine
{
    public class RetryMachineRunner : IRetryMachine
    {
        private IRetryStorage _storage;
        private List<IRetryable> _possibleActions;

        public RetryMachineRunner(IRetryStorage storage, IEnumerable<IRetryable> possibleActions)
        {
            _storage = storage;
            _possibleActions = possibleActions.ToList();
        }

        public Task CreateTasks<T>(string actionName, T value)
        {
            return CreateTasks([new RetryCreate(actionName, JsonConvert.SerializeObject(value), 1)]);
        }

        public Task CreateTasks(string actionName, string value)
        {
            return CreateTasks([new RetryCreate(actionName, value, 1)]);
        }

        public Task CreateTasks(RetryCreate task)
        {
            return CreateTasks([task]);
        }

        public async Task CreateTasks(List<RetryCreate> tasks)
        {
            var actions = new JObject();
            var actionOrder = new JObject();

            foreach (var task in tasks.OrderBy(o => o.Order))
            {
                actions[task.TaskName] = task.Value;
                actionOrder[task.TaskName] = task.Order;
            }

            await _storage.Save(new RetryTaskModel
            {
                CreatedOn = DateTime.Now,
                Status = (int)RetryStatus.Pending,
                ActionOn = DateTime.Now.AddSeconds(30),
                NextActions = JsonConvert.SerializeObject(actions),
                ActionOrder = JsonConvert.SerializeObject(actionOrder)
            });
        }

        public async Task PerformTasks()
        {
            var taskList = await _storage.Get();

            foreach (var task in taskList)
            {
                await DoTaskInner(task);
            }
        }

        private async Task DoTaskInner(RetryTaskModel retryTaskModel)
        {
            var actionOrder = JsonConvert.DeserializeObject<Dictionary<string, int>>(retryTaskModel.ActionOrder);
            var nextActions = ToDictionary(retryTaskModel.NextActions);
            var completedDictionary = ToDictionary(retryTaskModel.CompletedActions);
            var failedActions = ToDictionary(retryTaskModel.FailedActions);

            foreach (var order in nextActions.OrderBy(o => o.Value))
            {
                var action = nextActions[order.Key];
                var taskToDo = _possibleActions.FirstOrDefault(f => f.Name() == order.Key);
                var result = await taskToDo.Perform(action);

                if (result.isOk)
                {
                    actionOrder.Remove(order.Key);
                    nextActions.Remove(order.Key);
                    completedDictionary[order.Key] = action;
                    failedActions.Remove(order.Key);//remove this from the failed list if it passed now
                }
                else
                {
                    failedActions[order.Key] = result.error;
                    retryTaskModel.Status = (int)RetryStatus.Error;
                    retryTaskModel.RetryCount += 1;
                    retryTaskModel.ActionOn = DateTime.Now.AddSeconds(30 * retryTaskModel.RetryCount);
                }

                if (nextActions.Count == 0)
                {
                    retryTaskModel.Status = (int)RetryStatus.Done;
                }

                retryTaskModel.UpdatedOn = DateTime.Now;
                retryTaskModel.NextActions = JsonConvert.SerializeObject(nextActions);
                retryTaskModel.CompletedActions = JsonConvert.SerializeObject(completedDictionary);
                retryTaskModel.FailedActions = JsonConvert.SerializeObject(failedActions);

                await _storage.Update(retryTaskModel);
            }
        }

        private Dictionary<string, string> ToDictionary(string? values)
        {
            return values == null ?
                new Dictionary<string, string>() :
                JsonConvert.DeserializeObject<Dictionary<string, string>>(values);
        }
    }
}

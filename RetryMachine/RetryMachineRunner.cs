using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RetryMachine
{
    public class RetryMachineRunner : IRetryMachineRunner
    {
        private readonly IRetryStorage _storage;
        private readonly List<IRetryable> _possibleActions;

        public RetryMachineRunner(IRetryStorage storage, IEnumerable<IRetryable> possibleActions)
        {
            _storage = storage;
            _possibleActions = possibleActions.ToList();
        }

        public Task CreateTasks<T>(string actionName, T value, bool runImmediately = false, string taskName = "", string? taskId = null)
        {
            return CreateTasks(
                [new RetryCreate(actionName, JsonConvert.SerializeObject(value), 1, runImmediately)],
                taskName, taskId);
        }

        public Task CreateTasks(string actionName, string value, bool runImmediately = false, string taskName = "", string? taskId = null)
        {
            return CreateTasks([new RetryCreate(actionName, value, 1, runImmediately)], taskName, taskId);
        }

        public Task CreateTasks(RetryCreate tasks, string taskName = "", string? taskId = null)
        {
            return CreateTasks([tasks], taskName, taskId);
        }

        public async Task CreateTasks(List<RetryCreate> tasks, string taskName = "", string? taskId = null)
        {
            var nextActions = new JObject();
            var actionOrder = new JObject();

            var completedDictionary = new Dictionary<string, string>();
            var failedActions = new Dictionary<string, string>();

            var status = (int)RetryStatus.Pending;

            taskId ??= Guid.NewGuid().ToString("N");

            foreach (var task in tasks.OrderBy(o => o.Order))
            {
                if (task.RunImmediately)
                {
                    var taskToDo = _possibleActions.FirstOrDefault(f => f.Name() == task.TaskName);
                    if (taskToDo == null)
                    {
                        throw new Exception($"Could not find a IRetryable implementation to execute the task: {task.TaskName}");
                    }

                    var result = await taskToDo.Perform(task.Settings, taskName, taskId);

                    if (result.isOk)
                    {
                        //do not add to the actionOrder or actions object tasks that are done
                        completedDictionary[task.TaskName] = task.Settings;
                    }
                    else
                    {
                        failedActions[task.TaskName] = result.error;
                        nextActions[task.TaskName] = task.Settings;
                        actionOrder[task.TaskName] = task.Order;
                        status = (int)RetryStatus.Error; //if we fail on the task that runs immediately we start in an error status
                    }
                }
                else
                {
                    nextActions[task.TaskName] = task.Settings;
                    actionOrder[task.TaskName] = task.Order;
                }
            }

            await _storage.Save(new RetryTask
            {
                TaskName = taskName,
                TaskId = taskId,
                //if we executed all the tasks, then we are done
                Status = nextActions.HasValues ? status : (int)RetryStatus.Done,
                CreatedOn = DateTime.Now,
                NextActions = JsonConvert.SerializeObject(nextActions),
                ActionOrder = JsonConvert.SerializeObject(actionOrder),
                CompletedActions = JsonConvert.SerializeObject(completedDictionary),
                FailedActions = JsonConvert.SerializeObject(failedActions)
            });
        }

        public async Task<int> PerformTasks()
        {
            var taskList = await _storage.Get();

            foreach (var task in taskList)
            {
                await DoTaskInner(task);
            }

            return taskList.Count;
        }

        private async Task DoTaskInner(RetryTask retryTaskModel)
        {
            var actionOrder = JsonConvert.DeserializeObject<Dictionary<string, int>>(retryTaskModel.ActionOrder);
            var nextActions = ToDictionary(retryTaskModel.NextActions);
            var completedDictionary = ToDictionary(retryTaskModel.CompletedActions);
            var failedActions = ToDictionary(retryTaskModel.FailedActions);

            foreach (var order in actionOrder.OrderBy(o => o.Value))
            {
                var action = nextActions[order.Key];
                var taskToDo = _possibleActions.FirstOrDefault(f => f.Name() == order.Key);

                if (taskToDo == null)
                {
                    throw new Exception($"Could not find a IRetryable implementation to execute the task: {order.Key}");
                }

                var result = await taskToDo.Perform(action, retryTaskModel.TaskName, retryTaskModel.TaskId);

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
                }
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

        private Dictionary<string, string> ToDictionary(string? values)
        {
            return values == null ?
                new Dictionary<string, string>() :
                JsonConvert.DeserializeObject<Dictionary<string, string>>(values);
        }
    }
}

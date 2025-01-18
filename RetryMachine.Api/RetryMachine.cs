namespace RetryMachine.Api
{
    public interface IIRetryStorage
    {
        public Task Save(RetryTask retryTask);
        public Task Update(RetryTask retryTask);
        public Task<List<RetryTask>> Get();
    }

    public interface IRetryMachine
    {
        Task CreateTasks(Dictionary<string, string> tasks);
    }

    public class RetryMachine : IRetryMachine
    {
        private IIRetryStorage _storage;
        private List<IRetryable> _possibleActions;

        public RetryMachine(IIRetryStorage storage, IEnumerable<IRetryable> possibleActions)
        {
            _storage = storage;
            _possibleActions = possibleActions.ToList();
        }

        public async Task CreateTasks(Dictionary<string, string> tasks)
        {
            var actions = new JObject();
            foreach (var task in tasks)
            {
                actions[task.Key] = task.Value;
            }

            await _storage.Save(new RetryTask
            {
                Status = (int)RetryStatus.Pending,
                CreatedOn = DateTime.Now,
                ActionOn = DateTime.Now.AddSeconds(30),
                NextActions = actions.ToString()
            });
        }

        public async Task PerformTasks()
        {
            var taskList = await _storage.Get();

            foreach (var task in taskList)
            {

            }
        }

        private async Task DoTaskInner(RetryTask retryTask)
        {
            var actionsActions = ToDictionary(retryTask.NextActions);
            var completedDictionary = ToDictionary(retryTask.CompletedActions);
            var failedActions = ToDictionary(retryTask.FailedActions);

            foreach (var action in actionsActions)
            {
                var taskToDo = _possibleActions.FirstOrDefault(f => f.Name() == action.Key);
                var result = await taskToDo.Perform(action.Value);

                if (result.isOk)
                {
                    actionsActions.Remove(action.Key);
                    completedDictionary[action.Key] = action.Value;
                    failedActions.Remove(action.Key);//remove this from the failed list if it passed now
                }
                else
                {
                    failedActions[action.Key] = result.error;
                    retryTask.Status = (int)RetryStatus.Error;
                    retryTask.RetryCount += 1;
                    retryTask.ActionOn = DateTime.Now.AddSeconds(30 * retryTask.RetryCount);
                }

                if (actionsActions.Count == 0)
                {
                    retryTask.Status = (int)RetryStatus.Done;
                }
                retryTask.UpdatedOn = DateTime.Now;
                retryTask.NextActions = actionsActions.ToString();
                retryTask.CompletedActions = completedDictionary.ToString();
                retryTask.FailedActions = failedActions.ToString();

                await _storage.Update(retryTask);
            }
        }

        private Dictionary<string, string> ToDictionary(string values)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(values);
        }
    }
}

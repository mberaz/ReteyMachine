using RetryMachine.SQL.Models;
using RetryMachine.SQL.Repositories;

namespace RetryMachine.SQL.Storage
{
    public class RetryStorage : IRetryStorage
    {
        private readonly IRetryTaskRepository _repository;

        public RetryStorage(IRetryTaskRepository repository)
        {
            _repository = repository;
        }

        public Task Save(RetryTaskModel retryTaskModel)
        {
            return _repository.Create(ToTask(retryTaskModel));
        }

        public Task Update(RetryTaskModel retryTaskModel)
        {
            return _repository.Edit(ToTask(retryTaskModel));
        }

        public async Task<List<RetryTaskModel>> Get()
        {
            var list = await _repository.FindTasksToRun();

            return list.Select(t => ToModel(t)).ToList();
        }

        private RetryTask ToTask(RetryTaskModel model)
        {
            return new RetryTask
            {
                Id = model.Id,
                ActionOn = model.ActionOn,
                CompletedActions = model.CompletedActions,
                CreatedOn = model.CreatedOn,
                UpdatedOn = model.UpdatedOn,
                FailedActions = model.FailedActions,
                NextActions = model.NextActions,
                RetryCount = model.RetryCount,
                Status = model.Status,
                ActionOrder = model.ActionOrder
            };
        }

        private RetryTaskModel ToModel(RetryTask task)
        {
            return new RetryTaskModel
            {
                Id = task.Id,
                ActionOn = task.ActionOn,
                CompletedActions = task.CompletedActions,
                CreatedOn = task.CreatedOn,
                UpdatedOn = task.UpdatedOn,
                FailedActions = task.FailedActions,
                NextActions = task.NextActions,
                RetryCount = task.RetryCount,
                Status = task.Status,
                ActionOrder = task.ActionOrder
            };
        }
    }
}

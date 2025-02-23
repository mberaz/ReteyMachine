using Microsoft.EntityFrameworkCore;
using RetryMachine.SQL.Models;

namespace RetryMachine.SQL.Repositories
{
    public class RetryTaskRepository : EntityBaseRepository<RetryTaskModel>, IRetryTaskRepository
    {
        private readonly RetrymachineContext _context;
        public RetryTaskRepository(RetrymachineContext context)
            : base(context)
        {
            _context = context;
        }

        public Task<List<RetryTask>> FindTasksToRun()
        {
            return _context.Set<RetryTask>()
                .Where(t => t.Status != (int)RetryStatus.Done && t.ActionOn < DateTime.Now)
                .ToListAsync();
        }

        public Task<RetryTask?> GetSingleAsync(long id)
        {
            return _context.Set<RetryTask>().FirstOrDefaultAsync(e => e.Id == id);
        }

        public Task Create(RetryTask taskModel)
        {
            taskModel.CreatedOn = taskModel.UpdatedOn = DateTime.Now;
            _context.Set<RetryTask>().Add(taskModel);
            return _context.SaveChangesAsync();
        }

        public async Task Edit(RetryTask taskModel)
        {
            var task = await GetSingleAsync(taskModel.Id);
            task.RetryCount = taskModel.RetryCount;
            task.Status = taskModel.Status;
            task.UpdatedOn = DateTime.Now;
            task.ActionOn = taskModel.ActionOn;
            task.ActionOrder = taskModel.ActionOrder;
            task.NextActions = taskModel.NextActions;
            task.CompletedActions = taskModel.CompletedActions;
            task.FailedActions = taskModel.FailedActions;
   
            await _context.SaveChangesAsync();
        }
    }
}

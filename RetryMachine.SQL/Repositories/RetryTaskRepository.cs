using Microsoft.EntityFrameworkCore;
using RetryMachine.SQL.Models;

namespace RetryMachine.SQL.Repositories
{
    public class RetryTaskRepository : EntityBaseRepository<RetryTaskModel>, IRetryTaskRepository
    {
        private bool SaveAsBase64 = true;
        public static string? Base64Decode(string? base64EncodedData)
        {
            if (base64EncodedData == null)
            {
                return null;
            }
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string? Base64Encode(string? plainText)
        {
            if (plainText == null)
            {
                return null;
            }
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        private readonly RetrymachineContext _context;
        public RetryTaskRepository(RetrymachineContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<List<RetryTask>> FindTasksToRun()
        {
            var tasks = await _context.Set<RetryTask>().AsNoTracking()
                .Where(t => t.Status != (int)RetryStatus.Done && t.ActionOn < DateTime.Now)
                .ToListAsync();

            if (SaveAsBase64)
            {
                foreach (var task in tasks)
                {
                    task.ActionOrder = Base64Decode(task.ActionOrder);
                    task.NextActions = Base64Decode(task.NextActions);
                    task.CompletedActions = Base64Decode(task.CompletedActions);
                    task.FailedActions = Base64Decode(task.FailedActions);
                }
            }

            return tasks;
        }

        public async Task<RetryTask?> GetSingleAsync(long id)
        {
            var task = await _context.Set<RetryTask>().FirstOrDefaultAsync(e => e.Id == id);

            if (SaveAsBase64)
            {
                task.ActionOrder = Base64Decode(task.ActionOrder);
                task.NextActions = Base64Decode(task.NextActions);
                task.CompletedActions = Base64Decode(task.CompletedActions);
                task.FailedActions = Base64Decode(task.FailedActions);
            }

            return task;
        }

        public Task Create(RetryTask taskModel)
        {
            taskModel.CreatedOn = taskModel.UpdatedOn = DateTime.Now;
            if (SaveAsBase64)
            {
                taskModel.ActionOrder = Base64Encode(taskModel.ActionOrder);
                taskModel.NextActions = Base64Encode(taskModel.NextActions);
                taskModel.CompletedActions = Base64Encode(taskModel.CompletedActions);
                taskModel.FailedActions = Base64Encode(taskModel.FailedActions);
            }

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
            task.ActionOrder = SaveAsBase64 ? Base64Encode(taskModel.ActionOrder) : taskModel.ActionOrder;
            task.NextActions = SaveAsBase64 ? Base64Encode(taskModel.NextActions) : taskModel.NextActions;
            task.CompletedActions = SaveAsBase64 ? Base64Encode(taskModel.CompletedActions) : taskModel.CompletedActions;
            task.FailedActions = SaveAsBase64 ? Base64Encode(taskModel.FailedActions) : taskModel.FailedActions;

            await _context.SaveChangesAsync();
        }
    }
}

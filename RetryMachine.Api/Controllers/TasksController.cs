using Microsoft.AspNetCore.Mvc;
using RetryMachine.Api.Actions;

namespace RetryMachine.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly IRetryMachine _retryMachine;

        public TasksController(ILogger<TasksController> logger, IRetryMachine retryMachine)
        {
            _retryMachine = retryMachine;
        }

        [HttpGet("Set")]
        public async Task Set()
        {
            await _retryMachine.CreateTasks(
            [
                new RetryCreate( AutoLogAction.ActionName, new AutoLogSettings
                {
                    AccountHolderId = 11,
                    Template = ""
                },1),
                new RetryCreate( UserActionLogAction.ActionName, new UserActionLogSettings
                {
                    ActionId = 11
                },2)
            ]);
        }

        [HttpGet("Run")]
        public async Task Run()
        {
            await _retryMachine.PerformTasks();
        }
    }
}

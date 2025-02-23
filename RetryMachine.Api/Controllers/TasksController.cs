using Microsoft.AspNetCore.Mvc;
using RetryMachine.Api.Actions;

namespace RetryMachine.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly IRetryMachineRunner _retryMachine;

        public TasksController(ILogger<TasksController> logger, IRetryMachineRunner retryMachine)
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

        [HttpGet("Immediately")]
        public async Task Immediately()
        {
            await _retryMachine.CreateTasks(
            [
                new RetryCreate( AutoLogAction.ActionName, new AutoLogSettings
                {
                    AccountHolderId = 11,
                    Template = ""
                },1,runImmediately:true),
                new RetryCreate( UserActionLogAction.ActionName, new UserActionLogSettings
                {
                    ActionId = 11
                },2)
            ]);
        }

        [HttpGet("Run")]
        public async Task<int> Run()
        {
            return await _retryMachine.PerformTasks();
        }
    }
}

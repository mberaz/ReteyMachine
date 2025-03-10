using Microsoft.AspNetCore.Mvc;
using RetryMachine.Api.Actions;
using RetryMachine.Api.Service;

namespace RetryMachine.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly IRandomService _service;
        private readonly IRetryMachineRunner _retryMachine;

        public TasksController(IRandomService service, IRetryMachineRunner retryMachine)
        {
            _service = service;
            _retryMachine = retryMachine;
        }

        [HttpGet("Set")]
        public async Task Set()
        {
            await _service.Stuff();
        }

        [HttpGet("Immediately")]
        public async Task Immediately()
        {
            await _service.Fast();
        }

        [HttpGet("Run")]
        public Task<int> Run()
        {
            //call from a scheduler
            return _retryMachine.PerformTasks();
        }
    }
}

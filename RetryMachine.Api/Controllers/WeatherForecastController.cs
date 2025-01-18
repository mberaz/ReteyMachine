using Microsoft.AspNetCore.Mvc;
using RetryMachine.Api.Actions;
using RetryMachine.Api.RetryMachine;

namespace RetryMachine.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private IRetryMachine _retryMachine;

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IRetryMachine retryMachine)
        {
            _logger = logger;
            _retryMachine = retryMachine;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task Get()
        {
            //await _retryMachine.CreateTasks(AutoLogAction.ActionName, new AutoLogSettings
            //{
            //    AccountHolderId = 11,
            //    Template = ""
            //});


            await _retryMachine.CreateTasks(new Dictionary<string, object>
            {
                { 
                    AutoLogAction.ActionName, new AutoLogSettings
                    {
                        AccountHolderId = 11,
                        Template = ""
                    }
                },
                {
                    UserActionLogAction.ActionName, new UserActionLogSettings
                    {
                        ActionId = 11
                    }
                }
            });
        }
    }
}

using RetryMachine.Api.Actions;

namespace RetryMachine.Api.Service
{
    public interface IRandomService
    {
        Task Stuff();
        Task Fast();
    }
    public class RandomService:IRandomService
    {
        private readonly IRetryMachineRunner _retryMachine;

        public RandomService(IRetryMachineRunner retryMachine)
        {
            _retryMachine = retryMachine;
        }

        public async Task Stuff()
        {
            await _retryMachine.CreateTasks(
            [
                new RetryCreate( AutoLogAction.ActionName, new AutoLogSettings { AccountHolderId = 11, Template = "" },order : 1),
                new RetryCreate( UserActionLogAction.ActionName, new UserActionLogSettings { ActionId = 11 },order : 2)
            ]);
        }

        public async Task Fast()
        {
            await _retryMachine.CreateTasks(
            [
                new RetryCreate( AutoLogAction.ActionName, new AutoLogSettings { AccountHolderId = 11, Template = "" },
                    order : 1, runImmediately:true),
                new RetryCreate( UserActionLogAction.ActionName, new UserActionLogSettings { ActionId = 11 },order : 2)
            ]);
        }
    }
}

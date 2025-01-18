namespace RetryMachine.Api
{
    public interface IRetryable
    {
        public string Name();
        public Task<(bool isOk, string error)> Perform(string value);
    }
}

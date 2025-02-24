namespace RetryMachine
{
    public interface IRetryable
    {
        public string Name();
        public Task<(bool isOk, string? error)> Perform(string value);
    }
}

public interface IAgentService
{
    Task<string> SendMessageAsync(string message);
}
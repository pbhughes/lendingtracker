namespace LendingTrackerApi.Services
{
    public interface IMessageSingalr
    {
        Task<String> SendMessage(string user, string message);
    }
}

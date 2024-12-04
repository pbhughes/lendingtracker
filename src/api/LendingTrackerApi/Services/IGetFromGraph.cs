namespace LendingTrackerApi.Services
{
    public interface IGetFromGraph
    {
        Task<string> GetPhoneNumber(string email);
    }
}

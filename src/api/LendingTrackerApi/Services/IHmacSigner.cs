namespace LendingTrackerApi.Services
{
    public interface IHmacSigner
    {
        string Sign(string data);

        bool Verify(string data, string signature);
    }
}

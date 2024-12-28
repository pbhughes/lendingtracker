using Azure.Communication.Email;

namespace LendingTrackerApi.Services
{
    public interface IMessageMailer
    {
        Task<EmailSendOperation> SendEmail(string toAddress, string subject, string message);
    }
}

using Azure.Communication.Email;
using LendingTrackerApi.Models;

namespace LendingTrackerApi.Services
{
    public interface IMessageMailer
    {
        Task<EmailSendOperation> SendEmail( EmailMessage message);
        Task<EmailSendOperation> SendEmail(string toAddress, string subject, string message);

        Task<EmailSendOperation> SendBorrowerAddedNotification(User lender, Borrower borrower);
    }
}

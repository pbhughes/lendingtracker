using Azure.Communication.Sms;

namespace LendingTrackerApi.Services
{
    public interface IMessageSMS
    {
        Task<SmsSendResult> SendSMS(string to, string from, string message);

        string Number { get; set;  } 
    }
}

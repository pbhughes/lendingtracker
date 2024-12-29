using Azure;
using System.Net.Mail;
using System;
using System.Collections.Generic;
using Azure;
using Azure.Communication.Email;
using LendingTrackerApi.Models;
using System.Web;

namespace LendingTrackerApi.Services
{

    public class MessageMailer : IMessageMailer
    {
        IConfiguration _config;
        ILogger<MessageMailer> _logger;
        EmailClient _emailClient;
        string _senderAddress = "DoNotReply@100a20d5-d4df-4a99-96ae-6e2b2a12ddfc.azurecomm.net";
        IHmacSigner _hmacSigner;

        public MessageMailer(IConfiguration config, ILogger<MessageMailer> logger, IHmacSigner signer)
        {
            _config = config;
            _logger = logger;
            _senderAddress = config["MessageMailer:SenderAddress"];
            _emailClient = new EmailClient(config["MessageMailer:ConnectionString"]);
            _hmacSigner = signer;
        }

        public async Task<EmailSendOperation> SendBorrowerAddedNotification(Models.User lender, Borrower borrower)
        {
            string encodeHmacSignatuer = HttpUtility.UrlEncode(_hmacSigner.Sign(borrower.BorrowerEmail.ToString()));
            var emailMessage = new EmailMessage(senderAddress: _senderAddress,
                content: new EmailContent($"{lender.FullName} at {lender.Email} has added you as a borrower")
                {
                    PlainText = $"{{lender.FullName}} at {{lender.Email}} has added you as a borrower",
                    Html = $@"<html>
                    <p>Welcome to our tracker</p>
                    <p>{lender.FullName} has added you to {_config["ViewHost:BaseUrl"]}</p>
                    <a href={_config["ViewHost:BaseUrl"]}/borrower/confirm/{borrower.BorrowerId}?apikey={encodeHmacSignatuer}> click here to confirm addition</a>
                    </html>"
                },
                recipients: new EmailRecipients(new List<EmailAddress> { new EmailAddress(borrower.BorrowerEmail) }));

            EmailSendOperation emailSendOperation = await _emailClient.SendAsync(
                WaitUntil.Started,
                emailMessage);

            return emailSendOperation;

        }

        public async Task<EmailSendOperation> SendEmail(string toAddress, string subject, string message)
        {
            var emailMessage = new EmailMessage(
            senderAddress: _senderAddress,
            content: new EmailContent(subject)
            {
                PlainText = message,
                Html = @$"
		        <html>
			        <body>
				        <p>{message}</p>
			        </body>
		        </html>"
            },
            recipients: new EmailRecipients(new List<Azure.Communication.Email.EmailAddress> { new Azure.Communication.Email.EmailAddress(toAddress) }));


            EmailSendOperation emailSendOperation = await _emailClient.SendAsync(
                WaitUntil.Started,
                emailMessage);

            return emailSendOperation;
        }

       
    }
}







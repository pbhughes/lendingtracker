using Azure;
using Microsoft.Graph.Models;
using System.Net.Mail;
using System;
using System.Collections.Generic;
using Azure;
using Azure.Communication.Email;

namespace LendingTrackerApi.Services
{

    public class MessageMailer : IMessageMailer
    {
        IConfiguration _config;
        ILogger<MessageMailer> _logger;
        EmailClient _emailClient;
        string _senderAddress = "DoNotReply@100a20d5-d4df-4a99-96ae-6e2b2a12ddfc.azurecomm.net";

        public MessageMailer(IConfiguration config, ILogger<MessageMailer> logger)
        {
            _config = config;
            _logger = logger;
            _senderAddress = config["MessageMailer:SenderAddress"];
            _emailClient = new EmailClient(config["MessageMailer:ConnectionString"]);
        }
        public async Task<EmailSendOperation> SendEmail(string toAddress, string subject, string message)
        {
            var emailMessage = new EmailMessage(
            senderAddress: _config["MessageMailer:SenderAddress"],
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







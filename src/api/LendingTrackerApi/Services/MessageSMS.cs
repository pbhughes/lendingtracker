using System;
using System.Collections.Generic;

using Azure;
using Azure.Communication;
using Azure.Communication.Sms;
using LendingTrackerApi.Models;
using Microsoft.Extensions.Options;

namespace LendingTrackerApi.Services
{
    public class MessageSMS : IMessageSMS
    {
        private readonly MessageSMSSettings _settings;
        private readonly SmsClient _smsClient;


        public string Number { get; set; }
        public MessageSMS(IOptions<MessageSMSSettings> settings)
        {
            _settings = settings.Value;
            string smsConnectionString = _settings.SmsKey;
            _smsClient = new SmsClient(smsConnectionString);
            Number = _settings.Number;
        }

        public async Task<SmsSendResult> SendSMS(string to, string from , string message)
        {
            SmsSendResult result = await _smsClient.SendAsync(from: _settings.Number,
                            to: to,
                            message: message);

            return result;
        }
    }
}

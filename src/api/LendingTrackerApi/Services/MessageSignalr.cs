using LendingTrackerApi.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace LendingTrackerApi.Services
{
    public class MessageSignalr : IMessageSingalr
    {
        private IHubContext<MessagesHub> _context;
        public MessageSignalr(IHubContext<MessagesHub> context)
        {
            _context = context;
        }
        public async Task<string> SendMessage(string user, string message)
        {
            await _context.Clients.All.SendAsync("user", message);

            return "sent";
        }
    }
}

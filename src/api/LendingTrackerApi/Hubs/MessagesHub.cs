using Microsoft.AspNetCore.SignalR;

namespace LendingTrackerApi.Hubs
{
    public class MessagesHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            string connectionId = Context.ConnectionId;

            // Log or handle the connection event
            Console.WriteLine($"Client connected: {connectionId}");

            // Optionally, send a message to the connected client
            await Clients.Client(connectionId).SendAsync("OnConnected", $"Welcome! Your Connection ID: {connectionId}");

            await base.OnConnectedAsync();
        }


        // Triggered when a client disconnects from the hub
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            string connectionId = Context.ConnectionId;

            // Log or handle the disconnection event
            Console.WriteLine($"Client disconnected: {connectionId}");

            if (exception != null)
            {
                Console.WriteLine($"Disconnection due to error: {exception.Message}");
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}

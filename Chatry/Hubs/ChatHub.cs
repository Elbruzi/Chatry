using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Chatry.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {

        private readonly IHubContext<ChatHub> _hub;

        public ChatHub(IHubContext<ChatHub> hub)
        {
            _hub = hub; 
        }

        public async Task SendMessage(string message)
        {
            var Username = Context.User?.Identity?.Name;
            if (Username == null)
            {
                Username = "Anonim";
            }

            await Clients.All.SendAsync("ReceiveMessage", Username, message);
        }

    }
}

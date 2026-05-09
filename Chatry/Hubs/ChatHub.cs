using Chatry.Data;
using Chatry.Services.CRUD;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Chatry.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {

        private readonly RoomRepository roomRepository;

        public ChatHub(RoomRepository _roomRepository)
        {
            roomRepository = _roomRepository;
        }

        public async Task AddToRoom(string RequestedRoom)
        {
            string? JwtID = Context.UserIdentifier;

            if (AuthHelper(RequestedRoom, JwtID))
            {
                if (await roomRepository.Exists(RequestedRoom))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, RequestedRoom);
                }
                else
                {
                    await roomRepository.CreateRoom(RequestedRoom);
                    await Groups.AddToGroupAsync(Context.ConnectionId, RequestedRoom);
                }
                await Clients.Group(RequestedRoom).SendAsync("UserJoined", JwtID);
            }
        }

        public async Task SendMessageRoom(string RequestedRoom, string message)
        {
            var Username = Context.User?.Identity?.Name;
            if (Username == null)
            {
                Username = "Anonim";
            }
            await Clients.Group(RequestedRoom).SendAsync("ReceiveMessage", Username, message);
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

        public static bool AuthHelper(string RequestedRoom, string JwtID)
        {
            if (string.IsNullOrWhiteSpace(RequestedRoom) || !RequestedRoom.Contains('-'))
            {
                return false;
            }
            else
            {
                string[] ReqIDs = RequestedRoom.Split('-');
                string ReqID0 = ReqIDs[0];
                string ReqID1 = ReqIDs[1];
                if (ReqID0 == JwtID || ReqID1 == JwtID)
                {
                    int reqID0 = Int32.Parse(ReqID0);
                    int reqID1 = Int32.Parse(ReqID1);
                    if (reqID0 < reqID1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }


    }
}

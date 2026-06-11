using Chatry.Data;
using Chatry.Models;
using Chatry.Services;
using Chatry.Services.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;

namespace Chatry.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {

        private readonly IServiceScopeFactory _serviceScopeFactory;

        private readonly RoomRepository roomRepository;

        public ChatHub(RoomRepository _roomRepository , IServiceScopeFactory serviceScopeFactory)
        {
            roomRepository = _roomRepository;

            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task AddToRoom(string RequestedRoom)
        {
            string? JwtID = Context.UserIdentifier;
            if (JwtID == null)
            {
                return;   
            }

            if (AuthHelper(RequestedRoom, JwtID))
            {
                var (boolen,State) = await roomRepository.Exists(RequestedRoom);
                if (State == Enum_Results.BREAK)
                {
                    return;
                }
                if (boolen)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, RequestedRoom);
                }
                else
                {
                    await roomRepository.CreateRoom(RequestedRoom,JwtID);
                    await Groups.AddToGroupAsync(Context.ConnectionId, RequestedRoom);
                }
                await Clients.Group(RequestedRoom).SendAsync("UserJoined", JwtID);
            }
        }

        public async Task SendMessageRoom(string RequestedRoom, string message)
        {
            string? JwtID = Context.UserIdentifier;
            var (userID , State)= Helpers.ToInt(JwtID);
            if (State == Enum_Results.BREAK)
            {
                return;
            }


            var Username = Context.User?.Identity?.Name;


            if (Username == null)
            {
                Username = "Anonim";
            }
            await Clients.Group(RequestedRoom).SendAsync("ReceiveMessage", Username, message);

            // 3. BYPASS VE ARKA PLAN: SQL kaydını arka plandaki işçiye devrediyoruz (Ucu açık bırakılıyor, await yok)
            _ = Task.Run(async () =>
            {
                try
                {
                    // Arka plan işçisi için izole, güvenli bir DbContext alanı açıyoruz
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        // Senin yazdığın metodun bulunduğu servis/repository hangisiyse onu çağırıyoruz
                        // Örn: IMessageService veya IMessageRepository
                        var messageService = scope.ServiceProvider.GetRequiredService<MessagesRepository>();

                        // Arka plandaki işçi kendi içinde bu metodu await eder, ana programı bağlamaz
                        bool isSaved = await messageService.SaveMessage(RequestedRoom, message, userID);

                        if (!isSaved)
                        {
                            // Buraya log atabilirsin: "Mesaj veritabanına yazılamadı!"
                            Console.WriteLine($"Mesaj SQL'e kaydedilemedi. Oda: {RequestedRoom}, Kullanıcı: {userID}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Arka planda oluşabilecek herhangi bir hata canlı chat akışını kopartmaz
                    Console.WriteLine($"Arka plan işlemi tamamen çöktü: {ex.Message}");
                }
            });
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

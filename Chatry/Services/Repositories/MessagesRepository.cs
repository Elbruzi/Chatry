using Chatry.Data;
using Chatry.DTOs;
using Chatry.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Chatry.Services.Repositories
{
    public class MessagesRepository
    {

        private readonly ChatryDbContext _context;

        private readonly ILogger<MessagesRepository> _logger;


        public MessagesRepository(ChatryDbContext chatryDbContext, HttpContext httpContext, IServiceScopeFactory serviceScopeFactory , ILogger<MessagesRepository> logger)
        {
            _context = chatryDbContext;

            _logger = logger;
        }


        public async Task<IQueryable> Load_MessageQuery(string RoomName , int count)
        {
             return  _context.Messages
             .AsNoTracking()
             .Where(x => x.Room.RoomName == RoomName)
             .OrderByDescending(x => x.TimeStamp)
             .Skip(20 * count)
             .Take(20)
              .Select(x => new
              {
                  x.MessageID,
                  x.RoomID,
                  x.UserID,
                  UserName = x.User.Username,
                  x.Text,
                  x.TimeStamp
              })
             .AsQueryable();

        }


        public async Task<bool> SaveMessage(string RoomName, string text , int userID)
        {
            try
            {
                int roomID = await _context.Rooms.Where(ro => ro.RoomName == RoomName).Select(ro => ro.RoomID).FirstOrDefaultAsync();

                Message message = new Message
                {
                    UserID = userID,
                    RoomID = roomID,
                    Text = text,
                    Active = true,
                    TimeStamp = DateTime.UtcNow,
                };

                _context.Messages.Add(message);

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Quick_Save.Error");
                return false;
            }
            
        }

        
    }
}

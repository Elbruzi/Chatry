using Chatry.Data;
using Chatry.DTOs;
using Chatry.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace Chatry.Services.Repositories
{
    public class RoomRepository
    {

        private readonly ChatryDbContext _context;

        private readonly ILogger<RoomRepository> _logger;

        private readonly HttpContext _httpContext;

        public RoomRepository(ChatryDbContext chatryDbContext, ILogger<RoomRepository> logger , HttpContext httpContext)
        {
            _context = chatryDbContext;

            _logger = logger;

            _httpContext = httpContext;

        }




        public (Enum_Results State , IQueryable<RoomUser_DTO_F> Query) ListFriends()
        {
            var jwtID = _httpContext.GetJwtID();
            if (jwtID.State == Enum_Results.BREAK)
            {
                _logger.LogWarning("HttpContext_Error");
                return (Enum_Results.BREAK, null);
            }

            string JwtID = jwtID.JwtID;
            

            var jwtResult = Helpers.ToInt(JwtID);
            if (jwtResult.State == Enum_Results.BREAK)
            {
                return (Enum_Results.BREAK, null);
            }

            int userID = jwtResult.Int;


            var query = _context.RoomUsers
            .AsNoTracking()
            .Where(x => x.UserID == userID)
            .Where(x => x.Room.FriendStatus == 3)
            .Select(x => new RoomUser_DTO_F
            {
                RoomName = x.Room.RoomName,

                Friends = x.Room.RoomUsers
                    .Where(ru => ru.UserID != userID)
                    .Select(ru => ru.User.Username)
                    .FirstOrDefault()
            });

            return (Enum_Results.Silent, query);
        }

        public async Task<(string String,Enum_Results State)>  FriendAddRemove(string RoomName)
        {

            var jwtID = _httpContext.GetJwtID();
            if (jwtID.State == Enum_Results.BREAK)
            {
                _logger.LogWarning("HttpContext_Error");
                return (null , Enum_Results.BREAK);
            }

            string JwtID = jwtID.JwtID;

            var roomNames = Helpers.RoomDecoder(RoomName);
            if (roomNames.State == Enum_Results.BREAK)
            {
                _logger.LogWarning("Helper_Break");
                return (null, Enum_Results.BREAK);
            }

            var exist = await this.Exists(RoomName);
            if (exist.State == Enum_Results.BREAK)
            {
                _logger.LogWarning("Helper_Break");
                return (null, Enum_Results.BREAK);
            }

            bool Exists = exist.boolen;

            if (!Exists)
            {
                bool response =  await CreateRoom(RoomName, JwtID);
                if (!response)
                {
                    return ("Check_RoomName",Enum_Results.Silent);
                }
            }

            string[] IDs = roomNames.Strings;


            bool IsFirst = JwtID == IDs[0];
            bool IsSecond = JwtID == IDs[1];

            if (!IsFirst && !IsSecond)
            {
                return ("Auth_Error" , Enum_Results.Silent) ;
            }

            int CurrentState = await _context.Rooms.Where(x => x.RoomName == RoomName).Select(x => x.FriendStatus).FirstOrDefaultAsync();

            if (CurrentState > 3 || CurrentState < 0)
            {
                _logger.LogCritical("LOGIC PROBLEM AT FRIENDSTATUS_VALUE FIX ASAP");
                return ("DB_Errorr",Enum_Results.BREAK);
            }

            var StateMap = new Dictionary<(bool, int), (int NewState, string Msg)>
            {
                {(true , 0) , (1 , "Friendship request sent.")},
                {(true , 1) , (0 , "You cancelled the request.")},
                {(true , 2) , (3 , "You are now friends!")},
                {(true , 3) , (0 , "You unfriended this user.")},

                {(false , 0) , (2 ,"Friendship request sent" )},
                {(false , 2) , (0 , "You cancelled the request.")},
                {(false , 1) , (3 , "You are now friends!")},
                {(false , 3) , (0 , "You unfriended this user.")},
            };

            if (StateMap.TryGetValue((IsFirst, CurrentState), out var result))
            {
                try
                {
                    await _context.Rooms.Where(x => x.RoomName == RoomName).ExecuteUpdateAsync(setter => setter.SetProperty(x => x.FriendStatus, result.NewState));
                }
                catch (Exception ex)
                {
                    _logger.LogError("DB_Update failure" , ex);
                    return ("DB_Error", Enum_Results.BREAK);
                }
                return (result.Msg,Enum_Results.Silent);
            }
            _logger.LogCritical("DIDNT TRIGGER ANYTHING IN FRIENDADDREMOVE METHOD");
            return ("Method_Problem",Enum_Results.Silent);
        }


        public async Task<bool> CreateRoom(string RoomName, string JwtID)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var roomNames = Helpers.RoomDecoder(RoomName);
                if (roomNames.State == Enum_Results.BREAK)
                {
                    _logger.LogError("Helper_BREAK RoomDecoder");
                    return false;
                }

                string[] IDs = roomNames.Strings;

                var room = new Rooms
                {
                    RoomName = RoomName,
                    RoomUsers = new List<RoomUser>()
                };

                for (int i = 0; i < 2; i++)
                {
                    room.RoomUsers.Add(new RoomUser
                    {
                        UserID = Helpers.ToInt(IDs[i]).Int
                    });
                }

                await _context.Rooms.AddAsync(room);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError("Room couldn't be created", ex);
                return false;
            }
        }


        public async Task<(bool boolen,Enum_Results State)> Exists(string RoomName)
        {
            try
            {
                bool InDB = await _context.Rooms.AnyAsync(x => x.RoomName == RoomName);

                return(InDB,Enum_Results.Silent);
            }
            catch (Exception ex)
            {
                _logger.LogError("DB_ERROR" ,ex);
                return (false, Enum_Results.BREAK);
            }
        }




    }
}

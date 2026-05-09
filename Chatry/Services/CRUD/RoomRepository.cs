using Chatry.Data;
using Chatry.Migrations;
using Chatry.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Xml;


namespace Chatry.Services.CRUD
{
    public class RoomRepository
    {


        private readonly ChatryDbContext _context;

        private readonly IHttpContextAccessor _httpContextAccessor;


        public RoomRepository(ChatryDbContext chatryDbContext, IHttpContextAccessor iHttpContextAccessor)
        {
            _context = chatryDbContext;

            _httpContextAccessor = iHttpContextAccessor;

        }


        public async Task<string>  FriendAddRemove(string RoomName)
        {

            bool Exists = await this.Exists(RoomName);
            if (!Exists)
            {
                bool response =  await this.CreateRoom(RoomName);
                if (!response)
                {
                    return "Check_RoomName";
                }
            }


            string JwtID = _httpContextAccessor.HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

            string[] IDs = RoomRepository.RoomDecoder(RoomName);
            bool IsFirst = (JwtID == IDs[0]);
            bool IsSecond = (JwtID == IDs[1]);

            if (!IsFirst && !IsSecond)
            {
                return "Auth_Error";
            }

            int CurrentState = await _context.Rooms.Where(x => x.RoomName == RoomName).Select(x => x.FriendStatus).FirstOrDefaultAsync();

            if (CurrentState > 3 || CurrentState < 0)
            {
                return "DB_Errorr";
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
                await _context.Rooms.Where(x => x.RoomName == RoomName).ExecuteUpdateAsync(setter => setter.SetProperty(x => x.FriendStatus,result.NewState));

                return result.Msg;
            }

            return "Method_Problem";

        }



        public static string[] RoomDecoder(string RoomName)
        {
            if (string.IsNullOrWhiteSpace(RoomName) || !RoomName.Contains('-'))
            {
                return null;
            }
            else
            {
                string[] IDs = RoomName.Split('-');
                return IDs;
            }
        }

        public async Task<bool> CreateRoom(string RoomName)
        {
            try
            {
                var rooms = new Rooms
                {
                    RoomName = RoomName
                };
                await _context.Rooms.AddAsync(rooms);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {

                //var RoomName = new Rooms
                //{
                //    RoomName = FirstID + "-" + SecondID
                //};
                //await _context.Rooms.AddAsync(RoomName);
                //await _context.SaveChangesAsync();
                //return true;
                throw;
            }
              


        }


        //public async Task<bool> CreateRoomV2(int FirstID , int SecondID)
        //{
        //    if (FirstID == SecondID && FirstID < 0 && SecondID < 0)
        //    {
        //        return false;
        //    }
        //    if (FirstID < SecondID)
        //    {
        //        var RoomName = new Rooms
        //        {
        //            RoomName = FirstID + "-" + SecondID
        //        };
        //        await _context.Rooms.AddAsync(RoomName);
        //        await _context.SaveChangesAsync();
        //        return true;

        //    }
        //    else
        //    {
        //        var RoomName = new Rooms
        //        {
        //            RoomName = FirstID + "-" + SecondID
        //        };
        //        await _context.Rooms.AddAsync(RoomName);
        //        await _context.SaveChangesAsync();
        //        return true;

        //    }
        //}



        public async Task<bool> Exists(string RoomName)
        {

            if (string.IsNullOrWhiteSpace(RoomName))
            {
                return false;
            }
            try
            {
                bool InDB = await _context.Rooms.AnyAsync(x => x.RoomName == RoomName);

                return InDB;
            }
            catch (Exception)
            {

                throw;
                
            }

        }

    }
}

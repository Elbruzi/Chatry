using Chatry.Data;
using Chatry.Migrations;
using Chatry.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Xml;

namespace Chatry.Services.CRUD
{
    public class RoomRepository
    {

        private readonly ChatryDbContext _context;

        public RoomRepository(ChatryDbContext chatryDbContext)
        {
            _context = chatryDbContext;
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


        public async Task<bool> CreateRoomV2(int FirstID , int SecondID)
        {
            if (FirstID == SecondID && FirstID < 0 && SecondID < 0)
            {
                return false;
            }
            if (FirstID < SecondID)
            {
                var RoomName = new Rooms
                {
                    RoomName = FirstID + "-" + SecondID
                };
                await _context.Rooms.AddAsync(RoomName);
                await _context.SaveChangesAsync();
                return true;

            }
            else
            {
                var RoomName = new Rooms
                {
                    RoomName = FirstID + "-" + SecondID
                };
                await _context.Rooms.AddAsync(RoomName);
                await _context.SaveChangesAsync();
                return true;

            }
        }



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

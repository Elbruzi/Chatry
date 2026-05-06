using Chatry.Data;
using Chatry.DTOs;
using Chatry.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Chatry.Services
{
    public class DTO_Filler
    {

        private readonly ChatryDbContext _context;

        public DTO_Filler(ChatryDbContext context)
        {
            _context = context;
        }
        
        public async Task<List<User_DTO_F>> Get_Users_F() 
        {
            var Users  = await _context.Users.Select(_01_Mapping.User_DTO_F).ToListAsync();

            return (Users);
        }

    }
}

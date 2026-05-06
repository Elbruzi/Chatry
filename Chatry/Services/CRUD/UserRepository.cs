using Chatry.Data;
using Chatry.Models;
using Chatry.Services;
using Microsoft.EntityFrameworkCore;

namespace Chatry.Services.CRUD
{
    public class UserRepository : ICrudRepository<User>
    {

        private readonly ChatryDbContext _context;

        public UserRepository(ChatryDbContext chatryDbContext)
        {
            _context = chatryDbContext;
        }


        public async Task<Enum_Results> Async_ADD(User user)
        {
            if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Password))
            {
                return Enum_Results.Param_Null;
            }
            bool Is_Exists = await _context.Users.AnyAsync(x => x.Username == user.Username);
            if (Is_Exists)
            {
                return Enum_Results.DB_Error;
            }
            try
            {
                user.Password = Hasher.Hash(user.Password);
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return Enum_Results.Successful;
            }
            catch (Exception)
            {
                return Enum_Results.DB_Error;
            }
        }



        public async Task<Enum_Results> User_is_Exists(User Userinfo)
        {
            if (string.IsNullOrWhiteSpace(Userinfo.Username) == true || string.IsNullOrWhiteSpace(Userinfo.Password) == true)
            {
                return Enum_Results.Param_Null;
            }
            try
            {
                if (await _context.Users.AnyAsync(x => x.Username == Userinfo.Username) == true)
                {
                    string HashedPassword = await _context.Users.Where(x => x.Username == Userinfo.Username).Select(x => x.Password).SingleAsync();

                    bool Verifyer = Hasher.Verify(Userinfo.Password, HashedPassword);
                    if (Verifyer == true)
                    {
                        return Enum_Results.Successful;
                    }
                }
                return Enum_Results.Unsuccessful;
            }
            catch (Exception)
            {
                return Enum_Results.DB_Error;
            }
        }



    }

}

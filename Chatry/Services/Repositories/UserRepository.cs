using Chatry.Data;
using Chatry.Models;
using Microsoft.EntityFrameworkCore;

namespace Chatry.Services.Repositories
{
    public class UserRepository
    {

        private readonly ChatryDbContext _context;

        private readonly ILogger<UserRepository> _logger;

        public UserRepository(ChatryDbContext chatryDbContext, ILogger<UserRepository> logger)
        {
            _context = chatryDbContext;

            _logger = logger;
        }

        public async Task<Enum_Results> Async_ADD(User user)
        {
            if (Helpers.IsEmpty(user.Username) == Enum_Results.BREAK || Helpers.IsEmpty(user.Password) == Enum_Results.BREAK)
            {
                _logger.LogError("Helper_BREAK");
                return (Enum_Results.BREAK);
            }

            bool Exists = await _context.Users.AnyAsync(x => x.Username == user.Username);

            if (Exists)
            {
                _logger.LogInformation("This Username already exists");
                return Enum_Results.Unsuccessful;
            }

            try
            {
                user.Password = Hasher.Hash(user.Password);
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return Enum_Results.Successful;
            }
            catch (Exception ex)
            {
                _logger.LogCritical("PASSWORD HASHING PROBLEM", ex);
                return Enum_Results.BREAK;
            }
        }



        public async Task<Enum_Results> User_is_Exists(User Userinfo)
        {
            if (Helpers.IsEmpty(Userinfo.Username) == Enum_Results.BREAK || Helpers.IsEmpty(Userinfo.Password) == Enum_Results.BREAK)
            {
                _logger.LogError("Helper_BREAK");
                return (Enum_Results.BREAK);
            }

            if (await _context.Users.AnyAsync(x => x.Username == Userinfo.Username) == true)
            {
                try
                {
                    string HashedPassword = await _context.Users.Where(x => x.Username == Userinfo.Username).Select(x => x.Password).SingleAsync();

                    bool Verifyer = Hasher.Verify(Userinfo.Password, HashedPassword);
                    if (Verifyer == true)
                    {
                        return Enum_Results.Successful;
                    }
                    else
                    {
                        _logger.LogInformation("Password is wrong");
                        return Enum_Results.Unsuccessful;
                    }
                }
                catch (Exception)
                {
                    _logger.LogCritical("HASHER && DB PROBLEM");
                    return Enum_Results.BREAK;
                }

            }

            _logger.LogInformation("User is not exist");
            return Enum_Results.Unsuccessful;

        }



     public async Task<(Enum_Results enum_Results,int? ID)> User_is_Exists_ReturnsID(User Userinfo)
        {
            if (Helpers.IsEmpty(Userinfo.Username) == Enum_Results.BREAK || Helpers.IsEmpty(Userinfo.Password) == Enum_Results.BREAK)
            {
                _logger.LogError("Helper_BREAK");
                return (Enum_Results.BREAK,null);
            }

            var userdb = await _context.Users.Where(x => x.Username == Userinfo.Username).Select(x => new { x.UserID, x.Password }).FirstOrDefaultAsync();
            if (userdb != null)
            {
                try
                {

                    bool Verifyer = Hasher.Verify(Userinfo.Password, userdb.Password);
                    if (Verifyer == true)
                    {
                        return (Enum_Results.Successful,userdb.UserID);
                    }
                    else
                    {
                        _logger.LogInformation("Password is wrong");
                        return (Enum_Results.Unsuccessful,null);
                    }
                }
                catch (Exception)
                {
                    _logger.LogCritical("HASHER && DB PROBLEM");
                    return (Enum_Results.BREAK, null);
                }

            }

            _logger.LogInformation("User is not exist");
            return (Enum_Results.Unsuccessful,null);

        }

    }



}

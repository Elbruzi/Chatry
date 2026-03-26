using Chatry.Models;
using System.Linq.Expressions;

namespace Chatry.DTOs
{
    public static class _01_Mapping
    {

        public static Expression<Func<User, User_DTO_F>> User_DTO_F => u => new User_DTO_F 
        {
            Username = u.Username
        };

    }
}

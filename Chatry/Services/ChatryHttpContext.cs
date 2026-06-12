using System.Security.Claims;

namespace Chatry.Services
{
    public class ChatryHttpContext
    {

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly ILogger<ChatryHttpContext> _logger;


        public ChatryHttpContext(IHttpContextAccessor httpContextAccessor, ILogger<ChatryHttpContext> logger)
        {
            _httpContextAccessor = httpContextAccessor;

            _logger = logger;
        }

        public (string JwtID , Enum_Results State)GetJwtID()
        {
            string JwtID = _httpContextAccessor.HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            if (JwtID == null)
            {
                _logger.LogError("Couldnt get JwtID || JwtID == null");
                return (null, Enum_Results.BREAK);
            }
            return (JwtID,Enum_Results.Silent);
        }






    }
}

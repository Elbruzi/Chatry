 namespace Chatry.DTOs.Jwt
{
    public class LoginResponseModel
    {
        public string? Username { get; set; }
        public string? AccesToken { get; set; }
        public int? ExpiresIn{ get; set; }
    }
}

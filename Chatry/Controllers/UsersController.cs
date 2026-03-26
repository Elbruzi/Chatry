using Chatry.Data;
using Chatry.Models;
using Chatry.DTOs.Jwt;
using Chatry.Services;
using Chatry.Services.CRUD;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.AspNetCore.Authorization;


namespace Chatry.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
 
        private readonly DTO_Filler _DTO_Filler;
        private readonly ICrudRepository<User> _User_repository;
        private readonly JwtService _JwtService;

        public UsersController(DTO_Filler dTO_Filler, ICrudRepository<User> User_reposityory , JwtService jwtService)
        {
            _DTO_Filler = dTO_Filler;
            _User_repository = User_reposityory;
            _JwtService = jwtService;
        }


        [Authorize]
        [HttpGet("User_List")]
        public async Task<ActionResult> User_List()
        {
            var users = await _DTO_Filler.Get_Users_F();

            if (users == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(users);
            }
        }



        [HttpPost("User_ADD")]
        public async Task<ActionResult> Async_ADD(User user)
        {
            var response = await _User_repository.Async_ADD(user);
            switch (response)
            {
                case Enum_Results.Successful:
                    var LoginRequest = new LoginRequestModel
                    {
                        Username = user.Username,
                        Password = user.Password
                    };

                    var tokenResponse =  _JwtService.Authenticate(LoginRequest);

                    return Ok(tokenResponse);

                case Enum_Results.Param_Null:
                    return StatusCode(StatusCodes.Status406NotAcceptable);

                case Enum_Results.DB_Error:
                    return StatusCode(StatusCodes.Status500InternalServerError);

                default: return NotFound();
            }
        }




        [HttpPost("User_Login")]
        public async Task<ActionResult> User_login(User user)
        {
            var response = await _User_repository.User_is_Exists(user);
            switch (response)
            {
                case Enum_Results.Successful:
                    var LoginRequest = new LoginRequestModel
                    {
                        Username = user.Username ,
                        Password = user.Password
                    };

                    var tokenResponse =  _JwtService.Authenticate(LoginRequest);

                    return Ok(tokenResponse);

                case Enum_Results.Param_Null:
                    return StatusCode(StatusCodes.Status406NotAcceptable);

                case Enum_Results.DB_Error:
                    return StatusCode(StatusCodes.Status500InternalServerError);

                default: return NotFound();
            }
        }












    }
}

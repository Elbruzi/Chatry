using Chatry.Services;
using Chatry.Services.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Chatry.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly MessagesRepository _messagesRepository;

        public MessagesController(MessagesRepository messagesRepository)
        {
            _messagesRepository = messagesRepository;
        }


        [Authorize]
        [HttpGet("Load_Msg")]
        public async Task<ActionResult> Load_Messages(string RoomName , int Count)
        {
            if (Helpers.IsEmpty(RoomName) == Enum_Results.BREAK || Count < 0 )
            {
                return BadRequest();
            }
            try
            {
                var messages = await _messagesRepository.Load_MessageQuery(RoomName, Count);
                return Ok(messages);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }








    }
}

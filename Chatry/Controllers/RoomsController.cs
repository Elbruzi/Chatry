using Chatry.Data;
using Chatry.DTOs;
using Chatry.Models;
using Chatry.Services;
using Chatry.Services.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Mono.TextTemplating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chatry.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {

        private readonly RoomRepository _RoomRepository;

        public RoomsController(RoomRepository roomRepository)
        {

            _RoomRepository = roomRepository;
        }

        [Authorize]
        [HttpPut("FriendAddRemove")]
        public async Task<ActionResult> FriendAddRemove(string RoomName)
        {
            if (Helpers.IsEmpty(RoomName) == Enum_Results.BREAK)
            {
                return BadRequest();
            }

            var (msg, State) = await _RoomRepository.FriendAddRemove(RoomName);
            if (Enum_Results.BREAK == State)
            {
                    return StatusCode(StatusCodes.Status406NotAcceptable);
            }

            return Ok (msg);

        }

        [Authorize]
        [HttpGet("FriendList")]
        public async Task<ActionResult> FriendList()
        {
            var (State , query ) = _RoomRepository.ListFriends();
            if (Enum_Results.BREAK == State)
            {
                return StatusCode(StatusCodes.Status406NotAcceptable);
            }

            List<RoomUser_DTO_F> list = await query.ToListAsync();
            return Ok(list);
        }

    }
}

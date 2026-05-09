using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Chatry.Data;
using Chatry.Models;
using Microsoft.AspNetCore.Authorization;
using Chatry.Services.CRUD;

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
        [HttpPut]
        public async Task<ActionResult> FriendAddRemove(string IDs)
        {
            return Ok(_RoomRepository.FriendAddRemove(IDs));
        }

    }
}

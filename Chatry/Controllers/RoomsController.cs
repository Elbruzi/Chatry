using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Chatry.Data;
using Chatry.Models;

namespace Chatry.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly ChatryDbContext _context;

        public RoomsController(ChatryDbContext context)
        {
            _context = context;
        }











    }
}

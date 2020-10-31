using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KanbanMaster.Server.Data;
using KanbanMaster.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KanbanMaster.Server.Controllers
{
    [Route("api/projectItem")]
    [Authorize]
    [ApiController]
    public class ProjectItemController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ProjectItemController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ProjectItem> ListProjectItems()
        {
            
        }
    }
}

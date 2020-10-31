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

        [HttpPost]
        public async Task CreateProjectItem(ProjectItem project)
        {
            project.NewTime = DateTime.Now;
            project.Description = "Enter a descript for this project.";
            await _context.AddAsync(project);
            await _context.SaveChangesAsync();
        }

        [HttpPost("delete")]
        public async Task DeleteProjectItem(ProjectItem projectItem)
        {
            _context.Remove(projectItem);
            await _context.SaveChangesAsync();
        }

        [HttpGet("list")]
        public async Task<List<ProjectItem>> ListProjectItems()
        {
            return await _context.ProjectItems.ToListAsync();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using KanbanMaster.Server.Data;
using KanbanMaster.Shared.Models;
using Microsoft.AspNetCore.Authorization;
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

        #region GET

        [HttpGet("list")]
        public async Task<List<ProjectItem>> ListProjectItems()
        {
            string user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await _context.ProjectItems.Where(x => x.UserId == user).ToListAsync();
        }

        #endregion

        #region POST

        [HttpPost]
        public async Task CreateProjectItem(ProjectItem project)
        {
            project = SetInitalProjectItemFields(project);

            await _context.AddAsync(project);
            await _context.SaveChangesAsync();
        }

        [HttpPost("delete")]
        public async Task DeleteProjectItem(ProjectItem projectItem)
        {
            _context.Remove(projectItem);
            await _context.SaveChangesAsync();
        }

        #endregion

        #region

        [HttpPut]
        public async Task UpdateProjectItem(ProjectItem project)
        {
            _context.Entry(project).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        [HttpPut("setActive")]
        public async Task SetActiveProject(ProjectItem project)
        {
            project.IsActive = true;

            string user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            List<ProjectItem> projects = _context.ProjectItems.Where(x => x.UserId == user).ToList();

            projects.RemoveAll(x => x.ProjectItemId != project.ProjectItemId);
            projects.ForEach(x => x.IsActive = false);

            _context.UpdateRange(projects);
            await _context.SaveChangesAsync();

            await UpdateProjectItem(project);
        }

        #endregion

        #region

        [NonAction]
        public ProjectItem SetInitalProjectItemFields(ProjectItem project)
        {
            string user = User.FindFirstValue(ClaimTypes.NameIdentifier);

            project.UserId = user;
            project.NewTime = DateTime.Now;
            project.Description = "Enter a descript for this project.";
            if (string.IsNullOrWhiteSpace(project.Name)) project.Name = "New Project";

            return project;
        }

        #endregion
    }
}

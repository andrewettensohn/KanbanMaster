﻿using System;
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

            List<ProjectItem> projectItems = await _context.ProjectItems.Where(x => x.UserId == user).ToListAsync();
            projectItems = GetProjectTodoItems(projectItems);  

            return projectItems;
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

            string user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            List<ProjectItem> projects = _context.ProjectItems.Where(x => x.UserId == user).ToList();

            if(projects.Any(x => x.IsActive))
            {
                ProjectItem oldActiveProject = projects.FirstOrDefault(x => x.IsActive);
                oldActiveProject.IsActive = false;
                await UpdateProjectItem(oldActiveProject);
            }

            ProjectItem newActiveProject = await _context.ProjectItems.FirstOrDefaultAsync(x => x.ProjectItemId == project.ProjectItemId);
            newActiveProject.IsActive = true;
            await UpdateProjectItem(newActiveProject);
        }

        #endregion

        #region

        [NonAction]
        private ProjectItem SetInitalProjectItemFields(ProjectItem project)
        {
            string user = User.FindFirstValue(ClaimTypes.NameIdentifier);

            project.UserId = user;
            project.NewTime = DateTime.Now;
            project.Description = "Enter a description for this project.";
            if (string.IsNullOrWhiteSpace(project.Name)) project.Name = "New Project";

            return project;
        }

        [NonAction]
        private List<ProjectItem> GetProjectTodoItems(List<ProjectItem> projectItems)
        {
            foreach (ProjectItem project in projectItems)
            {
                if(project.TotalTasks != 0)
                {
                    project.TodoItems = _context.TodoItems.Where(x => x.ProjectItemId == project.ProjectItemId).ToList();

                    if(project.TodoItems.Any(x => x.Status == "New"))
                    {
                        project.PercentageNew = Math.Round((decimal)(100 * project.TodoItems.Where(x => x.Status == "New").Count()) / project.TotalTasks);
                    }
                    if(project.TodoItems.Any(x => x.Status == "Doing"))
                    {
                        project.PercentageDoing = Math.Round((decimal)(100 * project.TodoItems.Where(x => x.Status == "Doing").Count()) / project.TotalTasks);
                    }
                    if(project.TodoItems.Any(x => x.Status == "Done"))
                    {
                        project.PercentageDone = Math.Round((decimal)(100 * project.TodoItems.Where(x => x.Status == "Done").Count()) / project.TotalTasks);
                    }
                }
            }

            return projectItems;
        }

        #endregion
    }
}

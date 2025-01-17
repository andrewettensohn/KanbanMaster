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

            List<ProjectItem> projectItems = await _context.ProjectItems.Where(x => x.UserId == user && !x.IsArchived).ToListAsync();
            projectItems = GetProjectTodoItems(projectItems);  

            return projectItems;
        }

        [HttpGet("listArchived")]
        public async Task<List<ProjectItem>> ListArchivedProjectItems()
        {
            string user = User.FindFirstValue(ClaimTypes.NameIdentifier);

            List<ProjectItem> projectItems = await _context.ProjectItems.Where(x => x.UserId == user && x.IsArchived).ToListAsync();
            projectItems = GetProjectTodoItems(projectItems);

            return projectItems;
        }

        [HttpGet("listHistory")]
        public async Task<List<HistoryItem>> ListActiveProjectHistory()
        {
            string user = User.FindFirstValue(ClaimTypes.NameIdentifier);

            List<ProjectItem> projectItems = new List<ProjectItem> { await _context.ProjectItems.FirstOrDefaultAsync(x => x.UserId == user && x.IsActive) };
            projectItems = GetProjectTodoItems(projectItems);
            ProjectItem projectItem = projectItems.FirstOrDefault();

            List<HistoryItem> historyItems = new List<HistoryItem>();

            historyItems.AddRange(GetProjectHistoryItems(projectItem));
            historyItems.AddRange(GetTodoItemHistoryItems(projectItem.TodoItems));

            historyItems = historyItems.OrderBy(x => x.Time).ToList();
            
            return historyItems;
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

        #region PUT

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

            if(_context.ProjectItems.Any(x => x.IsActive))
            {
                await SetActiveProjectToInactive();
            }

            ProjectItem newActiveProject = await _context.ProjectItems.FirstOrDefaultAsync(x => x.ProjectItemId == project.ProjectItemId);
            newActiveProject.IsActive = true;
            await UpdateProjectItem(newActiveProject);
        }

        [HttpPut("archive")]
        public async Task ArchiveProject(ProjectItem project)
        {
            await SetActiveProjectToInactive();

            project.DoneTime = DateTime.Now;
            project.IsArchived = true;

            await UpdateProjectItem(project);
        }

        #endregion

        #region Non-Action

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
        private async Task SetActiveProjectToInactive()
        {
            if (_context.ProjectItems.Any(x => x.IsActive))
            {
                ProjectItem activeProject = _context.ProjectItems.FirstOrDefault(x => x.IsActive);
                activeProject.IsActive = false;
                await UpdateProjectItem(activeProject);
            }
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

        [NonAction]
        private static List<HistoryItem> GetProjectHistoryItems(ProjectItem projectItem)
        {
            List<HistoryItem> historyItems = new List<HistoryItem>
            {
                new HistoryItem { Title = "Project Created", Time = projectItem.NewTime, Description = $"{projectItem.Name} created." }
            };

            if(projectItem.DoingTime != null)
            {
                historyItems.Add(new HistoryItem { Title = "Project Started", Time = projectItem.DoingTime, Description = $"{projectItem.Name} first task moved to doing." });
            }

            if(projectItem.DoneTime != null)
            {
                historyItems.Add(new HistoryItem { Title = "Project Completed and Archived", Time = projectItem.DoneTime, Description = $"{projectItem.Name} moved to archived." });
            }

            return historyItems;
        }


        [NonAction]
        private static List<HistoryItem> GetTodoItemHistoryItems(List<TodoItem> todoItems)
        {
            List<HistoryItem> historyItems = new List<HistoryItem>();

            foreach (TodoItem todoItem in todoItems)
            {
                historyItems.Add(new HistoryItem { Title = "Task Created", Time = todoItem.TaskNewTime, Description = $"{todoItem.Title} created." });

                if(todoItem.TaskDoingTime != null)
                {
                    historyItems.Add(new HistoryItem { Title = "Task Started", Time = todoItem.TaskDoingTime, Description = $"{todoItem.Title} started." });
                }

                if(todoItem.TaskDoneTime != null)
                {
                    historyItems.Add(new HistoryItem { Title = "Task Completed", Time = todoItem.TaskDoneTime, Description = $"{todoItem.Title} completed." });
                }
            }

            return historyItems;
        }


        #endregion
    }
}

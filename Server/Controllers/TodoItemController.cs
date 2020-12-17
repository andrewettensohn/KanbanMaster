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
    [Route("api/todoItem")]
    [Authorize]
    [ApiController]
    public class TodoItemController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TodoItemController(ApplicationDbContext context)
        {
            _context = context;
        }

        #region GET

        [HttpGet]
        public async Task<TodoItem> GetTodoItem(int todoItem)
        {
            return await _context.TodoItems.FindAsync(todoItem);
        }

        [HttpGet("list")]
        public async Task<List<TodoItem>> ListTodoItems()
        {
            string user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ProjectItem activeProject = GetActiveProject(user);
            return await GetProjectTodoItems(activeProject);
        }

        #endregion

        #region POST

        [HttpPost]
        public async Task CreateTodoItem(TodoItem todoItem)
        {
            todoItem = await SetInitalTodoItemFields(todoItem);

            await _context.AddAsync(todoItem);
            await _context.SaveChangesAsync();
        }

        [HttpPost("delete")]
        public async Task DeleteTodoItem(TodoItem todoItem)
        {
            _context.Remove(todoItem);
            await _context.SaveChangesAsync();
        }

        #endregion

        #region PUT

        [HttpPut]
        public async Task UpdateTodoItem(TodoItem todoItem)
        {
            _context.Entry(todoItem).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        #endregion

        #region NonAction

        [NonAction]
        private async Task<TodoItem> SetInitalTodoItemFields(TodoItem todoItem)
        {
            string user = User.FindFirstValue(ClaimTypes.NameIdentifier);

            todoItem.UserId = user;
            todoItem.Description = "Click here to add a descripton!";
            todoItem.Status = "New";
            await SetProjectItemFields(todoItem);

            return todoItem;
        }

        [NonAction]
        private async Task<TodoItem> SetProjectItemFields(TodoItem todoItem)
        {
            ProjectItem activeProject = GetActiveProject(todoItem.UserId);

            if(activeProject != null)
            {
                todoItem.ProjectItemId = activeProject.ProjectItemId;
                activeProject.TotalTasks += 1;

                _context.Entry(activeProject).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }

            return todoItem;
        }

        [NonAction]
        private ProjectItem GetActiveProject(string user) => _context.ProjectItems.FirstOrDefault(x => x.IsActive && x.UserId == user);

        [NonAction]
        private async Task<List<TodoItem>> GetProjectTodoItems(ProjectItem project) => await _context.TodoItems.Where(x => x.ProjectItemId == project.ProjectItemId).ToListAsync();

        #endregion
    }
}

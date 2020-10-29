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

        [HttpPost]
        public async Task CreateTodoItem(TodoItem todoItem)
        {
            await _context.AddAsync(todoItem);
            await _context.SaveChangesAsync();
        }

        [HttpGet]
        public async Task<TodoItem> GetTodoItem(int todoItem)
        {
            return await _context.TodoItems.FindAsync(todoItem);
        }

        [HttpGet("list")]
        public async Task<List<TodoItem>> ListTodoItems()
        {
            return await _context.TodoItems.ToListAsync();
        }

        [HttpPut]
        public async Task UpdateTodoItem(TodoItem todoItem)
        {
            _context.Entry(todoItem).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        [HttpPost("delete")]
        public async Task DeleteTodoItem(TodoItem todoItem)
        {
            _context.Remove(todoItem);
            await _context.SaveChangesAsync();
        }
    }
}

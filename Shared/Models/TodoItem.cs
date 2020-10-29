using System;
using System.Collections.Generic;
using System.Text;

namespace KanbanMaster.Shared.Models
{
    public class TodoItem
    {
        public int ToDoItemId { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
    }
}

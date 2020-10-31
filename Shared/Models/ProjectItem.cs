using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace KanbanMaster.Shared.Models
{
    public class ProjectItem
    {
        public int ProjectItemId { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime? NewTime { get; set; }
        public DateTime? DoingTime { get; set; }
        public DateTime? DoneTime { get; set; }
        public int TotalTasks { get; set; }

        [NotMapped]
        public List<TodoItem> TodoItems { get; set; }
    }
}

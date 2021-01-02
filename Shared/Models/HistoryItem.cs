using System;
using System.Collections.Generic;
using System.Text;

namespace KanbanMaster.Shared.Models
{
    public class HistoryItem
    {
        public string Title { get; set; }

        public DateTime? Time { get; set; }

        public string Description { get; set; }
    }
}

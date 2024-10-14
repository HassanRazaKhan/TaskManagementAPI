using System;
using System.Collections.Generic;

namespace TaskManagementAPI.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; }
        public List<int> Dependencies  { get; set; } = new List<int>();
    }
}

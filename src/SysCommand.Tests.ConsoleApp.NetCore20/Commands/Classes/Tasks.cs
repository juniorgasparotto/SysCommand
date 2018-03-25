using System;
using System.Collections.Generic;

namespace SysCommand.Tests.ConsoleApp
{
    public class Tasks
    {
        public List<Task> AllTasks = new List<Task>();
    }

    public class Task
    {
        public int Id { get; set; }
        public DateTime DateAndTime { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}

using System;
using System.Collections.Generic;
using SysCommand.Mapping;

namespace SysCommand.Tests.ConsoleApp.Commands.Classes
{
    public class Tasks
    {
        public List<Task> AllTasks = new List<Task>();
    }

    public class Task
    {
        public int Id { get; set; }

        [Argument(Help = "Date help")]
        public DateTime DateAndTime { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}

using System.Collections.Generic;
using System;
using SysCommand.ConsoleApp;
using SysCommand.Mapping;

namespace SysCommand.Tests.ConsoleApp.Commands
{
    [ObjectFile(FileName = "all.json", Folder = @".app\tasks\")]
    public class Tasks
    {
        public List<Task> AllTasks = new List<Task>();
    }

    public class Task
    {
        public int Id { get; set; }

        [ArgumentAttribute(Help = "Date help")]
        public DateTime DateAndTime { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}

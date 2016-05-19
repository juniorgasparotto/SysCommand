using System.Collections.Generic;
using System.IO;
using System;

namespace SysCommand.Tests
{
    [ObjectFile(FileName = "all.json", Folder = @"Tasks")]
    public class Tasks
    {
        public List<Task> AllTasks = new List<Task>();
    }

    public class Task
    {
        [CommandPropertyAttribute(Help = "Date help")]
        public DateTime DateAndTime { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}

namespace Example.FileManager
{
    using SysCommand.ConsoleApp;
    using SysCommand.ConsoleApp.Files;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Program
    {
        public static int Main(string[] args)
        {
            return App.RunApplication();
        }
    }

    public class Command1 : Command
    {
        private JsonFileManager fileManager;

        public Command1()
        {
            fileManager = App.Items.GetOrCreate<JsonFileManager>();
        }

        public void Save(string title, string description = null)
        {
            var tasks = fileManager.GetOrCreate<Tasks>();
            tasks.LastUpdate = DateTime.Now;

            var task = tasks.AllTasks.FirstOrDefault(t => t.Title == title);
            if (task == null)
            {
                task = new Task
                {
                    Id = tasks.AllTasks.Count + 1,
                    Title = title,
                    Description = description,
                    DateAndTime = DateTime.Now
                };
                tasks.AllTasks.Add(task);
            }

            fileManager.Save(tasks);
        }

        public void Get(string title)
        {
            var tasks = fileManager.GetOrCreate<Tasks>();
            this.ShowTask(tasks.AllTasks.Where(t => t.Title.Contains(title)));
        }

        private void ShowTask(IEnumerable<Task> tasks)
        {
            foreach (var task in tasks)
                this.ShowTask(task);
        }

        private void ShowTask(Task task)
        {
            if (task == null)
            {
                App.Console.Error("Task not found");
                return;
            }

            App.Console.Write("Id: " + task.Id);
            App.Console.Write("Title: " + task.Title ?? "-");
            App.Console.Write("Description: " + task.Description ?? "-");
            App.Console.Write("Date: " + task.DateAndTime);
        }

        [ObjectFile(FileName = "tasks")]
        public class Tasks
        {
            public DateTime LastUpdate { get; set; }
            public List<Task> AllTasks { get; set; } = new List<Task>();
        }

        public class Task
        {
            public int Id { get; set; }
            public DateTime DateAndTime { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
        }
    }
}
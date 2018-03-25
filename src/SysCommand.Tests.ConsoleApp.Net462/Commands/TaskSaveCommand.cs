using System;
using System.Collections.Generic;
using SysCommand.ConsoleApp;
using SysCommand.ConsoleApp.Files;
using SysCommand.Mapping;
using static SysCommand.Helpers.ReflectionHelper;

namespace SysCommand.Tests.ConsoleApp
{
    public class TaskSaveCommand : Command
    {
        public bool CleanEmptys { get; set; }

        public TaskSaveCommand()
        {
            this.HelpText = "Methods to SAVE tasks";
        }

        public void Main()
        {
            if (CleanEmptys)
            {
                new App().Run("delete-emptys");
            }
        }

        public void Save()
        {
            App.Console.Write(this.GetDebugName(this.GetActionMap(T())));
            var fileManager = App.Items.GetOrCreate<JsonFileManager>();
            var tasks = fileManager.GetOrCreate<List<Task>>();
            var task = new Task
            {
                Id = tasks.Count + 1,
                Title = null,
                Description = null,
                DateAndTime = DateTime.Now
            };

            tasks.Add(task);
            fileManager.Save(tasks);
        }

        public void Save(string title)
        {
            App.Console.Write(this.GetDebugName(this.GetActionMap(T<string>())));
            var fileManager = App.Items.GetOrCreate<JsonFileManager>();
            var tasks = fileManager.GetOrCreate<List<Task>>();
            var task = new Task
            {
                Id = tasks.Count + 1,
                Title = title,
                Description = null,
                DateAndTime = DateTime.Now
            };

            tasks.Add(task);
            fileManager.Save(tasks);
        }

        public void Save(string title, string description = null, DateTime? date = null)
        {
            App.Console.Write(this.GetDebugName(this.GetActionMap(T<string, string, DateTime?>())));
            var fileManager = App.Items.GetOrCreate<JsonFileManager>();
            var tasks = fileManager.GetOrCreate<List<Task>>();
            var task = new Task
            {
                Id = tasks.Count + 1,
                Title = title,
                Description = description,
                DateAndTime = date ?? DateTime.Now
            };

            tasks.Add(task);
            fileManager.Save(tasks);
        }

        private string GetDebugName(ActionMap map)
        {
            var specification = Program.GetMethodSpecification(map);
            return this.GetType().Name + "." + specification;
        }
    }
}

using System;
using System.Collections.Generic;
using SysCommand.ConsoleApp;
using SysCommand.ConsoleApp.Files;
using SysCommand.Mapping;
using SysCommand.Tests.ConsoleApp.Commands.Classes;

namespace SysCommand.Tests.ConsoleApp.Commands
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
            App.Console.Write(this.GetDebugName(this.CurrentActionMap()));
            var fileManager = App.Items.GetOrCreate<JsonFileManager>();
            var tasks = fileManager.Get<List<Task>>();
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
            App.Console.Write(this.GetDebugName(this.CurrentActionMap()));
            var fileManager = App.Items.GetOrCreate<JsonFileManager>();
            var tasks = fileManager.Get<List<Task>>();
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
            App.Console.Write(this.GetDebugName(this.CurrentActionMap()));
            var fileManager = App.Items.GetOrCreate<JsonFileManager>();
            var tasks = fileManager.Get<List<Task>>();
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

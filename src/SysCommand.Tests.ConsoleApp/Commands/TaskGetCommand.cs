using System;
using System.Collections.Generic;
using System.Linq;
using SysCommand.ConsoleApp;
using SysCommand.ConsoleApp.Files;
using SysCommand.Mapping;
using SysCommand.Tests.ConsoleApp.Commands.Classes;
using static SysCommand.Helpers.ReflectionHelper;

namespace SysCommand.Tests.ConsoleApp.Commands
{
    public class TaskGetCommand : Command
    {
        public TaskGetCommand()
        {
            this.HelpText = "Methods to GET tasks";
        }

        public void Get(string title = null, string description = null, DateTime? date = null)
        {
            App.Console.Write(this.GetDebugName(this.GetActionMap(T<string, string, DateTime?>())));
            var fileManager = App.Items.GetOrCreate<JsonFileManager>();
            var tasks = fileManager.GetOrCreate<List<Task>>();
            this.ShowTask(tasks.Where(t => t.Title == title || t.Description == description || t.DateAndTime == date));
            
        }

        public void Get(int? id = null)
        {
            App.Console.Write(this.GetDebugName(this.GetActionMap(T<int?>())));
            var fileManager = App.Items.GetOrCreate<JsonFileManager>();
            var tasks = fileManager.GetOrCreate<List<Task>>();
            this.ShowTask(tasks.Where(t => t.Id == id));
        }

        public void Get(string title = null)
        {
            App.Console.Write(this.GetDebugName(this.GetActionMap(T<string>())));
            var fileManager = App.Items.GetOrCreate<JsonFileManager>();
            var tasks = fileManager.GetOrCreate<List<Task>>();
            this.ShowTask(tasks.Where(t => t.Title == title));
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

        private string GetDebugName(ActionMap map)
        {
            var specification = Program.GetMethodSpecification(map);
            return this.GetType().Name + "." + specification;
        }
    }
}

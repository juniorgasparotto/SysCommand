using System;
using System.Collections.Generic;
using SysCommand.ConsoleApp;
using SysCommand.ConsoleApp.Files;
using SysCommand.Execution;
using SysCommand.Mapping;
using SysCommand.Tests.ConsoleApp.Commands.Classes;

namespace SysCommand.Tests.ConsoleApp.Commands
{
    public class TaskDeleteCommand : Command
    {
        public TaskDeleteCommand()
        {
            this.HelpText = "Methods to DELETE tasks";
        }

        public void Delete(int id)
        {
            App.Console.Write(this.GetDebugName(this.CurrentActionMap()));
            var fileManager = App.Items.GetOrCreate<JsonFileManager>();
            var tasks = fileManager.GetOrCreate<List<Task>>();
            tasks.RemoveAll(t => t.Id == id);
            fileManager.Save(tasks);
        }

        [Action(Name = "delete-by-title")]
        public void Delete(
            [Argument(LongName="id", ShortName='i', Help="My ID", DefaultValue="not work here", ShowHelpComplement=true)]
            string title
        )
        {
            App.Console.Write(this.GetDebugName(this.CurrentActionMap()));
            var fileManager = App.Items.GetOrCreate<JsonFileManager>();
            var tasks = fileManager.GetOrCreate<List<Task>>();
            tasks.RemoveAll(t => t.Title == title);
            fileManager.Save(tasks);
        }

        public void DeleteEmptys()
        {
            App.Console.Write(this.GetDebugName(this.CurrentActionMap()));
            var fileManager = App.Items.GetOrCreate<JsonFileManager>();
            var tasks = fileManager.GetOrCreate<List<Task>>();
            tasks.RemoveAll(t => t.Title == null || t.Description == null);
            fileManager.Save(tasks);
        }

        private string GetDebugName(ActionMap map)
        {
            var specification = Program.GetMethodSpecification(map);
            return this.GetType().Name + "." + specification;
        }
    }
}

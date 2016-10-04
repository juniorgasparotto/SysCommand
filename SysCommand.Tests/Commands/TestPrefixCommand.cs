using Fclp;
using System;
using System.Linq.Expressions;
using System.Linq;
using System.Collections.Generic;

namespace SysCommand.Tests
{
    public class TestPrefixCommand : CommandAction
    {
        public TestPrefixCommand()
        {
            this.AllowSaveArgsInStorage = true;
            this.AddPrefixInActions = true;
            this.PrefixActions = "custom-prefix";
        }

        public void Get(string title = null, string description = null, DateTime? date = null)
        {
            App333.Current.Response.WriteLine("Executing: Task2Command/title/description/date");
        }

        public void Get(int? id = null)
        {
            App333.Current.Response.WriteLine("Executing: Task2Command/Get/id");
            var tasks = App333.Current.GetOrCreateObjectFile<List<Task>>();
            App333.Current.Response.Write(tasks.FirstOrDefault(f=>f.Id == id));
        }

        public void Get(string title = null)
        {
            App333.Current.Response.WriteLine("Executing: Task2Command/Get/title");
            var tasks = App333.Current.GetOrCreateObjectFile<List<Task>>();
            App333.Current.Response.Write(tasks.FirstOrDefault(f => f.Title == title));
        }

        public void Save()
        {
            App333.Current.Response.WriteLine("Executing: Task2Command/Save");
        }

        public void Save(string title)
        {
            App333.Current.Response.WriteLine("Executing: Task2Command/Save/title");
            var tasks = App333.Current.GetOrCreateObjectFile<List<Task>>();
            var task = new Task
            {
                Id = tasks.Count + 1,
                Title = title,
                DateAndTime = DateTime.Now
            };
            tasks.Add(task);
            App333.Current.SaveObjectFile(tasks);
        }

        public void Save(string title, string description = null, DateTime? date = null)
        {
            App333.Current.Response.WriteLine("Executing: Task2Command/Save/title/description/date");
            var tasks = App333.Current.GetOrCreateObjectFile<List<Task>>();
            var task = new Task
            {
                Id = tasks.Count + 1,
                Title = title,
                Description = description,
                DateAndTime = date ?? DateTime.Now
            };
            tasks.Add(task);
            App333.Current.SaveObjectFile(tasks);
        }

        public void Delete(int id)
        {
            App333.Current.Response.WriteLine("Executing: Task2Command/Delete/id(int)");
            var tasks = App333.Current.GetOrCreateObjectFile<List<Task>>();
            tasks.RemoveAll(t => t.Id == id);
            App333.Current.SaveObjectFile(tasks);
        }

        [Action(Name = "ignored-prefix", UsePrefix = false)]
        public void Delete(
            [Argument(LongName="id", ShortName='i', Help="My ID", DefaultValue="not work here", ShowHelpComplement=false)]
            string id
        )
        {
            App333.Current.Response.WriteLine("Executing: Task2Command/Delete/id(string)");
            var tasks = App333.Current.GetOrCreateObjectFile<List<Task>>();
            tasks.RemoveAll(t => t.Title == id);
            App333.Current.SaveObjectFile(tasks);
        }

        //[Action(Ignore = true)]
        public void NotAction(string id)
        {
            App333.Current.Response.WriteLine("Executing: Task2Command/Ignored");
        }

    }
}

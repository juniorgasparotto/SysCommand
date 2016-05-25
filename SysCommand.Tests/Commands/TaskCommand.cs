using Fclp;
using System;
using System.Linq.Expressions;
using System.Linq;
using System.Collections.Generic;

namespace SysCommand.Tests
{
    public class TaskCommand : CommandAction
    {
        public TaskCommand()
        {
            this.AllowSaveArgsInStorage = true;
        }

        public void Get(int id)
        {
            App.Current.Response.WriteLine("Executing: TaskCommand/Get");
            var tasks = App.Current.GetOrCreateObjectFile<List<Task>>();
            App.Current.Response.Write(tasks.FirstOrDefault(f=>f.Id == id));
        }

        public void Save()
        {
            App.Current.Response.WriteLine("Executing: TaskCommand/Save");
        }

        public void Save(string title)
        {
            App.Current.Response.WriteLine("Executing: TaskCommand/Save/title");
            var tasks = App.Current.GetOrCreateObjectFile<List<Task>>();
            var task = new Task
            {
                Id = tasks.Count + 1,
                Title = title,
                DateAndTime = DateTime.Now
            };
            tasks.Add(task);
            App.Current.SaveObjectFile(tasks);
        }

        public void Save(string title, string description = null, DateTime? date = null)
        {
            App.Current.Response.WriteLine("Executing: TaskCommand/Save/title/description/date");
            var tasks = App.Current.GetOrCreateObjectFile<List<Task>>();
            var task = new Task
            {
                Id = tasks.Count + 1,
                Title = title,
                Description = description,
                DateAndTime = date ?? DateTime.Now
            };
            tasks.Add(task);
            App.Current.SaveObjectFile(tasks);
        }

        public void Delete(int id)
        {
            App.Current.Response.WriteLine("Executing: TaskCommand/Delete/id");
            var tasks = App.Current.GetOrCreateObjectFile<List<Task>>();
            tasks.RemoveAll(t => t.Id == id);
            App.Current.SaveObjectFile(tasks);
        }
    }
}

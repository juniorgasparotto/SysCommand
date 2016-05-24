using Fclp;
using System;
using System.Linq.Expressions;
using System.Linq;
using System.Collections.Generic;

namespace SysCommand.Tests
{
    public class Task2Command : Command2
    {
        public Task2Command()
        {
            this.AllowSaveArgsInStorage = true;
        }

        public void GetTask(int id)
        {
            var tasks = App.Current.GetOrCreateObjectFile<List<Task>>();
            App.Current.Response.Post(tasks.FirstOrDefault(f=>f.Id == id));
        }

        public void Save(string title, string description, DateTime? date = null)
        {
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
    }
}

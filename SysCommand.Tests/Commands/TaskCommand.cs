using Fclp;
using System;
using System.Linq.Expressions;
using System.Linq;
using System.Collections.Generic;

namespace SysCommand.Tests
{
    public class TaskCommand : Command<Task>
    {
        public override void Execute()
        {
            if (this.Args.DateAndTime == default(DateTime))
                this.Args.DateAndTime = DateTime.Now;

            ConsoleWriter.Info("Title: " + this.Args.Title);
            ConsoleWriter.Info("Date: " + this.Args.DateAndTime);
            ConsoleWriter.Info("Description: " + this.Args.Description);

            // get with manage instances
            var tasksPath = App.Current.GetObjectFileName(typeof(List<Task>), useTypeFullName: true);
            var tasks = App.Current.GetOrCreateObjectFile<List<Task>>(tasksPath);
            tasks.Add(this.Args);
            App.Current.SaveObjectFile<List<Task>>(tasks, tasksPath);

            // save with manage instances
            App.Current.SaveObjectFile<Task>(this.Args);

            // save without manage instances
            ObjectFile.Save<Task>(this.Args, AppHelpers.GetPathFromRoot(@".app\tasks\task-" + DateTime.Now.Ticks + ".object"));
            ConsoleWriter.Success(string.Format("Task {0} saved", this.Args.Title));
        }
    }
}

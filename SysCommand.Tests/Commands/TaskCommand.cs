using Fclp;
using System;
using System.Linq.Expressions;
using System.Linq;
using System.Collections.Generic;

namespace SysCommand.Tests
{
    public class TaskCommand : Command<Task>
    {
        public TaskCommand()
        {
            this.AllowSaveArgsInStorage = true;
        }

        public override void Execute()
        {
            if (this.ArgsObject.DateAndTime == default(DateTime))
                this.ArgsObject.DateAndTime = DateTime.Now;

            ConsoleWriter.Info("Title: " + this.ArgsObject.Title);
            ConsoleWriter.Info("Date: " + this.ArgsObject.DateAndTime);
            ConsoleWriter.Info("Description: " + this.ArgsObject.Description);

            // get with manage instances
            var tasksPath = App.Current.GetObjectFileName(typeof(List<Task>), useTypeFullName: true);
            var tasks = App.Current.GetOrCreateObjectFile<List<Task>>(tasksPath);
            tasks.Add(this.ArgsObject);
            App.Current.SaveObjectFile<List<Task>>(tasks, tasksPath);

            // save with manage instances
            App.Current.SaveObjectFile<Task>(this.ArgsObject);

            // save without manage instances
            ObjectFile.Save<Task>(this.ArgsObject, AppHelpers.GetPathFromRoot(@".app\tasks\task-" + DateTime.Now.Ticks + ".object"));
            ConsoleWriter.Success(string.Format("Task {0} saved", this.ArgsObject.Title));
        }
    }
}

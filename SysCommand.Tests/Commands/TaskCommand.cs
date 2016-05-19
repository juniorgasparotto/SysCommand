using Fclp;
using System;
using System.Linq.Expressions;
using System.Linq;

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
            var tasks = App.Current.GetOrCreateObjectFile<Tasks>();
            tasks.AllTasks.Add(this.Args);
            App.Current.SaveObjectFile<Tasks>(tasks);

            // save with manage instances
            App.Current.SaveObjectFile<Task>(this.Args);

            // save without manage instances
            ObjectFile.Save<Task>(this.Args, AppHelpers.GetPathFromRoot(@"objects\IndividualTasks\" + DateTime.Now.Ticks + ".object"));
            ConsoleWriter.Success(string.Format("Task {0} saved", this.Args.Title));
        }
    }
}

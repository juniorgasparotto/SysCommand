using Fclp;
using System;
using System.Linq.Expressions;
using System.Linq;
using System.Collections.Generic;

namespace SysCommand.Tests
{
    public class SampleArgsCommand : CommandArguments<Task>
    {
        public SampleArgsCommand()
        {
            this.AllowSaveArgsInStorage = true;
        }

        public override void Execute()
        {
            App.Current.Response.WriteLine("Executing: SampleArgsCommand");
            App.Current.Response.WriteLine("Title: " + this.ArgsObject.Title);
            App.Current.Response.WriteLine("Date: " + this.ArgsObject.DateAndTime);
            App.Current.Response.WriteLine("Description: " + this.ArgsObject.Description);
        }
    }
}

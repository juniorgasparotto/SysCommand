using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Example.Views;
using SysCommand.ConsoleApp.View;

namespace Example
{
    using SysCommand.ConsoleApp;

    public class Program
    {
        public static int Main()
        {
            return App.RunApplication();
        }
    }

    public class HelloWorld1 : Command
    {
       
        public string HelloWorld(string myArg0, int? myArg1 = null)
        {
            return string.Format("My HelloWorld1 (Arg0: {0}; Arg1: {1})", myArg0, myArg1);
        }

        public string HelloWorld(string myArg0, DateTime myArg1)
        {
            return string.Format("My HelloWorld1 (Arg0: {0}; Arg1: {1})", myArg0, myArg1);
        }

        public string HelloWorld2(IEnumerable<string> myArg0)
        {
            return string.Format("My HelloWorld1 (Arg0: {0})", myArg0.Count());
        }
    }
    
    public class HelloWorld2 : Command
    {
        public string MyArg0 { get; set; }
        public string MyArg1 { get; set; }

        public string Main()
        {
            return string.Format("My HelloWorld2 (Arg0: {0}; Arg1: {1})", MyArg0, MyArg1);
        }
    }

    public class HelloWorld3 : Command
    {
        public decimal Test()
        {
            var result = this.App.Console.Read("My question: ");

            if (result != "S")
            {
                // option1: use write method
                this.App.Console.Write(99.99m);
                // option2: or use return, its the same.
                return 99.99m;
            }

            return 0;
        }
    }

    public class RazorCommand : Command
    {
        public string MyAction()
        {
            return View<MyModel>();
        }

        public string MyAction2()
        {
            var model = new MyModel
            {
                Name = "MyName"
            };

            return View(model, "MyAction.razor");
        }

        public class MyModel
        {
            public string Name { get; set; }
        }
    }

    public class T4Command : Command
    {
        public string T4MyAction()
        {
            return ViewT4<MyActionView>();
        }

        public string T4MyAction2()
        {
            var model = new MyModel
            {
                Name = "MyName"
            };

            return ViewT4<MyActionView, MyModel>(model);
        }

        public class MyModel
        {
            public string Name { get; set; }
        }
    }

    public class TableCommand : Command
    {
        public string MyTable()
        {
            var list = new List<MyModel>
            {
                new MyModel() {Id = "1", Column2 = "Line 1 Line 1"},
                new MyModel() {Id = "2 " , Column2 = "Line 2 Line 2"},
                new MyModel() {Id = "3", Column2 = "Line 3 Line 3"}
            };

            return TableView.ToTableView(list)
                            .Build()
                            .ToString();
        }

        public class MyModel
        {
            public string Id { get; set; }
            public string Column2 { get; set; }
        }
    }
}

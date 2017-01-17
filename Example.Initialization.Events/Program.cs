using System;
using System.Linq;
using SysCommand.ConsoleApp;

namespace Example.Initialization.Events
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();
            
            app.OnBeforeMemberInvoke += (appResult, memberResult) =>
            {
                app.Console.Write("Before: " + memberResult.Name);
            };

            app.OnAfterMemberInvoke += (appResult, memberResult) =>
            {
                app.Console.Write("After: " + memberResult.Name);
            };

            app.OnMethodReturn += (appResult, memberResult) =>
            {
                app.Console.Write("After MethodReturn: " + memberResult.Name);
            };

            app.OnComplete += (appResult) =>
            {
                app.Console.Write("Count: " + appResult.ExecutionResult.Results.Count());
                throw new Exception("Some error!!!");
            };

            app.OnException += (appResult, exception) =>
            {
                app.Console.Write(exception.Message);
            };

            app.Run(args);
        }

        public class FirstCommand : Command
        {
            public string MyProperty { get; set; }

            public string MyAction()
            {
                return "Return MyAction";
            }
        }
    }
}

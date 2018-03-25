using SysCommand.ConsoleApp;
using SysCommand.Helpers;
using SysCommand.Mapping;
using System;

namespace SysCommand.Tests.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Razor
            Run("View auto discover", "index");
            Run("View specify view", "index-specify-view");
            Run("View Dynamic Model", "index-dynamic-model");
            Run("Embedded auto discover", "embedded");

            // Execute commands
            Run("MyCommand.Main", "--my-property value");
            Run("MyCommand.MyAction - without parameter", "my-action");
            Run("MyCommand.MyAction - with parameter", "my-action --parameter 100");
            Run("MyOtherCommand.Main", "--parameter1 --parameter2 value");

            Run("TaskSaveCommand.Save", "save --title newTask");
            Run("TaskSaveCommand.Get", "get --title newTask");
            Run("TaskSaveCommand.Delete", "delete-by-title --title newTask");

            Console.ReadKey();
        }

        private static void Run(string name, string args)
        {
            Console.WriteLine("--> " + name);
            new App().Run(args);
            Console.WriteLine();
        }

        public class CustomCommand : Command
        {
            public string Index()
            {
                return View();
            }

            public string IndexSpecifyView()
            {
                return View(viewName: "Views/Custom/Index.cshtml");
            }

            public string IndexDynamicModel()
            {
                return View(new { Title = "MyTitle" });
            }

            public string Embedded()
            {
                return View(new { Title = "MyTitle" });
            }
        }

        public static string GetMethodSpecification(ActionMap map)
        {
            var format = "{0}({1})";
            string args = null;
            foreach (var arg in map.ArgumentsMaps)
            {
                var typeName = ReflectionHelper.CSharpName(arg.Type);
                args += args == null ? typeName : ", " + typeName;
            }
            return string.Format(format, map.ActionName, args);
        }
    }
}

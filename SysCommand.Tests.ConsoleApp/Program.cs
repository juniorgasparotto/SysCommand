using SysCommand.ConsoleApp;
using SysCommand.ConsoleApp.Files;
using SysCommand.Helpers;
using SysCommand.Mapping;

namespace SysCommand.Tests.ConsoleApp
{
    class Program
    {
        static int Main(string[] args)
        {
            var app = new App();
            var jsonFiles = app.Items.GetOrCreate<JsonFiles>();
            jsonFiles.DefaultFilePrefix = "file-";

            return App.RunInfiniteIfDebug(app);
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
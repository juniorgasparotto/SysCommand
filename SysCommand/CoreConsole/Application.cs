using System.Collections.Generic;
using System.Linq;
using System;
using Fclp;
using System.Linq.Expressions;
using System.IO;

namespace SysCommand.ConsoleApp
{
    public class Application
    {
        public static bool IsDebug { get { return System.Diagnostics.Debugger.IsAttached; } }
        public static bool WhenIsDebugReadArgs { get; set; }

        public static string[] GetArguments()
        {
            if (Application.IsDebug && Application.WhenIsDebugReadArgs)
            {
                Console.WriteLine("Enter with args:");
                return StringToArgs(Console.ReadLine());
            }
            else
            {
                var args = Environment.GetCommandLineArgs();
                var listArgs = args.ToList();
                // remove the app path that added auto by .net
                listArgs.RemoveAt(0);
                return listArgs.ToArray();
            }
        }

        public static string[] StringToArgs(string args)
        {
            if (!string.IsNullOrWhiteSpace(args))
                return AppHelpers.CommandLineToArgs(args);
            else
                return new string[0];
        }
    }
}

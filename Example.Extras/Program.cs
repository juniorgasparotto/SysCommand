using SysCommand.ConsoleApp;
using SysCommand.ConsoleApp.Helpers;
using SysCommand.Extras;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Example.Extras
{
    public class Program
    {
        public static void Main(string[] args)
        {
            while (true)
            {
                Console.Write(Strings.CmdIndicator);
                args = ConsoleAppHelper.StringToArgs(Console.ReadLine());

                bool verbosity = false;
                var shouldShowHelp = false;
                var names = new List<string>();

                var options = new OptionSet();

                options.Add(new OptionSet.Argument<List<string>>("name", "the name of someone to greet.")
                {
                    Action = (n) =>
                    {
                        if (n != null)
                            names.AddRange(n);
                    }
                });

                options.Add(new OptionSet.Argument<bool>('v', "show verbose")
                {
                    Action = (v) =>
                    {
                        verbosity = v;
                    }
                });

                options.Add(new OptionSet.Argument<bool>("help", "show help")
                {
                    Action = (h) =>
                    {
                        shouldShowHelp = h;
                    }
                });

                options.Parse(args);

                if (!options.ArgumentsInvalid.Any())
                {
                    Console.WriteLine("verbosity: " + verbosity);
                    Console.WriteLine("shouldShowHelp: " + shouldShowHelp);
                    Console.WriteLine("names.Count: " + names.Count);
                }
                else
                {
                    Console.WriteLine("error");
                }
            }
        }
    }
}
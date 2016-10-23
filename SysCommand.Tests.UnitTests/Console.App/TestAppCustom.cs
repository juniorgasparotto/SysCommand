using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using System.Linq;
using SysCommand.Tests.ConsoleApp.Commands;
using SysCommand.ConsoleApp;
using System.IO;
using SysCommand.Test;
using SysCommand.Parser;
using Mono.Options;

namespace SysCommand.Tests.UnitTests
{
    [TestClass]
    public class TestAppCustom
    {
        public TestAppCustom()
        {
            TestHelper.SetCultureInfoToInvariant();
        }

        [TestMethod]
        public void TestCustom2()
        {
            var args = AppHelpers.StringToArgs("--name glauber donizeti gasparotto junior -r 10 -v --help+");

            // these variables will be set when the command line is parsed
            bool verbosity = false;
            var shouldShowHelp = false;
            var names = new List<string>();
            var repeat = 1;

            // thses are the available options, not that they set the variables
            var options = new CustomEvaluator
            {
                {
                    "name", "the name of someone to greet.",
                    (List<string> n) =>
                    {
                        names.AddRange(n);
                    }
                },
                {
                    'r', "the number of times to repeat the greeting.",
                    (int r) => 
                    {
                        repeat = r;
                    }
                },
                {
                    'v', "increase debug message verbosity",
                    (bool v) => 
                    {
                        verbosity = v;
                    }
                },
                {
                    "help", "show this message and exit",
                    (bool h) => 
                    {
                        shouldShowHelp = h;
                    }
                }
            };

            var parseResult = options.Parse(args, null, false);
            var evaluationResult = options.Evaluate(args, null, parseResult);
            var app = new App(null, false, null, options);

        }

        [TestMethod]
        public void TestCustom()
        {
            var args = new String[] { "--name", "value", "--repeat", "0" };

            // these variables will be set when the command line is parsed
            var verbosity = 0;
            var shouldShowHelp = false;
            var names = new List<string>();
            var repeat = 1;

            // thses are the available options, not that they set the variables
            var options = new OptionSet {
                { "n|name=", "the name of someone to greet.", n => names.Add (n) },
                { "r|repeat=", "the number of times to repeat the greeting.", (int r) => repeat = r },
                { "v", "increase debug message verbosity", v => {
                    if (v != null)
                        ++verbosity;
                } },
                { "h|help", "show this message and exit", h => shouldShowHelp = h != null }
            };

            List<string> extra;
            try
            {
                // parse the command line
                extra = options.Parse(args);
            }
            catch (OptionException e)
            {
                // output some error message
                Console.Write("greet: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `greet --help' for more information.");
                return;
            }
        }
    }
}

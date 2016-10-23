using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using System.Linq;
using SysCommand.Tests.ConsoleApp.Commands;
using SysCommand.ConsoleApp;
using System.IO;
using SysCommand.Test;
using SysCommand.Parsing;
using Mono.Options;
using SysCommand.Evaluation;

namespace SysCommand.Tests.UnitTests
{
    [TestClass]
    public class TestArgumentSetWithoutApp
    {
        public TestArgumentSetWithoutApp()
        {
            TestHelper.SetCultureInfoToInvariant();
        }

        [TestMethod]
        public void TestWithoutApp()
        {
            var args = AppHelpers.StringToArgs("--name glauber donizeti gasparotto junior -r 10 -v --help+");

            // these variables will be set when the command line is parsed
            bool verbosity = false;
            var shouldShowHelp = false;
            var names = new List<string>();
            var repeat = 1;

            // thses are the available options, not that they set the variables
            var options = new ArgumentSet
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

            var parseResult = options.Parse(args);
            var evaluationResult = options.Evaluate(parseResult);

            switch (evaluationResult.State)
            {
                case EvaluateState.HasError:
                    Assert.Fail();
                    break;
                case EvaluateState.NotFound:
                    Assert.Fail();
                    break;
                case EvaluateState.Success:
                    Assert.IsTrue(verbosity == true);
                    Assert.IsTrue(shouldShowHelp == true);
                    Assert.IsTrue(names.Count == 4);
                    Assert.IsTrue(repeat == 10);
                    break;
                default:
                    Assert.Fail();
                    break;
            }
        }
    }
}

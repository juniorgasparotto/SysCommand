using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using SysCommand.Test;
using SysCommand.Execution;
using SysCommand.Utils;
using SysCommand.Utils.Extras;

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

            var options = new ArgumentSet();

            options.Add(new ArgumentSet.Argument<List<string>>("name", "the name of someone to greet.")
            {
                Action = (n) =>
                {
                    names.AddRange(n);
                }
            });

            options.Add(new ArgumentSet.Argument<int>('r', "param C")
            {
                Action = (r) =>
                {
                    repeat = r;
                }
            });

            options.Add(new ArgumentSet.Argument<bool>('v', "param verbose")
            {
                Action = (v) =>
                {
                    verbosity = v;
                }
            });

            options.Add(new ArgumentSet.Argument<int>('v', "show verbose")
            {
                Action = (r) =>
                {
                    repeat = r;
                }
            });

            options.Add(new ArgumentSet.Argument<bool>("help", "show help")
            {
                Action = (h) =>
                {
                    shouldShowHelp = h;
                }
            });

            options.Parse(args);

            if (options.ArgumentsInvalid.Empty())
            {
                Assert.IsTrue(verbosity == true);
                Assert.IsTrue(shouldShowHelp == true);
                Assert.IsTrue(names.Count == 4);
                Assert.IsTrue(repeat == 10);
            }
            else
            {
                Assert.Fail();
            }
        }
    }
}

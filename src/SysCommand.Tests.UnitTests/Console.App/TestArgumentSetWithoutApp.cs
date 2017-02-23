using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using SysCommand.TestUtils;
using SysCommand.Extras;
using System.Linq;
using SysCommand.ConsoleApp.Helpers;

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
            var args = ConsoleAppHelper.StringToArgs("--name glauber donizeti gasparotto junior -r 10 -v --help+");

            // these variables will be set when the command line is parsed
            bool verbosity = false;
            var shouldShowHelp = false;
            var names = new List<string>();
            var repeat = 1;

            var options = new OptionSet();

            options.Add(new OptionSet.Argument<List<string>>("name", "the name of someone to greet.")
            {
                Action = (n) =>
                {
                    names.AddRange(n);
                }
            });

            options.Add(new OptionSet.Argument<int>('r', "param C")
            {
                Action = (r) =>
                {
                    repeat = r;
                }
            });

            options.Add(new OptionSet.Argument<bool>('v', "param verbose")
            {
                Action = (v) =>
                {
                    verbosity = v;
                }
            });

            options.Add(new OptionSet.Argument<int>('v', "show verbose")
            {
                Action = (r) =>
                {
                    repeat = r;
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

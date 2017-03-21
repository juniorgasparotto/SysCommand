using System.Collections.Generic;
using SysCommand.Extras;
using System.Linq;
using SysCommand.ConsoleApp.Helpers;
using Xunit;
using SysCommand.Tests.UnitTests.Common;

namespace SysCommand.Tests.UnitTests
{
    
    public class TestArgumentSetWithoutApp
    {
        public TestArgumentSetWithoutApp()
        {
            TestHelper.Setup();
        }

        [Fact]
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
                Assert.True(verbosity == true);
                Assert.True(shouldShowHelp == true);
                Assert.True(names.Count == 4);
                Assert.True(repeat == 10);
            }
            else
            {
                Assert.True(false, "force fail");
            }
        }
    }
}

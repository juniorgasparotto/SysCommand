using SysCommand.TestUtils;
using System;
using Xunit;

namespace SysCommand.Tests.UnitTests
{
    public class TestConsoleInputParser
    {
        public TestConsoleInputParser()
        {
            TestHelper.Setup();
        }

        [Fact]
        public void Test1()
        {
            var args = ConsoleInputParser.Parse(@"'asdsad\'");
            Assert.True(args.Length == 1);
            Assert.True(args[0] == @"asdsad'");
        }

        [Fact]
        public void Test2()
        {
            var args = ConsoleInputParser.Parse(@"'asds'");
            Assert.True(args.Length == 1);
            Assert.True(args[0] == @"asds");
        }

        [Fact]
        public void Test3()
        {
            var args = ConsoleInputParser.Parse(@"a a ");
            Assert.True(args.Length == 2);
            Assert.True(args[0] == @"a");
            Assert.True(args[1] == @"a");
        }

        [Fact]
        public void Test4()
        {
            var args = ConsoleInputParser.Parse(@" a a ");
            Assert.True(args.Length == 2);
            Assert.True(args[0] == @"a");
            Assert.True(args[1] == @"a");
        }

        [Fact]
        public void Test5()
        {
            var args = ConsoleInputParser.Parse(@"  a");
            Assert.True(args.Length == 1);
            Assert.True(args[0] == @"a");
        }

        [Fact]
        public void Test6()
        {
            var args = ConsoleInputParser.Parse(@"a  ");
            Assert.True(args.Length == 1);
            Assert.True(args[0] == @"a");
        }

        [Fact]
        public void Test7()
        {
            var args = ConsoleInputParser.Parse(@"""aaaa    """);
            Assert.True(args.Length == 1);
            Assert.True(args[0] == @"aaaa    ");
        }

        [Fact]
        public void Test8()
        {
            var args = ConsoleInputParser.Parse(@"""    aaaa    """);
            Assert.True(args.Length == 1);
            Assert.True(args[0] == @"    aaaa    ");
        }

        [Fact]
        public void Test9()
        {
            var args = ConsoleInputParser.Parse(@"a """"");
            Assert.True(args.Length == 2);
            Assert.True(args[0] == @"a");
            Assert.True(args[1] == @"");
        }

        [Fact]
        public void Test10()
        {
            var args = ConsoleInputParser.Parse(@"sdjkas'asdsakl""asdas""'");
            Assert.True(args.Length == 1);
            Assert.True(args[0] == @"sdjkasasdsakl""asdas""");
        }

        [Fact]
        public void Test11()
        {
            var args = ConsoleInputParser.Parse(@"sdjkas'asdsakl\\""asdas""'");
            Assert.True(args.Length == 1);
            Assert.True(args[0] == @"sdjkasasdsakl\\""asdas""");
        }

        [Fact]
        public void Test12()
        {
            var args = ConsoleInputParser.Parse(@"");
            Assert.True(args.Length == 0);
        }

        [Fact]
        public void Test13()
        {
            var args = ConsoleInputParser.Parse(@"     ");
            Assert.True(args.Length == 0);
        }

        [Fact]
        public void Test14()
        {
            var args = ConsoleInputParser.Parse(@"abc'defg \' 'hi 123 ""456 678\""quote\"""" ");
            Assert.True(args.Length == 3);
            Assert.True(args[0] == @"abcdefg ' hi");
            Assert.True(args[1] == @"123");
            Assert.True(args[2] == "456 678\"quote\"");
        }
    }
}

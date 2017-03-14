using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SysCommand.Tests.UnitTests
{
    [TestClass]
    public class TestConsoleInputParser
    {
        [TestMethod]
        public void Test1()
        {
            var args = ConsoleInputParser.Parse(@"'asdsad\'");
            Assert.IsTrue(args.Length == 1);
            Assert.IsTrue(args[0] == @"asdsad'");
        }

        [TestMethod]
        public void Test2()
        {
            var args = ConsoleInputParser.Parse(@"'asds'");
            Assert.IsTrue(args.Length == 1);
            Assert.IsTrue(args[0] == @"asds");
        }

        [TestMethod]
        public void Test3()
        {
            var args = ConsoleInputParser.Parse(@"a a ");
            Assert.IsTrue(args.Length == 2);
            Assert.IsTrue(args[0] == @"a");
            Assert.IsTrue(args[1] == @"a");
        }

        [TestMethod]
        public void Test4()
        {
            var args = ConsoleInputParser.Parse(@" a a ");
            Assert.IsTrue(args.Length == 2);
            Assert.IsTrue(args[0] == @"a");
            Assert.IsTrue(args[1] == @"a");
        }

        [TestMethod]
        public void Test5()
        {
            var args = ConsoleInputParser.Parse(@"  a");
            Assert.IsTrue(args.Length == 1);
            Assert.IsTrue(args[0] == @"a");
        }

        [TestMethod]
        public void Test6()
        {
            var args = ConsoleInputParser.Parse(@"a  ");
            Assert.IsTrue(args.Length == 1);
            Assert.IsTrue(args[0] == @"a");
        }

        [TestMethod]
        public void Test7()
        {
            var args = ConsoleInputParser.Parse(@"""aaaa    """);
            Assert.IsTrue(args.Length == 1);
            Assert.IsTrue(args[0] == @"aaaa    ");
        }

        [TestMethod]
        public void Test8()
        {
            var args = ConsoleInputParser.Parse(@"""    aaaa    """);
            Assert.IsTrue(args.Length == 1);
            Assert.IsTrue(args[0] == @"    aaaa    ");
        }

        [TestMethod]
        public void Test9()
        {
            var args = ConsoleInputParser.Parse(@"a """"");
            Assert.IsTrue(args.Length == 2);
            Assert.IsTrue(args[0] == @"a");
            Assert.IsTrue(args[1] == @"");
        }

        [TestMethod]
        public void Test10()
        {
            var args = ConsoleInputParser.Parse(@"sdjkas'asdsakl""asdas""'");
            Assert.IsTrue(args.Length == 1);
            Assert.IsTrue(args[0] == @"sdjkasasdsakl""asdas""");
        }

        [TestMethod]
        public void Test11()
        {
            var args = ConsoleInputParser.Parse(@"sdjkas'asdsakl\\""asdas""'");
            Assert.IsTrue(args.Length == 1);
            Assert.IsTrue(args[0] == @"sdjkasasdsakl\\""asdas""");
        }

        [TestMethod]
        public void Test12()
        {
            var args = ConsoleInputParser.Parse(@"");
            Assert.IsTrue(args.Length == 0);
        }

        [TestMethod]
        public void Test13()
        {
            var args = ConsoleInputParser.Parse(@"     ");
            Assert.IsTrue(args.Length == 0);
        }

        [TestMethod]
        public void Test14()
        {
            var args = ConsoleInputParser.Parse(@"abc'defg \' 'hi 123 ""456 678\""quote\"""" ");
            Assert.IsTrue(args.Length == 3);
            Assert.IsTrue(args[0] == @"abcdefg ' hi");
            Assert.IsTrue(args[1] == @"123");
            Assert.IsTrue(args[2] == "456 678\"quote\"");
        }
    }
}

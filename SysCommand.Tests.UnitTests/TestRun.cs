using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using SysCommand.Tests.ConsoleApp.Commands;
using System;
using System.Linq;

namespace SysCommand.Tests.UnitTests
{
    [TestClass]
    public class TestRun
    {
        [TestMethod]
        [ExpectedException(typeof(Exception), "No command found")]
        public void TestNoCommands()
        {
            var result = new Executor(null, new List<Command>()).Execute();
        }

        [TestMethod]
        public void TestEmptyCommand()
        {
            var result = new Executor(null, new List<Command>() { new EmptyCommand() }).Execute();
            Assert.IsTrue(!result.Any());
        }

        [TestMethod]
        public void TestMainCommand()
        {
            var result = new Executor(null, new List<Command>() { new MainCommand() }).Execute();
            var value = result.WithAlias("Main").GetValue<string>();
            var methods = result.With<MethodMain>().First();

            Assert.IsTrue(result.Count() == 1);
            Assert.IsTrue(value == "Main");
        }
    }
}

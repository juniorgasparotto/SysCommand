using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using System.Linq;
using SysCommand;
using SysCommand.Tests.ConsoleApp.Commands;
using SysCommand.ConsoleApp;
using System.IO;

namespace SysCommand.Tests.UnitTests
{
    [TestClass]
    public class TestRun
    {
        [TestMethod]
        [ExpectedException(typeof(Exception), "No command found")]
        public void TestNoCommands()
        {
            var strWrite = new StringWriter();
            var result = new Application(
                    args: new string[0],
                    commands: new List<CommandBase>(),
                    output: strWrite
                ).Execute();
        }

        [TestMethod]
        public void TestEmptyCommand()
        {
            var strWrite = new StringWriter();
            var result = new Application(
                    args: new string[0],
                    command: new EmptyCommand(),
                    output: strWrite
                ).Execute();

            Assert.IsTrue(!result.Any());
        }

        [TestMethod]
        public void TestMainCommand()
        {
            var strWrite = new StringWriter();
            var result = new Application(
                    args: new string[0],
                    command: new MainCommand(),
                    output: strWrite
                ).Execute();

            var value = result.WithAlias("Main").GetValue<string>();
            var methods = result.With<MethodMain>().First();

            Assert.IsTrue(result.Count() == 1);
            Assert.IsTrue(value == "Main");
        }
    }
}

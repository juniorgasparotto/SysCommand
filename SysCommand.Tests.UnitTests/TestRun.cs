using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using System.Linq;
using SysCommand.Tests.ConsoleApp.Commands;
using SysCommand.ConsoleApp;
using System.IO;

namespace SysCommand.Tests.UnitTests
{
    [TestClass]
    public class TestRun
    {
        [TestMethod]
        public void TestNoCommands()
        {
            try
            {
                var result = this.GetApp("", new List<Command>()).Run();
            }
            catch(Exception ex)
            {
                Assert.IsTrue(ex.Message == "No command found");
            }
        }

        [TestMethod]
        public void TestEmptyCommand()
        {
            var result = this.GetApp("", new EmptyCommand()).Run();
            Assert.IsTrue(!result.Any());
        }

        [TestMethod]
        public void TestMainCommand()
        {
            var result = this.GetApp("", new MainCommand()).Run();
            var value = result.WithAlias("Main").GetValue<string>();
            var methods = result.With<MethodMain>().First();

            Assert.IsTrue(result.Count() == 1);
            Assert.IsTrue(value == "Main");
        }

        [TestMethod]
        public void TestHelp()
        {
            var result = this.GetApp("", new MainCommand()).Run();

            var value = result.WithAlias("Main").GetValue<string>();
            var methods = result.With<MethodMain>().First();

            Assert.IsTrue(result.Count() == 1);
            Assert.IsTrue(value == "Main");
        }

        private App GetApp(string args, Command cmd)
        {
            var app = new App(
                    args: args,
                    command: cmd,
                    output: new StringWriter()
                );
            return app;
        }

        private App GetApp(string args, List<Command> cmds)
        {
            var app = new App(
                    args: args,
                    commands: cmds,
                    output: new StringWriter()
                );
            return app;
        }
    }
}

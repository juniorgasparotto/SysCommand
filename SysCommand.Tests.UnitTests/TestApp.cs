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
    public class TestApp
    {
        [TestMethod]
        public void TestNoCommandsException()
        {
            try
            {
                var result = this.GetApp(new List<Command>()).Run("");
            }
            catch(Exception ex)
            {
                Assert.IsTrue(ex.Message == "No command found");
            }
        }

        [TestMethod]
        public void TestCommandsAttachedInOtherAppException()
        {
            try
            {
                var commands = new List<Command>() { new OnlyMainCommand() };
                this.GetApp(commands).Run("");
                this.GetApp(commands).Run("");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message == "The command 'SysCommand.Tests.ConsoleApp.Commands.OnlyMainCommand' already attached to another application.");
            }
        }

        [TestMethod]
        public void TestNoArgsAndEmptyCommand()
        {
            var app = this.GetApp(new EmptyCommand());
            var result = app.Run("");
            var output = app.Console.Out.ToString();
            Assert.IsTrue(result.Empty());
            Assert.IsTrue(output == Strings.NotFoundMessage);
        }

        [TestMethod]
        public void TestMainCommand()
        {
            var result = this.GetApp(new OnlyMainCommand()).Run("-a Y");
            Assert.IsTrue(result.Count == 2);
            Assert.IsTrue(result.With<Property>().GetNullableValue<char>() == 'Y');
            Assert.IsTrue("Main" == result.WithAlias("Main").GetValue<string>());
        }

        [TestMethod]
        public void TestHelp()
        {
            var result = this.GetApp(new OnlyMainCommand()).Run("");

            var value = result.WithAlias("Main").GetValue<string>();
            var methods = result.With<MethodMain>().First();

            Assert.IsTrue(result.Count() == 1);
            Assert.IsTrue(value == "Main");
        }

        private App GetApp(Command cmd)
        {
            var app = new App(
                    //args: AppHelpers.StringToArgs(args),
                    commands: new List<Command> { cmd }
                );
            app.Console.Out = new StringWriter();
            return app;
        }

        private App GetApp(List<Command> cmds)
        {
            var app = new App(
                    //args: AppHelpers.StringToArgs(args),
                    commands: cmds
                );
            return app;
        }
    }
}

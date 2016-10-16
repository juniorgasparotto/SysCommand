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
                var result = this.GetApp(new SysCommand.ConsoleApp.Command[0]).Run("");
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
                var commands = new SysCommand.ConsoleApp.Command[1] { new OnlyMainCommand() };
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
            var appResult = app.Run("");
            var output = app.Console.Out.ToString();
            Assert.IsTrue(appResult.EvaluateResult.Result.Empty());
            Assert.IsTrue(output == Strings.NotFoundMessage);
        }

        [TestMethod]
        public void TestMainCommand()
        {
            var appResult = this.GetApp(new OnlyMainCommand()).Run("-a Y");
            Assert.IsTrue(appResult.EvaluateResult.Result.Count == 2);
            Assert.IsTrue(appResult.EvaluateResult.Result.With<Property>().GetNullableValue<char>() == 'Y');
            Assert.IsTrue("Main" == appResult.EvaluateResult.Result.WithName("Main").GetValue<string>());
        }
       
        [TestMethod]
        public void TestDefaultCallMainButDeclared()
        {
            var app = this.GetApp(new Command1(), new Command2());
            app.Console.Out = new StringWriter();
            var appResult = app.Run("main 1 b");

            var output = app.Console.Out.ToString();
            var expected =
@"";

            Assert.IsTrue(expected == output);
            Assert.IsTrue(appResult.EvaluateResult.Result.Count == 2);
            Assert.IsTrue(appResult.EvaluateResult.Result.With<Method>().Count == 2);
            Assert.IsTrue(appResult.EvaluateResult.Result.With<MethodMain>().Count == 2);
            Assert.IsTrue(appResult.EvaluateResult.Result.With<Property>().Count == 2);
        }

        [TestMethod]
        public void TestDefaultCallMainAndIgnoreByOrderTheSaveMethod()
        {
            var app = this.GetApp(new Command1(), new Command2());
            app.Console.Out = new StringWriter();
            var appResult = app.Run("1 a");

            var output = app.Console.Out.ToString();
            var expected =
@"";

            Assert.IsTrue(expected == output);
            Assert.IsTrue(appResult.EvaluateResult.Result.Count == 2);
            Assert.IsTrue(appResult.EvaluateResult.Result.With<Method>().Count == 2);
            Assert.IsTrue(appResult.EvaluateResult.Result.With<MethodMain>().Count == 2);
            Assert.IsTrue(appResult.EvaluateResult.Result.With<Property>().Count == 2);
        }

        [TestMethod]
        public void TestDefaultCallSaveAndIgnoreByTypingTheMainMethod()
        {
            var app = this.GetApp(new Command1(), new Command2());
            app.Console.Out = new StringWriter();
            var appResult = app.Run("a a");

            var output = app.Console.Out.ToString();
            var expected =
@"";

            Assert.IsTrue(expected == output);
            Assert.IsTrue(appResult.EvaluateResult.Result.Count == 2);
            Assert.IsTrue(appResult.EvaluateResult.Result.With<Method>().Count == 2);
            Assert.IsTrue(appResult.EvaluateResult.Result.With<MethodMain>().Count == 2);
            Assert.IsTrue(appResult.EvaluateResult.Result.With<Property>().Count == 2);
        }

        [TestMethod]
        public void TestDefaultCallSaveAndIgnoreByTypingTheMainMethod2()
        {
            var app = this.GetApp(new Command1(), new Command2());
            app.Console.Out = new StringWriter();
            var appResult = app.Run("--id a b");

            var output = app.Console.Out.ToString();
            var expected =
@"";

            Assert.IsTrue(expected == output);
            Assert.IsTrue(appResult.EvaluateResult.Result.Count == 2);
            Assert.IsTrue(appResult.EvaluateResult.Result.With<Method>().Count == 2);
            Assert.IsTrue(appResult.EvaluateResult.Result.With<MethodMain>().Count == 2);
            Assert.IsTrue(appResult.EvaluateResult.Result.With<Property>().Count == 2);
        }

        [TestMethod]
        public void TestDefaultCallAllDefaultsWithZeroArgsInEachCommand()
        {
            var app = this.GetApp(new Command1(), new Command2());
            app.Console.Out = new StringWriter();
            var appResult = app.Run("");

            var output = app.Console.Out.ToString();
            var expected =

@"Command1.default()
Command2.default()";
//Command2.default2()";

            Assert.IsTrue(expected == output);
            Assert.IsTrue(appResult.EvaluateResult.Result.With<Method>().Count == 1);
            Assert.IsTrue(appResult.EvaluateResult.Result.With<MethodMain>().Count == 0);
            Assert.IsTrue(appResult.EvaluateResult.Result.With<Property>().Count == 0);
            Assert.IsTrue(appResult.EvaluateResult.Result.Count == 2);
        }

        [TestMethod]
        public void TestDefaultCallMainByTypingAndSaveAfter()
        {
            var app = this.GetApp(new Command1(), new Command2());
            app.Console.Out = new StringWriter();
            var appResult= app.Run("1 a save");

            var output = app.Console.Out.ToString();
            var expected =
@"";

            Assert.IsTrue(expected == output);
            Assert.IsTrue(appResult.EvaluateResult.Result.Count == 2);
            Assert.IsTrue(appResult.EvaluateResult.Result.With<Method>().Count == 2);
            Assert.IsTrue(appResult.EvaluateResult.Result.With<MethodMain>().Count == 2);
            Assert.IsTrue(appResult.EvaluateResult.Result.With<Property>().Count == 2);
        }

        [TestMethod]
        public void TestMultiActionOrderExecution()
        {
            var app = this.GetApp(new Command1(), new Command2());
            app.Console.Out = new StringWriter();
            var appResult= app.Run("save delete");

            var output = app.Console.Out.ToString();
            var expected =
@"";

            Assert.IsTrue(expected == output);
            Assert.IsTrue(appResult.EvaluateResult.Result.Count == 2);
            Assert.IsTrue(appResult.EvaluateResult.Result.With<Method>().Count == 2);
            Assert.IsTrue(appResult.EvaluateResult.Result.With<MethodMain>().Count == 2);
            Assert.IsTrue(appResult.EvaluateResult.Result.With<Property>().Count == 2);
        }

        [TestMethod]
        public void TestMultiActionErrorPerLevel()
        {
            var app = this.GetApp(new Command1(), new Command2());
            app.Console.Out = new StringWriter();
            var appResult= app.Run("delete a save");

            var output = app.Console.Out.ToString();
            var expected =
@"";

            Assert.IsTrue(expected == output);
            Assert.IsTrue(appResult.EvaluateResult.Result.Count == 2);
            Assert.IsTrue(appResult.EvaluateResult.Result.With<Method>().Count == 2);
            Assert.IsTrue(appResult.EvaluateResult.Result.With<MethodMain>().Count == 2);
            Assert.IsTrue(appResult.EvaluateResult.Result.With<Property>().Count == 2);
        }

        [TestMethod]
        public void TestMultiAction1()
        {
            var app = this.GetApp(new Command1(), new Command2(), new Command3(), new Command4());
            app.Console.Out = new StringWriter();
            var appResult= app.Run("--prop1 value1 --prop2 value2");

            var output = app.Console.Out.ToString();
            var expected =
@"";

            Assert.IsTrue(expected == output);
            Assert.IsTrue(appResult.EvaluateResult.Result.Count == 2);
            Assert.IsTrue(appResult.EvaluateResult.Result.With<Method>().Count == 2);
            Assert.IsTrue(appResult.EvaluateResult.Result.With<MethodMain>().Count == 2);
            Assert.IsTrue(appResult.EvaluateResult.Result.With<Property>().Count == 2);
        }

        [TestMethod]
        public void TestMultiAction2()
        {
            var app = this.GetApp( new Command5() );
            //var app = this.GetApp(new Command1(), new Command2(), new Command3(), new Command4());
            app.Console.Out = new StringWriter();
            var appResult= app.Run("get 1 a --description a get 1 a --description a");

            var output = app.Console.Out.ToString();
            var expected =
@"";

            Assert.IsTrue(expected == output);
            Assert.IsTrue(appResult.EvaluateResult.Result.Count == 2);
            Assert.IsTrue(appResult.EvaluateResult.Result.With<Method>().Count == 2);
            Assert.IsTrue(appResult.EvaluateResult.Result.With<MethodMain>().Count == 2);
            Assert.IsTrue(appResult.EvaluateResult.Result.With<Property>().Count == 2);
        }

        [TestMethod]
        public void TestChoosedByAllValidsAndHaveMajorityAsMappedAndInputIsValid()
        {
            // one command and single action
            var app = this.GetApp(new Commands.Command7());
            app.Console.Out = new StringWriter();
            var appResult= app.Run("save 1 2 value");

            var output = app.Console.Out.ToString();
            var expected =
@"";

            Assert.IsTrue(expected == output);
            Assert.IsTrue(appResult.EvaluateResult.Result.Count == 2);
            Assert.IsTrue(appResult.EvaluateResult.Result.With<Method>().Count == 2);
            Assert.IsTrue(appResult.EvaluateResult.Result.With<MethodMain>().Count == 2);
            Assert.IsTrue(appResult.EvaluateResult.Result.With<Property>().Count == 2);

            // one command and multi action
//            app = this.GetApp(new Command6());
//            //var app = this.GetApp(new Command1(), new Command2(), new Command3(), new Command4());
//            app.Console.Out = new StringWriter();
//            result = app.Run("save 1 2 value save 1 2 value");

//            output = app.Console.Out.ToString();
//            expected =
//@"";

//            Assert.IsTrue(expected == output);
//            Assert.IsTrue(result.Count == 2);
//            Assert.IsTrue(result.With<Method>().Count == 2);
//            Assert.IsTrue(result.With<MethodMain>().Count == 2);
//            Assert.IsTrue(result.With<Property>().Count == 2);
        }

        [TestMethod]
        public void TestNoDefaultMethodOnlyHasPropertiesInFirstLevel()
        {
            // one command and single action
            var app = this.GetApp(new Commands.Command8());
            app.Console.Out = new StringWriter();
            var appResult= app.Run("--prop1 --prop2 save 1 2");

            var output = app.Console.Out.ToString();
            var expected =
@"";

            Assert.IsTrue(expected == output);
            Assert.IsTrue(appResult.EvaluateResult.Result.Count == 2);
            Assert.IsTrue(appResult.EvaluateResult.Result.With<Method>().Count == 2);
            Assert.IsTrue(appResult.EvaluateResult.Result.With<MethodMain>().Count == 2);
            Assert.IsTrue(appResult.EvaluateResult.Result.With<Property>().Count == 2);

            // one command and multi action
            app = this.GetApp(new Commands.Command8());
            //var app = this.GetApp(new Command1(), new Command2(), new Command3(), new Command4());
            app.Console.Out = new StringWriter();
            var appResult2 = app.Run("--prop1 --prop2 save 1 2 delete 1");

            output = app.Console.Out.ToString();
            expected =
@"";

            Assert.IsTrue(expected == output);
            Assert.IsTrue(appResult.EvaluateResult.Result.Count == 2);
            Assert.IsTrue(appResult.EvaluateResult.Result.With<Method>().Count == 2);
            Assert.IsTrue(appResult.EvaluateResult.Result.With<MethodMain>().Count == 2);
            Assert.IsTrue(appResult.EvaluateResult.Result.With<Property>().Count == 2);
        }

        [TestMethod]
        public void TestHelpAllMembers()
        {
            var app = this.GetApp(new Command1(), new Command2());
            app.Console.Out = new StringWriter();
            var appResult= app.Run("help");

            var output = app.Console.Out.ToString();
            var expected =
@"";

            Assert.IsTrue(expected == output);
            Assert.IsTrue(appResult.EvaluateResult.Result.Count == 2);
            Assert.IsTrue(appResult.EvaluateResult.Result.With<Method>().Count == 2);
            Assert.IsTrue(appResult.EvaluateResult.Result.With<MethodMain>().Count == 2);
            Assert.IsTrue(appResult.EvaluateResult.Result.With<Property>().Count == 2);
        }

        [TestMethod]
        public void TestHelpSpecifyMember()
        {
            var app = this.GetApp(new Command1(), new Command2());
            app.Console.Out = new StringWriter();
            var appResult= app.Run("help save");

            var output = app.Console.Out.ToString();
            var expected =
@"";

            Assert.IsTrue(expected == output);
            Assert.IsTrue(appResult.EvaluateResult.Result.Count == 2);
            Assert.IsTrue(appResult.EvaluateResult.Result.With<Method>().Count == 2);
            Assert.IsTrue(appResult.EvaluateResult.Result.With<MethodMain>().Count == 2);
            Assert.IsTrue(appResult.EvaluateResult.Result.With<Property>().Count == 2);
        }

        [TestMethod]
        public void TestHelpInEnd()
        {
            var app = this.GetApp(new Command1(), new Command2());
            app.Console.Out = new StringWriter();
            var appResult= app.Run("save help");

            var output = app.Console.Out.ToString();
            var expected =
@"";

            Assert.IsTrue(expected == output);
            Assert.IsTrue(appResult.EvaluateResult.Result.Count == 2);
            Assert.IsTrue(appResult.EvaluateResult.Result.With<Method>().Count == 2);
            Assert.IsTrue(appResult.EvaluateResult.Result.With<MethodMain>().Count == 2);
            Assert.IsTrue(appResult.EvaluateResult.Result.With<Property>().Count == 2);
        }

        [TestMethod]
        public void TestProperty()
        {
            var app = this.GetApp(new Command1(), new Command2());
            app.Console.Out = new StringWriter();
            var appResult= app.Run("save 1 --id=10");

            var output = app.Console.Out.ToString();
            var expected =
@"";

            Assert.IsTrue(expected == output);
            Assert.IsTrue(appResult.EvaluateResult.Result.Count == 2);
            Assert.IsTrue(appResult.EvaluateResult.Result.With<Method>().Count == 2);
            Assert.IsTrue(appResult.EvaluateResult.Result.With<MethodMain>().Count == 2);
            Assert.IsTrue(appResult.EvaluateResult.Result.With<Property>().Count == 2);
        }

        private App GetApp(params SysCommand.ConsoleApp.Command[] cmds)
        {
            var app = new App(
                    //args: AppHelpers.StringToArgs(args),
                    commands: cmds
                );
            app.Console.Out = new StringWriter();
            return app;
        }

    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using System.Linq;
using SysCommand.ConsoleApp;
using System.IO;
using SysCommand.Test;
using SysCommand.ConsoleApp.Files;
using SysCommand.ConsoleApp.Commands;
using System.Threading;
using SysCommand.Tests.UnitTests.Commands.T06;

namespace SysCommand.Tests.UnitTests
{
    [TestClass]
    public class TestAppHistory
    {
        public TestAppHistory()
        {
            TestHelper.SetCultureInfoToInvariant();
        }

        [TestMethod]
        public void Test_HistorySave_OtherActions_NotSave()
        {
            this.Compare(
                args: "save 1",
                commandsTypes: GetCmds(
                    new Commands.T31.Command1(),
                    new Commands.T31.Command2(),
                    new Commands.T31.Command3()
                ),
                funcName: TestHelper.GetCurrentMethodName()

            );
        }

        [TestMethod]
        public void Test_HistorySave_SpecialCharName_Save()
        {
            this.Compare(
                args: "save 1 history-save &",
                commandsTypes: GetCmds(
                    new Commands.T31.Command1(),
                    new Commands.T31.Command2(),
                    new Commands.T31.Command3()
                ),
                funcName: TestHelper.GetCurrentMethodName()

            );
        }

        [TestMethod]
        public void Test_HistorySave_InvalidArgs_NotSave()
        {
            this.Compare(
                args: "save a history-save history1",
                commandsTypes: GetCmds(
                    new Commands.T31.Command1(),
                    new Commands.T31.Command2(),
                    new Commands.T31.Command3()
                ),
                funcName: TestHelper.GetCurrentMethodName()

            );
        }

        [TestMethod]
        public void Test_HistorySave_NoArgsAndOnlySave_NoSave()
        {
            this.Compare(
                args: "history-save history1",
                commandsTypes: GetCmds(
                    new Commands.T31.Command1(),
                    new Commands.T31.Command2(),
                    new Commands.T31.Command3()
                ),
                funcName: TestHelper.GetCurrentMethodName()

            );
        }

        [TestMethod]
        public void Test_HistorySave_WithArgsAndNameWithScapedActionName_SaveWithoutScape()
        {
            this.Compare(
                args: @"--prop1 a --prop2 2 save 2 history-save \save",
                commandsTypes: GetCmds(
                    new Commands.T31.Command1(),
                    new Commands.T31.Command2(),
                    new Commands.T31.Command3()
                ),
                funcName: TestHelper.GetCurrentMethodName()

            );
        }

        [TestMethod]
        public void Test_HistorySave_WithArgsAndNameWithActionName_NotSave()
        {
            this.Compare(
                args: @"--prop1 a --prop2 2 save 2 history-save save",
                commandsTypes: GetCmds(
                    new Commands.T31.Command1(),
                    new Commands.T31.Command2(),
                    new Commands.T31.Command3()
                ),
                funcName: TestHelper.GetCurrentMethodName()

            );
        }

        private Type[] GetCmds(params Command[] commands)
        {
            return commands.Select(f=> f.GetType()).ToArray();
        }

        private void Compare(string args, Type[] commandsTypes, string funcName)
        {
            commandsTypes = new List<Type>(commandsTypes)
            {
                typeof(ArgsHistoryCommand)
            }.ToArray();

            var app = new App(
                  commandsTypes: commandsTypes
            );

            app.Console.Out = new StringWriter();

            var fileManager = app.Items.GetOrCreate<JsonFileManager>();
            fileManager.SaveInRootFolderWhenIsDebug = false;

            try
            {
                app.Run(args);

                var test = new TestData();
                test.Args = args;
                test.ExpectedResult = app.Console.Out.ToString().Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                test.HistoryFile = fileManager.Get<List<ArgsHistoryCommand.History>>(ArgsHistoryCommand.FILE_NAME);

                foreach (var cmd in app.Commands)
                {
                    test.Members.AddRange(app.Maps.Where(f => f.Command.GetType() == cmd.GetType()).SelectMany(f => f.Properties.Select(s => s.Target.GetType().Name + "." + s.TargetMember.ToString() + (s.IsOptional ? "" : " (obrigatory)") + (cmd.EnablePositionalArgs ? "" : " (NOT accept positional)"))));
                    test.Members.AddRange(app.Maps.Where(f => f.Command.GetType() == cmd.GetType()).SelectMany(f => f.Methods.Select(s => s.Target.GetType().Name + "." + CommandParserUtils.GetMethodSpecification(s))));
                }

                var result = TestHelper.CompareObjects<TestAppHistory>(test, null, funcName);
                Assert.IsTrue(result);
            }
            catch
            {
                throw;
            }
            finally
            {
                fileManager.Remove(ArgsHistoryCommand.FILE_NAME);
            }
        }

        public class TestData
        {
            public string Args { get; internal set; }
            public List<string> Members { get; internal set; }
            public string[] ExpectedResult { get; set; }
            public List<ArgsHistoryCommand.History> HistoryFile { get; set; }

            public TestData()
            {
                this.Members = new List<string>();
            }
        }
    }
}

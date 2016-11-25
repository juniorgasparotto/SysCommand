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
using System.Reflection;

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
        public void Test_HistorySave_PositionalAtTheEnd_Save()
        {
            this.Compare(
                args: "save 1 history-save history1",
                commandsTypes: GetCmds(
                    new Commands.T31.Command1(),
                    new Commands.T31.Command2(),
                    new Commands.T31.Command3()
                ),
                funcName: TestHelper.GetCurrentMethodName()

            );
        }

        [TestMethod]
        public void Test_HistorySave_PositionalAndHaveNextAction_Save()
        {
            this.Compare(
                args: "save 1 history-save history1 save 1",
                commandsTypes: GetCmds(
                    new Commands.T31.Command1(),
                    new Commands.T31.Command2(),
                    new Commands.T31.Command3()
                ),
                funcName: TestHelper.GetCurrentMethodName()

            );
        }

        [TestMethod]
        public void Test_HistorySave_NamedAtTheEnd_Save()
        {
            this.Compare(
                args: "save 1 history-save --name history1",
                commandsTypes: GetCmds(
                    new Commands.T31.Command1(),
                    new Commands.T31.Command2(),
                    new Commands.T31.Command3()
                ),
                funcName: TestHelper.GetCurrentMethodName()

            );
        }

        [TestMethod]
        public void Test_HistorySave_NamedAndHaveNextAction_Save()
        {
            this.Compare(
                args: "save 1 history-save --name history1 save",
                commandsTypes: GetCmds(
                    new Commands.T31.Command1(),
                    new Commands.T31.Command2(),
                    new Commands.T31.Command3()
                ),
                funcName: TestHelper.GetCurrentMethodName()

            );
        }

        [TestMethod]
        public void Test_HistorySave_NamedValueIsNullAndHaveNextAction_NotSave()
        {
            try
            {
                this.Compare(
                    args: "save 1 history-save --name save",
                    commandsTypes: GetCmds(
                        new Commands.T31.Command1(),
                        new Commands.T31.Command2(),
                        new Commands.T31.Command3()
                    ),
                    funcName: TestHelper.GetCurrentMethodName()
                );

            }
            catch (TargetInvocationException ex)
            {
                var ex2 = (ArgumentNullException)ex.InnerException;
                Assert.IsTrue(ex2.ParamName == "name");
            }
        }

        [TestMethod]
        public void Test_HistorySave_NamedWithAndValueIsActionScaped_Save()
        {
            this.Compare(
                args: @"save 1 history-save --name \save",
                commandsTypes: GetCmds(
                    new Commands.T31.Command1(),
                    new Commands.T31.Command2(),
                    new Commands.T31.Command3()
                ),
                funcName: TestHelper.GetCurrentMethodName()

            );
        }

        [TestMethod]
        public void Test_HistorySave_NamedWithEqualsSeparatorAtTheEnd_Save()
        {
            this.Compare(
                args: "save 1 history-save --name=history1",
                commandsTypes: GetCmds(
                    new Commands.T31.Command1(),
                    new Commands.T31.Command2(),
                    new Commands.T31.Command3()
                ),
                funcName: TestHelper.GetCurrentMethodName()

            );
        }

        [TestMethod]
        public void Test_HistorySave_NamedWithEqualsSeparatorAndNextAction_Save()
        {
            this.Compare(
                args: "save 1 history-save --name=history1 save 1",
                commandsTypes: GetCmds(
                    new Commands.T31.Command1(),
                    new Commands.T31.Command2(),
                    new Commands.T31.Command3()
                ),
                funcName: TestHelper.GetCurrentMethodName()

            );
        }

        [TestMethod]
        public void Test_HistorySave_RepeatHistorySaveOftentimes_Save()
        {
            this.Compare(
                args: "history-save history0 save 1 history-save --name=history1 history-save history2 history-save  --name:history3 save 1 history-save --name:history4",
                commandsTypes: GetCmds(
                    new Commands.T31.Command1(),
                    new Commands.T31.Command2(),
                    new Commands.T31.Command3()
                ),
                funcName: TestHelper.GetCurrentMethodName()

            );
        }

        [TestMethod]
        public void Test_HistorySave_NamedAndNextAction_Save()
        {
            try
            {
                this.Compare(
                    args: "save 1 history-save --name save 1",
                    commandsTypes: GetCmds(
                        new Commands.T31.Command1(),
                        new Commands.T31.Command2(),
                        new Commands.T31.Command3()
                    ),
                    funcName: TestHelper.GetCurrentMethodName()

                );
                Assert.Fail();
            }
            catch (TargetInvocationException ex)
            {
                var ex2 = (ArgumentNullException)ex.InnerException;
                Assert.IsTrue(ex2.ParamName == "name");
            }
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
        public void Test_HistorySave_NoArgsAndRepeatHistorySave4Times_NoSave()
        {
            this.Compare(
                args: "history-save history1 history-save history2 history-save history3 history-save history4",
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
            try
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
            catch (ArgumentNullException ex)
            {
                Assert.IsTrue(ex.ParamName == "name");
            }
        }

        [TestMethod]
        public void Test_HistorySave_WithSaveAndLoad_SaveAndLoad()
        {
            var strWriter = new StringWriter();
            var commandsTypes = GetCmds(
                new Commands.T31.Command1(),
                new Commands.T31.Command2(),
                new Commands.T31.Command3()
            );

            try
            {
                var list = new List<TestData>();
                list.Add(this.GetTestData("--prop1 value history-save key1", commandsTypes));
                list.Add(this.GetTestData("--prop1 \"TWO \\\"quote\\\"\" save 1 save 2 history-save key2", commandsTypes));
                list.Add(this.GetTestData("--prop2 0.1099 history-save key1", commandsTypes));
                list.Add(this.GetTestData("history-list", commandsTypes));
                list.Add(this.GetTestData("save history-list", commandsTypes));
                list.Add(this.GetTestData("history-load key2", commandsTypes));
                list.Add(this.GetTestData("save 1 history-load key2", commandsTypes));
                list.Add(this.GetTestData("history-load key1 history-load key2", commandsTypes));
                list.Add(this.GetTestData("history-delete key2", commandsTypes));
                list.Add(this.GetTestData("history-list", commandsTypes));

                var result = TestHelper.CompareObjects<TestAppHistory>(list, null, TestHelper.GetCurrentMethodName());
                Assert.IsTrue(result);
            }
            catch
            {
                throw;
            }
            finally
            {
                var fileManager = new JsonFileManager();
                fileManager.SaveInRootFolderWhenIsDebug = false;
                fileManager.Remove(ArgsHistoryCommand.FILE_NAME);
            }
        }

        private Type[] GetCmds(params Command[] commands)
        {
            return commands.Select(f=> f.GetType()).ToArray();
        }
        
        private TestData GetTestData(string args, Type[] commandsTypes)
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

            return test;
        }

        private void Compare(string args, Type[] commandsTypes, string funcName, bool removeFile = true)
        {
            try
            {
                var test = this.GetTestData(args, commandsTypes);
                var result = TestHelper.CompareObjects<TestAppHistory>(test, null, funcName);
                Assert.IsTrue(result);
            }
            catch
            {
                throw;
            }
            finally
            {
                var fileManager = new JsonFileManager();
                fileManager.SaveInRootFolderWhenIsDebug = false;
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

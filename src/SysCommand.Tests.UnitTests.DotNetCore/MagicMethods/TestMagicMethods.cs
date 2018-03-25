using System;
using SysCommand.ConsoleApp;
using System.IO;
using SysCommand.ConsoleApp.Commands;
using Xunit;
using SysCommand.Tests.UnitTests.Common;

namespace SysCommand.Tests.UnitTests
{
    
    public class TestMagicMethods
    {
        public TestMagicMethods()
        {
            TestHelper.Setup();
        }

        [Fact]
        public void CallPropertyWithGetArgumentMethod()
        {
            this.CompareAll(
                 args: "--prop1 my-property-value",
                 commands: GetCmds(new Common.Commands.T40.Command1(ignoreApp: true)),
                 funcName: TestHelper.GetCurrentMethodName()
             );
        }

        [Fact]
        public void CallMethodWithGetAction()
        {
            this.CompareAll(
                 args: "test-action-map 1",
                 commands: GetCmds(new Common.Commands.T40.Command1(ignoreApp: true)),
                 funcName: TestHelper.GetCurrentMethodName()
             );
        }

        [Fact]
        public void CallMethodWithGetActionMap()
        {
            this.CompareAll(
                 args: "test-action 1",
                 commands: GetCmds(new Common.Commands.T40.Command1(ignoreApp:true)),
                 funcName: TestHelper.GetCurrentMethodName()
             );
        }

        private void CompareAll(string args, Command[] commands, string funcName)
        {
            CompareHelper.Compare<TestMagicMethods>(args, commands, funcName);
        }

        private void CompareOutput(string args, Command[] commands, string funcName)
        {
            var strWriter = new StringWriter();
            var testData = CompareHelper.GetTestData(args, commands, strWriter);
            var outputHeader = string.Join("\r\n", testData.Members);
            outputHeader += "\r\n-----------------------------\r\n";
            var output = outputHeader + strWriter.ToString();
            Assert.True(TestHelper.CompareObjects<TestApp>(output, null, funcName));
        }

        private Command[] GetCmds(params Command[] command)
        {
            return CompareHelper.GetCmds(command);
        }
    }
}

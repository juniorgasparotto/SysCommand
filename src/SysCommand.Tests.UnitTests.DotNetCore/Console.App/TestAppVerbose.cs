using SysCommand.ConsoleApp;
using SysCommand.ConsoleApp.Commands;
using SysCommand.Tests.UnitTests.Common;
using Xunit;

namespace SysCommand.Tests.UnitTests
{
    
    public class TestAppVerbose
    {
        public TestAppVerbose()
        {
            TestHelper.Setup();
        }

        [Fact]
        public void Test_Verbose_None()
        {
            this.CompareAll(
                args: "test-verbose -v none",
                commands: GetCmds(
                    new TestVerboseCommand(),
                    new VerboseCommand()
                ),
                funcName: TestHelper.GetCurrentMethodName()

            );
        }

        [Fact]
        public void Test_Verbose_NoneAsInteger()
        {
            this.CompareAll(
                args: "test-verbose --verbose 0",
                commands: GetCmds(
                    new TestVerboseCommand(),
                    new VerboseCommand()
                ),
                funcName: TestHelper.GetCurrentMethodName()

            );
        }

        [Fact]
        public void Test_Verbose_Quiet()
        {
            this.CompareAll(
                args: "test-verbose -v quiet",
                commands: GetCmds(
                    new TestVerboseCommand(),
                    new VerboseCommand()
                ),
                funcName: TestHelper.GetCurrentMethodName()

            );
        }

        [Fact]
        public void Test_Verbose_QuietAsInteger()
        {
            this.CompareAll(
                args: "test-verbose -v 64",
                commands: GetCmds(
                    new TestVerboseCommand(),
                    new VerboseCommand()
                ),
                funcName: TestHelper.GetCurrentMethodName()

            );
        }

        [Fact]
        public void Test_Verbose_All_Default()
        {
            this.CompareAll(
                args: "test-verbose",
                commands: GetCmds(
                    new TestVerboseCommand(),
                    new VerboseCommand()
                ),
                funcName: TestHelper.GetCurrentMethodName()
                    
            );
        }

        [Fact]
        public void Test_Verbose_AllWithInteger()
        {
            this.CompareAll(
                args: "test-verbose -v 1",
                commands: GetCmds(
                    new TestVerboseCommand(),
                    new VerboseCommand()
                ),
                funcName: TestHelper.GetCurrentMethodName()

            );
        }

        [Fact]
        public void Test_Verbose_AllWithLabel()
        {
            this.CompareAll(
                args: "test-verbose --verbose all",
                commands: GetCmds(
                    new TestVerboseCommand(),
                    new VerboseCommand()
                ),
                funcName: TestHelper.GetCurrentMethodName()

            );
        }

        [Fact]
        public void Test_Verbose_AllWithAllLabel()
        {
            this.CompareAll(
                args: "test-verbose --verbose Info 4 Critical Warning 32",
                commands: GetCmds(
                    new TestVerboseCommand(),
                    new VerboseCommand()
                ),
                funcName: TestHelper.GetCurrentMethodName()

            );
        }

        [Fact]
        public void Test_Verbose_InfoAndWarning()
        {
            this.CompareAll(
                args: "test-verbose -v Info warning",
                commands: GetCmds(
                    new TestVerboseCommand(),
                    new VerboseCommand()
                ),
                funcName: TestHelper.GetCurrentMethodName()

            );
        }

        [Fact]
        public void Test_Verbose_Info()
        {
            this.CompareAll(
                args: "test-verbose -v Info",
                commands: GetCmds(
                    new TestVerboseCommand(),
                    new VerboseCommand()
                ),
                funcName: TestHelper.GetCurrentMethodName()

            );
        }

        [Fact]
        public void Test_Verbose_Warning()
        {
            this.CompareAll(
                args: "test-verbose -v Warning",
                commands: GetCmds(
                    new TestVerboseCommand(),
                    new VerboseCommand()
                ),
                funcName: TestHelper.GetCurrentMethodName()

            );
        }

        [Fact]
        public void Test_Verbose_Success()
        {
            this.CompareAll(
                args: "test-verbose -v Success",
                commands: GetCmds(
                    new TestVerboseCommand(),
                    new VerboseCommand()
                ),
                funcName: TestHelper.GetCurrentMethodName()

            );
        }

        [Fact]
        public void Test_Verbose_ErrorAndLowerCase()
        {
            this.CompareAll(
                args: "test-verbose -v error",
                commands: GetCmds(
                    new TestVerboseCommand(),
                    new VerboseCommand()
                ),
                funcName: TestHelper.GetCurrentMethodName()

            );
        }

        [Fact]
        public void Test_Verbose_CriticalAndInteger()
        {
            this.CompareAll(
                args: "test-verbose -v 8",
                commands: GetCmds(
                    new TestVerboseCommand(),
                    new VerboseCommand()
                ),
                funcName: TestHelper.GetCurrentMethodName()
            );
        }

        private void CompareAll(string args, Command[] commands, string funcName)
        {
            CompareHelper.Compare<TestAppVerbose>(args, commands, funcName);
        }

        private Command[] GetCmds(params Command[] command)
        {
            return CompareHelper.GetCmds(command);
        }
    }
}

using SysCommand.Mapping;
using SysCommand.Parsing;
using System.Collections.Generic;
using SysCommand.ConsoleApp.Helpers;
using Xunit;
using SysCommand.Tests.UnitTests.Common;

namespace SysCommand.Tests.UnitTests
{
    public class TestAction
    {
        public TestAction()
        {
            TestHelper.Setup();
        }

        private void TestActionParsed(IEnumerable<ActionMap> actionMaps, string input, bool enableMultiAction, string testMethodName)
        {
            // get raw
            string testContext = null;
            string[] args;
            if (!string.IsNullOrWhiteSpace(input))
                args = ConsoleAppHelper.CommandLineToArgs(input);
            else
                args = new string[0];

            var argsRaw = CommandParserUtils.ParseArgumentsRaw(args, actionMaps);
            IEnumerable<ArgumentRaw> initialExtraArguments;
            var actionsMapped = CommandParserUtils.GetActionsParsed(argsRaw, enableMultiAction, actionMaps, out initialExtraArguments);
            var objectTest = new { input, actionMaps, actionsMapped };

            var jsonSerializeConfig = TestHelper.GetJsonConfig();
            Assert.True(TestHelper.CompareObjects<TestAction>(objectTest, testContext, testMethodName, jsonSerializeConfig));
        }

        [Fact]
        public void CallWithoutIgnoredsAndOnlyMethodsWithAttributes()
        {
            var actionMaps = CommandParserUtils.GetActionsMapsFromTargetObject(new Git(), onlyWithAttribute: true, usePrefixInAllMethods: false, prefix: "custom-prefix");
            this.TestActionParsed(actionMaps, "clean -a 1 -b 2", true, TestHelper.GetCurrentMethodName());
        }

        [Fact]
        public void CallWithoutIgnoredsAndPrefixedWithClassName()
        {
            var actionMaps = CommandParserUtils.GetActionsMapsFromTargetObject(new Git(), onlyWithAttribute: false, usePrefixInAllMethods: true, prefix: null);
            this.TestActionParsed(actionMaps, "git-clean", true, TestHelper.GetCurrentMethodName());
        }
        
        [Fact]
        public void CallWithoutIgnoredsAndCustomPrefix()
        {
            var actionMaps = CommandParserUtils.GetActionsMapsFromTargetObject(new Git(), onlyWithAttribute: false, usePrefixInAllMethods: true, prefix: "custom-prefix");
            this.TestActionParsed(actionMaps, "custom-prefix-main", true, TestHelper.GetCurrentMethodName());
        }

        [Fact]
        public void CallActionMainAndEmptyArguments()
        {
            var actionMaps = CommandParserUtils.GetActionsMapsFromTargetObject(new Git(), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionParsed(actionMaps, DefaultExecutor.Executor.MAIN_METHOD_NAME, true, TestHelper.GetCurrentMethodName());
        }

        [Fact]
        public void CallEmptyArguments()
        {
            var actionMaps = CommandParserUtils.GetActionsMapsFromTargetObject(new Git(), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionParsed(actionMaps, "", true, TestHelper.GetCurrentMethodName());
        }

        [Fact]
        public void CallWithoutActionAnd1NamedArgument()
        {
            var actionMaps = CommandParserUtils.GetActionsMapsFromTargetObject(new Git(), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionParsed(actionMaps, "--args a b c d clean 1 2 clean 1 2", true, TestHelper.GetCurrentMethodName());
        }

        [Fact]
        public void CallWithoutActionAnd1NamedArgumentAndMultiActionDisabled()
        {
            var actionMaps = CommandParserUtils.GetActionsMapsFromTargetObject(new Git(), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionParsed(actionMaps, "--args a b c d clean 1 2 clean 1 2", false, TestHelper.GetCurrentMethodName());
        }

        [Fact]
        public void CallWithoutActionAndAllArgsPositioned()
        {
            var actionMaps = CommandParserUtils.GetActionsMapsFromTargetObject(new Git(), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionParsed(actionMaps, @"\--args a b c d", true, TestHelper.GetCurrentMethodName());
        }

        [Fact]
        public void CallWithoutActionAndAllArgsPositionedAnd1ActionScaped()
        {
            var actionMaps = CommandParserUtils.GetActionsMapsFromTargetObject(new Git(), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionParsed(actionMaps, @"\main a b c d", true, TestHelper.GetCurrentMethodName());
        }

        [Fact]
        public void CallTwoActionsMainInSameCommand()
        {
            var actionMaps = CommandParserUtils.GetActionsMapsFromTargetObject(new Git(), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionParsed(actionMaps, "main --args a b c d main 123", true, TestHelper.GetCurrentMethodName());
        }

        [Fact]
        public void CallMainActionAnd1ActionScaped()
        {
            var actionMaps = CommandParserUtils.GetActionsMapsFromTargetObject(new Git(), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionParsed(actionMaps, @"main --args a b c d \\main 123", true, TestHelper.GetCurrentMethodName());
        }

        [Fact]
        public void CallWithoutActionAnd2ArgNamed()
        {
            var actionMaps = CommandParserUtils.GetActionsMapsFromTargetObject(new Git(), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionParsed(actionMaps, "-a 1 -b 2", true, TestHelper.GetCurrentMethodName());
        }

        [Fact]
        public void CallWithoutActionAnd2ArgsPositioned()
        {
            var actionMaps = CommandParserUtils.GetActionsMapsFromTargetObject(new Git(), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionParsed(actionMaps, "1 2", true, TestHelper.GetCurrentMethodName());
        }

        [Fact]
        public void CallActionMainThatIsIgnoredAction()
        {
            var actionMaps = CommandParserUtils.GetActionsMapsFromTargetObject(new Git(), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionParsed(actionMaps, @"main -a 1 -b 2", true, TestHelper.GetCurrentMethodName());
        }

        [Fact]
        public void CallTwoCallAndFirstIsArgsOfDefaulActionAndRestAreArgsOfMainAction()
        {
            var actionMaps = CommandParserUtils.GetActionsMapsFromTargetObject(new Git(), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionParsed(actionMaps, @"\main main 1 2", true, TestHelper.GetCurrentMethodName());
        }

        [Fact]
        public void CallMethodDefaultWithPositionedArgs()
        {
            var actionMaps = CommandParserUtils.GetActionsMapsFromTargetObject(new Git(), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionParsed(actionMaps, @"method-default -a value1", true, TestHelper.GetCurrentMethodName());
        }

        [Fact]
        public void CallWithoutActionsAnd1NamedArg()
        {
            var actionMaps = CommandParserUtils.GetActionsMapsFromTargetObject(new Git(), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionParsed(actionMaps, "-a value1", true, TestHelper.GetCurrentMethodName());
        }

        [Fact]
        public void CallWithoutActionsAnd1PositionedArg()
        {
            var actionMaps = CommandParserUtils.GetActionsMapsFromTargetObject(new Git(), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionParsed(actionMaps, "value1", true, TestHelper.GetCurrentMethodName());
        }

        [Fact]
        public void CallActionCleanMethodWith2ArgsPositioned()
        {
            var actionMaps = CommandParserUtils.GetActionsMapsFromTargetObject(new Git(), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionParsed(actionMaps, "clean 1 2", true, TestHelper.GetCurrentMethodName());
        }

        public void CallActionCleanScapedThenAllArgsPositioned()
        {
            var actionMaps = CommandParserUtils.GetActionsMapsFromTargetObject(new Git(), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionParsed(actionMaps, @"\clean 1 2", true, TestHelper.GetCurrentMethodName());
        }

        [Fact]
        public void CallActionAddWithAllArgsPositionedButThisActionHasPosicionalDisable()
        {
            var actionMaps = CommandParserUtils.GetActionsMapsFromTargetObject(new Git(), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionParsed(actionMaps, "add abc def", true, TestHelper.GetCurrentMethodName());
        }

        [Fact]
        public void CallActionAddWithNamedArgsAndCallActionClearInMiddle()
        {
            var actionMaps = CommandParserUtils.GetActionsMapsFromTargetObject(new Git(), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionParsed(actionMaps, @"add -a \add -b clean clean 1 2", true, TestHelper.GetCurrentMethodName());
        }

        [Fact]
        public void CallActionCommit()
        {
            var actionMaps = CommandParserUtils.GetActionsMapsFromTargetObject(new Git(), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionParsed(actionMaps, @"commit a b", true, TestHelper.GetCurrentMethodName());
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SysCommand.Parsing;
using SysCommand.Test;
using System.Collections.Generic;

namespace SysCommand.Tests.UnitTests
{
    [TestClass]
    public class TestAction
    {
        private void TestActionMapped(IEnumerable<ActionMap> actionMaps, string input, bool enableMultiAction, string testMethodName)
        {
            // get raw
            string testContext = null;
            string[] args;
            if (!string.IsNullOrWhiteSpace(input))
                args = AppHelpers.CommandLineToArgs(input);
            else
                args = new string[0];

            var argsRaw = ParserUtils.ParseArgumentsRaw(args, actionMaps);
            IEnumerable<ArgumentRaw> initialExtraArguments;
            var actionsMapped = ParserUtils.ParseActionMapped(argsRaw, enableMultiAction, actionMaps, out initialExtraArguments);
            var objectTest = new { input, actionMaps, actionsMapped };

            var jsonSerializeConfig = TestHelper.GetJsonConfig();
            jsonSerializeConfig.Converters.Add(new TestObjectJsonConverter());
            Assert.IsTrue(TestHelper.CompareObjects<TestAction>(objectTest, testContext, testMethodName, jsonSerializeConfig));
        }

        //public static List<string> GetActionMappedErrors(IEnumerable<ActionMapped> actionsMapped)
        //{
        //    var errors = new List<string>();
        //    if (actionsMapped.Count() == 0)
        //    {
        //        errors.Add("No action was found");
        //    }
        //    else
        //    {
        //        foreach (var action in actionsMapped)
        //        {
        //            foreach(var erro in CommandParser.ValidateArgumentsMapped(action.ArgumentsMapped))
        //            {
        //                errors.Add(string.Format("{0}: {1}", action.ToString(), erro.DefaultMessage));
        //            }
        //        }
        //    }
        //    return errors;
        //}

        //public static List<string> GetArgumentsMappedErrors(IEnumerable<CommandParser.ErrorArgumentMapped> argumentsMappedErrors)
        //{
        //    var errors = new List<string>();
        //    foreach (var erro in argumentsMappedErrors)
        //    {
        //        errors.Add(string.Format("{1}", erro.DefaultMessage));
        //    }
        //    return errors;
        //}

        [TestMethod]
        public void CallWithoutIgnoredsAndOnlyMethodsWithAttributes()
        {
            var actionMaps = ParserUtils.GetActionsMapsFromSourceObject(new Git(), onlyWithAttribute: true, usePrefixInAllMethods: false, prefix: "custom-prefix");
            this.TestActionMapped(actionMaps, "clean -a 1 -b 2", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallWithoutIgnoredsAndPrefixedWithClassName()
        {
            var actionMaps = ParserUtils.GetActionsMapsFromSourceObject(new Git(), onlyWithAttribute: false, usePrefixInAllMethods: true, prefix: null);
            this.TestActionMapped(actionMaps, "git-clean", true, TestHelper.GetCurrentMethodName());
        }
        
        [TestMethod]
        public void CallWithoutIgnoredsAndCustomPrefix()
        {
            var actionMaps = ParserUtils.GetActionsMapsFromSourceObject(new Git(), onlyWithAttribute: false, usePrefixInAllMethods: true, prefix: "custom-prefix");
            this.TestActionMapped(actionMaps, "custom-prefix-main", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallActionMainAndEmptyArguments()
        {
            var actionMaps = ParserUtils.GetActionsMapsFromSourceObject(new Git(), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, ParserUtils.MAIN_METHOD_NAME, true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallEmptyArguments()
        {
            var actionMaps = ParserUtils.GetActionsMapsFromSourceObject(new Git(), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, "", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallWithoutActionAnd1NamedArgument()
        {
            var actionMaps = ParserUtils.GetActionsMapsFromSourceObject(new Git(), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, "--args a b c d clean 1 2 clean 1 2", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallWithoutActionAnd1NamedArgumentAndMultiActionDisabled()
        {
            var actionMaps = ParserUtils.GetActionsMapsFromSourceObject(new Git(), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, "--args a b c d clean 1 2 clean 1 2", false, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallWithoutActionAndAllArgsPositioned()
        {
            var actionMaps = ParserUtils.GetActionsMapsFromSourceObject(new Git(), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, @"\--args a b c d", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallWithoutActionAndAllArgsPositionedAnd1ActionScaped()
        {
            var actionMaps = ParserUtils.GetActionsMapsFromSourceObject(new Git(), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, @"\main a b c d", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallTwoActionsMainInSameCommand()
        {
            var actionMaps = ParserUtils.GetActionsMapsFromSourceObject(new Git(), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, "main --args a b c d main 123", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallMainActionAnd1ActionScaped()
        {
            var actionMaps = ParserUtils.GetActionsMapsFromSourceObject(new Git(), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, @"main --args a b c d \\main 123", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallWithoutActionAnd2ArgNamed()
        {
            var actionMaps = ParserUtils.GetActionsMapsFromSourceObject(new Git(), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, "-a 1 -b 2", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallWithoutActionAnd2ArgsPositioned()
        {
            var actionMaps = ParserUtils.GetActionsMapsFromSourceObject(new Git(), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, "1 2", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallActionMainThatIsIgnoredAction()
        {
            var actionMaps = ParserUtils.GetActionsMapsFromSourceObject(new Git(), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, @"main -a 1 -b 2", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallTwoCallAndFirstIsArgsOfDefaulActionAndRestAreArgsOfMainAction()
        {
            var actionMaps = ParserUtils.GetActionsMapsFromSourceObject(new Git(), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, @"\main main 1 2", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallMethodDefaultWithPositionedArgs()
        {
            var actionMaps = ParserUtils.GetActionsMapsFromSourceObject(new Git(), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, @"method-default -a value1", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallWithoutActionsAnd1NamedArg()
        {
            var actionMaps = ParserUtils.GetActionsMapsFromSourceObject(new Git(), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, "-a value1", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallWithoutActionsAnd1PositionedArg()
        {
            var actionMaps = ParserUtils.GetActionsMapsFromSourceObject(new Git(), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, "value1", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallActionCleanMethodWith2ArgsPositioned()
        {
            var actionMaps = ParserUtils.GetActionsMapsFromSourceObject(new Git(), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, "clean 1 2", true, TestHelper.GetCurrentMethodName());
        }

        public void CallActionCleanScapedThenAllArgsPositioned()
        {
            var actionMaps = ParserUtils.GetActionsMapsFromSourceObject(new Git(), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, @"\clean 1 2", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallActionAddWithAllArgsPositionedButThisActionHasPosicionalDisable()
        {
            var actionMaps = ParserUtils.GetActionsMapsFromSourceObject(new Git(), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, "add abc def", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallActionAddWithNamedArgsAndCallActionClearInMiddle()
        {
            var actionMaps = ParserUtils.GetActionsMapsFromSourceObject(new Git(), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, @"add -a \add -b clean clean 1 2", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallActionCommit()
        {
            var actionMaps = ParserUtils.GetActionsMapsFromSourceObject(new Git(), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, @"commit a b", true, TestHelper.GetCurrentMethodName());
        }

        
    }
}

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using SysCommand.Tests;
using System.Collections.Generic;
using System.Reflection;
using System.Globalization;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using Newtonsoft.Json;
using TestUtils;

namespace SysCommand.UnitTests
{
    [TestClass]
    public class TestActionCaller
    {
        private void TestActionMapped(IEnumerable<ArgumentsParser.ActionMap> actionMaps, string input, bool enableMultiAction, string testMethodName)
        {
            string testContext = null;
            string[] args;
            if (!string.IsNullOrWhiteSpace(input))
                args = AppHelpers.CommandLineToArgs(input);
            else 
                args = new string[0];

            var argsRaw = ArgumentsParser.ConvertToArgumentRaw(args, actionMaps);
            var actionsCallers = ArgumentsParser.ConvertToActionCaller(argsRaw, enableMultiAction, actionMaps.ToArray());
            var objectTest = new { input, actionMaps, actionsCallers };

            TestHelper.CompareObjects<TestActionCaller>(objectTest, testContext, testMethodName);
        }

        [TestMethod]
        public void CallWithoutIgnoredsAndOnlyMethodsWithAttributes()
        {
            var actionMaps = ArgumentsParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: true, usePrefixInAllMethods: false, prefix: "custom-prefix");
            this.TestActionMapped(actionMaps, "clean -a 1 -b 2", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallWithoutIgnoredsAndPrefixedWithClassName()
        {
            var actionMaps = ArgumentsParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: true, prefix: null);
            this.TestActionMapped(actionMaps, "git-clean", true, TestHelper.GetCurrentMethodName());
        }
        
        [TestMethod]
        public void CallWithoutIgnoredsAndCustomPrefix()
        {
            var actionMaps = ArgumentsParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: true, prefix: "custom-prefix");
            this.TestActionMapped(actionMaps, "custom-prefix-main", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallActionMainAndEmptyArguments()
        {
            var actionMaps = ArgumentsParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, "main", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallEmptyArguments()
        {
            var actionMaps = ArgumentsParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, "", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallWithoutActionAnd1NamedArgument()
        {
            var actionMaps = ArgumentsParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, "--args a b c d clean 1 2 clean 1 2", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallWithoutActionAnd1NamedArgumentAndMultiActionDisabled()
        {
            var actionMaps = ArgumentsParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, "--args a b c d clean 1 2 clean 1 2", false, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallWithoutActionAndAllArgsPositioned()
        {
            var actionMaps = ArgumentsParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, @"\--args a b c d", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallWithoutActionAndAllArgsPositionedAnd1ActionScaped()
        {
            var actionMaps = ArgumentsParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, @"\main a b c d", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallTwoActionsMainInSameCommand()
        {
            var actionMaps = ArgumentsParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, "main --args a b c d main 123", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallMainActionAnd1ActionScaped()
        {
            var actionMaps = ArgumentsParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, @"main --args a b c d \\main 123", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallWithoutActionAnd2ArgNamed()
        {
            var actionMaps = ArgumentsParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, "-a 1 -b 2", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallWithoutActionAnd2ArgsPositioned()
        {
            var actionMaps = ArgumentsParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, "1 2", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallActionMainThatIsIgnoredAction()
        {
            var actionMaps = ArgumentsParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, @"main -a 1 -b 2", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallTwoCallAndFirstIsArgsOfDefaulActionAndRestAreArgsOfMainAction()
        {
            var actionMaps = ArgumentsParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, @"\main main 1 2", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallMethodDefaultWithPositionedArgs()
        {
            var actionMaps = ArgumentsParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, @"method-default -a value1", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallWithoutActionsAnd1NamedArg()
        {
            var actionMaps = ArgumentsParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, "-a value1", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallWithoutActionsAnd1PositionedArg()
        {
            var actionMaps = ArgumentsParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, "value1", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallActionCleanMethodWith2ArgsPositioned()
        {
            var actionMaps = ArgumentsParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, "clean 1 2", true, TestHelper.GetCurrentMethodName());
        }

        public void CallActionCleanScapedThenAllArgsPositioned()
        {
            var actionMaps = ArgumentsParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, @"\clean 1 2", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallActionAddWithAllArgsPositionedButThisActionHasPosicionalDisable()
        {
            var actionMaps = ArgumentsParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, "add abc def", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallActionAddWithNamedArgsAndCallActionClearInMiddle()
        {
            var actionMaps = ArgumentsParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, @"add -a \add -b clean clean 1 2", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallActionCommit()
        {
            var actionMaps = ArgumentsParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, @"commit a b", true, TestHelper.GetCurrentMethodName());
        }
    }
}

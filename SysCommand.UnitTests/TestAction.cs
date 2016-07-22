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

            var argsRaw = CommandParser.ParseArgumentRaw(args, actionMaps);

            var notFound = true;
            var instance = new Git();
            object actionsInvoke;
            object argumentsInvoke;

            //  mapping actions
            {
                var results = new List<string>();
                var errors = new List<string>();
                var actionsMapped = CommandParser.ParseActionMapped(argsRaw, enableMultiAction, actionMaps);
                var actionsMappedBestToInvoke = CommandParser.GetBestActionsMappedToInvoke(actionsMapped);

                if (actionsMappedBestToInvoke.Count() > 0)
                {
                    notFound = false;
                    var hasError = false;
                    foreach (var action in actionsMappedBestToInvoke)
                    {
                        foreach (var arg in action.ArgumentsMapped)
                        {
                            if (arg.MappingStates.HasFlag(ArgumentMappingState.IsInvalid))
                            {
                                hasError = true;
                                errors.Add(string.Format("{0}: {1}", action.ToString(), this.GetArgumentMappedErrorDescription(arg)));
                            }
                        }
                    }

                    if (!hasError)
                    {
                        foreach (var action in actionsMappedBestToInvoke)
                        {
                            var result = CommandParser.InvokeAction(instance, action);
                            results.Add(string.Format("{0}: {1}", action.ToString(), result));
                        }
                    }
                }

                var allErrors = new List<string>();

                foreach (var action in actionsMapped)
                {
                    foreach (var arg in action.ArgumentsMapped)
                    {
                        if (arg.MappingStates.HasFlag(ArgumentMappingState.IsInvalid))
                        {
                            allErrors.Add(string.Format("{0}: {1}", action.ToString(), this.GetArgumentMappedErrorDescription(arg)));
                        }
                    }
                }

                actionsInvoke = new
                {
                    ActionsInvoke = new
                    {
                        All = new
                        {
                            Names = actionsMapped.Select(f => f.ActionMap.Method.ToString()),
                            Errors = allErrors
                        },
                        Best = new
                        {
                            Names = actionsMappedBestToInvoke.Select(f => f.ActionMap.Method.ToString()),
                            Errors = errors,
                            Results = results
                        }
                    }
                };
            }

            // mapping arguments
            {
                var errors = new List<string>();
                var argumentsMap = CommandParser.GetArgumentsMapsFromProperties(typeof(Git), false);
                var argumentsMapped = CommandParser.ParseArgumentMapped(argsRaw, true, argumentsMap);

                var hasError = false;
                foreach (var arg in argumentsMapped)
                {
                    if (arg.MappingStates.HasFlag(ArgumentMappingState.IsInvalid))
                    {
                        hasError = true;
                        errors.Add(string.Format("globals: {0}", this.GetArgumentMappedErrorDescription(arg)));
                    }
                }

                if (!hasError)
                    CommandParser.InvokeArgumentsMappedAsProperties(instance, argumentsMapped);

                argumentsInvoke = new
                {
                    ArgumentsInvoke = new
                    {
                        Errors = errors,
                        Result = instance
                    }
                };
            }

            var objectTest = new { input, actionMaps, actionsMapped, invokeActionStep = actionStep2 };
            TestHelper.CompareObjects<TestAction>(objectTest, testContext, testMethodName);
        }

        public string GetArgumentMappedErrorDescription(ArgumentMapped argumentMapped)
        {
            if (argumentMapped.MappingStates.HasFlag(ArgumentMappingState.ArgumentAlreadyBeenSet))
                return string.Format("The argument '{0}' has already been set", argumentMapped.GetArgumentNameInputted());
            else if (argumentMapped.MappingStates.HasFlag(ArgumentMappingState.ArgumentNotExists))
                return string.Format("The argument '{0}' does not exist", argumentMapped.GetArgumentNameInputted());
            else if (argumentMapped.MappingStates.HasFlag(ArgumentMappingState.ValueWithoutArgument))
                return string.Format("Could not find an argument to the specified value: {0}", argumentMapped.Raw);
            else if (argumentMapped.MappingStates.HasFlag(ArgumentMappingState.ArgumentIsRequired))
                return string.Format("The argument '{0}' is required", argumentMapped.GetArgumentNameInputted());
            else if (argumentMapped.MappingStates.HasFlag(ArgumentMappingState.ArgumentIsInvalid))
                return string.Format("The argument '{0}' is invalid", argumentMapped.GetArgumentNameInputted());
            else if (argumentMapped.MappingStates.HasFlag(ArgumentMappingState.ArgumentIsUnsupported))
                return string.Format("The argument '{0}' is unsupported", argumentMapped.GetArgumentNameInputted());
            return null;
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
            var actionMaps = CommandParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: true, usePrefixInAllMethods: false, prefix: "custom-prefix");
            this.TestActionMapped(actionMaps, "clean -a 1 -b 2", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallWithoutIgnoredsAndPrefixedWithClassName()
        {
            var actionMaps = CommandParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: true, prefix: null);
            this.TestActionMapped(actionMaps, "git-clean", true, TestHelper.GetCurrentMethodName());
        }
        
        [TestMethod]
        public void CallWithoutIgnoredsAndCustomPrefix()
        {
            var actionMaps = CommandParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: true, prefix: "custom-prefix");
            this.TestActionMapped(actionMaps, "custom-prefix-main", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallActionMainAndEmptyArguments()
        {
            var actionMaps = CommandParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, "main", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallEmptyArguments()
        {
            var actionMaps = CommandParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, "", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallWithoutActionAnd1NamedArgument()
        {
            var actionMaps = CommandParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, "--args a b c d clean 1 2 clean 1 2", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallWithoutActionAnd1NamedArgumentAndMultiActionDisabled()
        {
            var actionMaps = CommandParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, "--args a b c d clean 1 2 clean 1 2", false, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallWithoutActionAndAllArgsPositioned()
        {
            var actionMaps = CommandParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, @"\--args a b c d", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallWithoutActionAndAllArgsPositionedAnd1ActionScaped()
        {
            var actionMaps = CommandParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, @"\main a b c d", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallTwoActionsMainInSameCommand()
        {
            var actionMaps = CommandParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, "main --args a b c d main 123", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallMainActionAnd1ActionScaped()
        {
            var actionMaps = CommandParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, @"main --args a b c d \\main 123", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallWithoutActionAnd2ArgNamed()
        {
            var actionMaps = CommandParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, "-a 1 -b 2", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallWithoutActionAnd2ArgsPositioned()
        {
            var actionMaps = CommandParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, "1 2", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallActionMainThatIsIgnoredAction()
        {
            var actionMaps = CommandParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, @"main -a 1 -b 2", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallTwoCallAndFirstIsArgsOfDefaulActionAndRestAreArgsOfMainAction()
        {
            var actionMaps = CommandParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, @"\main main 1 2", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallMethodDefaultWithPositionedArgs()
        {
            var actionMaps = CommandParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, @"method-default -a value1", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallWithoutActionsAnd1NamedArg()
        {
            var actionMaps = CommandParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, "-a value1", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallWithoutActionsAnd1PositionedArg()
        {
            var actionMaps = CommandParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, "value1", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallActionCleanMethodWith2ArgsPositioned()
        {
            var actionMaps = CommandParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, "clean 1 2", true, TestHelper.GetCurrentMethodName());
        }

        public void CallActionCleanScapedThenAllArgsPositioned()
        {
            var actionMaps = CommandParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, @"\clean 1 2", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallActionAddWithAllArgsPositionedButThisActionHasPosicionalDisable()
        {
            var actionMaps = CommandParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, "add abc def", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallActionAddWithNamedArgsAndCallActionClearInMiddle()
        {
            var actionMaps = CommandParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, @"add -a \add -b clean clean 1 2", true, TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void CallActionCommit()
        {
            var actionMaps = CommandParser.GetActionsMapsFromType(typeof(Git), onlyWithAttribute: false, usePrefixInAllMethods: false, prefix: null);
            this.TestActionMapped(actionMaps, @"commit a b", true, TestHelper.GetCurrentMethodName());
        }
    }
}

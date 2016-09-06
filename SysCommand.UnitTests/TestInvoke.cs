using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TestUtils;

namespace SysCommand.UnitTests
{
    [TestClass]
    public class TestInvoke
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
            IEnumerable<ActionMapped> actionsMapped;

            //  mapping actions
            {
                var results = new List<string>();
                var errors = new List<string>();
                actionsMapped = CommandParser.ParseActionMapped(argsRaw, enableMultiAction, actionMaps);
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
                var lstValids = new List<ArgumentMapped>();
                foreach (var arg in argumentsMapped)
                {
                    if (arg.MappingStates.HasFlag(ArgumentMappingState.IsInvalid))
                    {
                        hasError = true;
                        errors.Add(string.Format("globals: {0}", this.GetArgumentMappedErrorDescription(arg)));
                    }
                    else
                    {
                        lstValids.Add(arg);
                    }
                }

                //if (!hasError)
                CommandParser.InvokeArgumentsMappedAsProperties(instance, lstValids);

                argumentsInvoke = new
                {
                    ArgumentsInvoke = new
                    {
                        Errors = errors,
                        Result = instance
                    }
                };
            }

            var objectTest = new { input, actionMaps, actionsMapped, actionsInvoke, argumentsInvoke };
            TestHelper.CompareObjects<TestAction>(objectTest, testContext, testMethodName);
        }

        public string GetArgumentMappedErrorDescription(ArgumentMapped argumentMapped)
        {
            if (argumentMapped.MappingStates.HasFlag(ArgumentMappingState.ArgumentAlreadyBeenSet))
                return string.Format("The argument '{0}' has already been set", argumentMapped.GetArgumentNameInputted());
            else if (argumentMapped.MappingStates.HasFlag(ArgumentMappingState.ArgumentNotExistsByName))
                return string.Format("The argument '{0}' does not exist", argumentMapped.GetArgumentNameInputted());
            else if (argumentMapped.MappingStates.HasFlag(ArgumentMappingState.ArgumentNotExistsByValue))
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
    }
}

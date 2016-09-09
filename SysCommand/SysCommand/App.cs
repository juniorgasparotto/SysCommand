using System;
using System.Collections.Generic;
using System.Linq;

namespace SysCommand
{
    public class App2
    {
        public string[] Args { get; set; }
        public CommandMapCollection CommandsMaps { get; private set; }
        public CommandMappedCollection CommandsMappeds { get; private set; }
        public bool EnableMultiAction { get; set; }

        public App2(IEnumerable<Command> commands)
        {
            this.CommandsMaps = new CommandMapCollection(commands);
        }

        public void Run()
        {
            this.Run(this.GetOrRead());
        }

        public void Run(string[] args)
        {
            this.Args = args;
            this.CommandsMappeds = new CommandMappedCollection(this.Args, this.CommandsMaps, this.EnableMultiAction);

            var notFound = true;
            var errors = new List<string>();
            var actionsMappedBestToInvoke = CommandParser.GetBestActionsMappedToInvoke(this.CommandsMappeds.ActionsMappeds);

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
                        //var result = CommandParser.InvokeAction(this.CommandsMappeds[action.ActionMap.ParentClassType], action);
                        //results.Add(string.Format("{0}: {1}", action.ToString(), result));
                    }
                }
            }
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

        private string[] GetOrRead()
        {
            //if (this.Args != null)
            //    return this.Args;

            if (Debug.IsInDebug && Debug.WhenIsInDebugReadArgs)
            {
                Console.WriteLine("Enter with args:");
                return StringToArgs(Console.ReadLine());
            }
            else
            {
                var args = Environment.GetCommandLineArgs();
                var listArgs = args.ToList();
                // remove the app path that added auto by .net
                listArgs.RemoveAt(0);
                return listArgs.ToArray();
            }
        }

        public string[] StringToArgs(string args)
        {
            if (!string.IsNullOrWhiteSpace(args))
                return AppHelpers.CommandLineToArgs(args);
            else
                return new string[0];
        }
    }
}

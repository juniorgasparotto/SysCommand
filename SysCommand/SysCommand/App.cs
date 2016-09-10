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

            var errors = new List<string>();
            var hasErrorInArguments = false;
            var notFoundArguments = true;

            //if (!this.CommandsMappeds.GetAllArgumentsMappeds().All(f => f.IsMapped))
            //    notFoundArguments = false;

            foreach (var commandMapped in this.CommandsMappeds)
            {
                //foreach (var argumentMapped in commandMapped.ArgumentsMappeds)
                //{

                //    //if (argumentMapped.IsMapped)
                //    //{
                        
                //    //}

                //    //if (argumentMapped.MappingStates.HasFlag(ArgumentMappingState.IsInvalid))
                //    //{
                        
                //    //    hasErrorInArguments = true;
                //    //    errors.Add(string.Format("{0}: {1}", argumentMapped.ToString(), this.GetArgumentMappedErrorDescription(argumentMapped)));
                //    //}
                //}

                var lstValids = commandMapped.ArgumentsMappeds.Where(f => !f.MappingStates.HasFlag(ArgumentMappingState.IsInvalid)).ToList();
                if (lstValids.Count > 0)
                {
                    notFoundArguments = false;
                    CommandParser.InvokeSourcePropertiesFromArgumentsMappeds(lstValids);
                    //commandMapped.ActionsMappeds.Where(f => f.ActionMap.Method.Name.ToLower() == "main");
                    commandMapped.Command.Main();
                }
            }

            var notFoundActions = true;
            var actionsMappedBestOrAllToInvoke = CommandParser.GetBestActionsMappedOrAll(this.CommandsMappeds.GetAllActionsMappeds());

            if (actionsMappedBestOrAllToInvoke.Count() > 0)
            {
                notFoundActions = false;
                var hasError = actionsMappedBestOrAllToInvoke.Any(f => f.MappingStates.HasFlag(ActionMappingState.IsInvalid));
                
                //foreach (var action in actionsMappedBestOrAllToInvoke)
                //{
                //    foreach (var arg in action.ArgumentsMapped)
                //    {
                //        if (arg.MappingStates.HasFlag(ArgumentMappingState.IsInvalid))
                //        {
                //            hasError = true;
                //            errors.Add(string.Format("{0}: {1}", action.ToString(), this.GetArgumentMappedErrorDescription(arg)));
                //        }
                //    }
                //}

                // execute only if all actions is valids
                if (!hasError)
                {
                    foreach (var action in actionsMappedBestOrAllToInvoke)
                        CommandParser.InvokeSourceMethodsFromActionsMappeds(action);
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

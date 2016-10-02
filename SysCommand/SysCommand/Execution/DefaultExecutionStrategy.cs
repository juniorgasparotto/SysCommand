using System.Collections.Generic;
using System.Linq;

namespace SysCommand
{
    public class DefaultExecutionStrategy : IExecutionStrategy
    {
        public virtual Map CreateMap(IEnumerable<Command> commands)
        {
            var maps = new List<MapCommand>();

            foreach (var command in commands)
            {
                var mapCommand = new MapCommand(command);
                mapCommand.Methods.AddRange(CommandParser.GetActionsMapsFromSourceObject(command, command.OnlyMethodsWithAttribute, command.UsePrefixInAllMethods, command.PrefixMethods));
                mapCommand.Properties.AddRange(CommandParser.GetArgumentsMapsFromProperties(command, command.OnlyPropertiesWithAttribute));
                maps.Add(mapCommand);
            }

            return new Map(maps);
        }

        public virtual Result<IMember> Parse(string[] args, Map map, bool enableMultiAction)
        {
            var result = new Result<IMember>();
            var actionsMaps = map.GetAllActionsMaps();
            var argumentsRaw = CommandParser.ParseArgumentRaw(args, actionsMaps);

            foreach (var item in map)
            {
                var argsMapped = CommandParser.ParseArgumentMapped(argumentsRaw, item.Command.EnablePositionalArgs, map[item.Command].Properties);
                var actions = CommandParser.ParseActionMapped(argumentsRaw, enableMultiAction, actionsMaps);
                result.AddRange(argsMapped.Select(f => new Property(f/*, PriorityConstants.PriorityUser*/)));
                result.AddRange(actions.Select(f => new Method(f/*, PriorityConstants.PriorityUser*/)));
            }

            return result;
        }

        public ExecutionState Execute(string[] args, Map map, Result<IMember> result)
        {
            // step1: create main methods
            this.CreateMainMethods(map, result);

            // step1: invoke all properties
            result
                .With<Property>(f => f.ArgumentMapped.MappingStates.HasFlag(ArgumentMappingState.Valid))
                .Invoke();

            // step2: invoke all main methods
            result
                .With<MethodMain>()
                .Invoke();

            // step2: validate if exists errors
            return this.Validate(args, result);
        }

        private ExecutionState Validate(string[] args, Result<IMember> result)
        {
            if (args.Empty())
                return ExecutionState.ArgsIsEmpty;

            // methods - exclude 'main' methods
            var actions = result.With<Method>(/*f => f.InvokePriority == PriorityConstants.PriorityUser*/);
            var actionsValids = actions.With(f => f.ActionMapped.MappingStates.HasFlag(ActionMappingState.Valid));
            var actionsInvalids = actions.With(f => f.ActionMapped.MappingStates.HasFlag(ActionMappingState.IsInvalid));

            // properties
            var arguments = result.With<Property>();
            var argumentsValids = arguments.With(f => f.ArgumentMapped.MappingStates.HasFlag(ArgumentMappingState.Valid));
            var argumentsInvalids = arguments.With(f => f.ArgumentMapped.MappingStates.HasFlag(ArgumentMappingState.IsInvalid));

            // check errors
            var notFoundActions = actions.Empty();
            var notFoundArguments = arguments.Empty() || argumentsInvalids.All(f => f.ArgumentMapped.IsMapped == false);
            var existsActionsError = actionsValids.Empty() && actionsInvalids.Any();
            //var existsArgumentsError = !notFoundArguments && lstPropertiesInvokers.Count == 0 && lstInvalidsArguments.Count > 0;

            if (notFoundActions && notFoundArguments)
                return ExecutionState.NotFound;
            else if (existsActionsError)
                return ExecutionState.HasInvalidAction;

            return ExecutionState.Success;
        }

        private void CreateMainMethods(Map map, Result<IMember> result)
        {
            foreach (var command in map.GetAllCommands())
            {
                var mainMethod = command.GetType().GetMethods().Where(f => f.Name.ToLower() == CommandParser.MAIN_METHOD_NAME && f.GetParameters().Length == 0).FirstOrDefault();
                if (mainMethod != null)
                    result.Add(new MethodMain(mainMethod.Name, command, mainMethod));
            }
        }
    }
}
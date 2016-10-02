using SysCommand.Parser;
using System.Collections.Generic;
using System.Linq;

namespace SysCommand
{
    public class DefaultExecutionStrategy : IExecutionStrategy
    {
        public virtual Map CreateMap(IEnumerable<Command> commands)
        {
            var maps = new List<MapItem>();

            foreach (var command in commands)
            {
                var mapCommand = new MapItem(command);
                mapCommand.Methods.AddRange(CommandParser.GetActionsMapsFromSourceObject(command, command.OnlyMethodsWithAttribute, command.UsePrefixInAllMethods, command.PrefixMethods));
                mapCommand.Properties.AddRange(CommandParser.GetArgumentsMapsFromProperties(command, command.OnlyPropertiesWithAttribute));
                maps.Add(mapCommand);
            }

            return new Map(maps);
        }

        public virtual Result<IMember> Parse(string[] args, Map map, bool enableMultiAction)
        {
            var result = new Result<IMember>();
            var actionsMaps = map.GetMethods();
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

            // step1: invoke the properties that are valid
            result
                .With<Property>(f => f.ArgumentMapped.MappingStates.HasFlag(ArgumentMappingState.Valid))
                .Invoke();

            // step2: invoke all main methods
            result
                .With<MethodMain>()
                .Invoke();

            // step3: get state and execute other methods if success
            var state = this.GetExecutionState(args, result);
            if (state == ExecutionState.Success)
                this.GetValidMethods(result.With<Method>()).Invoke();

            return state;
        }

        private ExecutionState GetExecutionState(string[] args, Result<IMember> result)
        {
            // methods
            var methods = result.With<Method>();
            var methodsValids = this.GetValidMethods(methods);
            var methodsInvalids = this.GetInvalidMethods(methods);

            // properties
            var properties = result.With<Property>();
            var propertiesValids = this.GetValidProperties(properties);
            var propertiesInvalids = this.GetInvalidProperties(properties);

            // check errors
            var notFoundActions = methods.Empty();
            var notFoundArguments = properties.Empty() || propertiesInvalids.All(f => f.ArgumentMapped.IsMapped == false);
            var existsActionsError = methodsValids.Empty() && propertiesInvalids.Any();
            //var existsArgumentsError = !notFoundArguments && lstPropertiesInvokers.Count == 0 && lstInvalidsArguments.Count > 0;

            if (notFoundActions && notFoundArguments)
                return ExecutionState.NotFound;
            else if (existsActionsError)
                return ExecutionState.HasInvalidAction;

            return ExecutionState.Success;
        }

        private Result<Method> GetValidMethods(Result<Method> result)
        {
            return result.With(f => f.ActionMapped.MappingStates.HasFlag(ActionMappingState.Valid));
        }

        private Result<Method> GetInvalidMethods(Result<Method> result)
        {
            return result.With(f => f.ActionMapped.MappingStates.HasFlag(ActionMappingState.IsInvalid));
        }

        private Result<Property> GetValidProperties(Result<Property> result)
        {
            return result.With(f => f.ArgumentMapped.MappingStates.HasFlag(ActionMappingState.Valid));
        }

        private Result<Property> GetInvalidProperties(Result<Property> result)
        {
            return result.With(f => f.ArgumentMapped.MappingStates.HasFlag(ActionMappingState.IsInvalid));
        }

        private void CreateMainMethods(Map map, Result<IMember> result)
        {
            foreach (var command in map.GetCommands())
            {
                var mainMethod = command.GetType().GetMethods().Where(f => f.Name.ToLower() == CommandParser.MAIN_METHOD_NAME && f.GetParameters().Length == 0).FirstOrDefault();
                if (mainMethod != null)
                    result.Add(new MethodMain(mainMethod.Name, command, mainMethod));
            }
        }
    }
}
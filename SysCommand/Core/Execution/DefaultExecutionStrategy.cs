using SysCommand.Parser;
using System.Collections.Generic;
using System.Linq;

namespace SysCommand
{
    public class DefaultExecutionStrategy : IExecutionStrategy
    {
        public virtual Result<IMember> Parse(string[] args, IEnumerable<CommandMap> maps, bool enableMultiAction)
        {
            var result = new Result<IMember>();
            var methods = maps.GetMethods();
            var properties = CommandParser.ParseArgumentRaw(args, methods);

            foreach (var item in maps)
            {
                var propertiesParsed = CommandParser.ParseArgumentMapped(properties, item.Command.EnablePositionalArgs, item.Properties);
                var methodsParsed = CommandParser.ParseActionMapped(properties, enableMultiAction, methods);
                result.AddRange(propertiesParsed.Select(f => new Property(f)));
                result.AddRange(methodsParsed.Select(f => new Method(f)));
            }

            return result;
        }

        public ExecutionState Execute(string[] args, IEnumerable<CommandMap> maps, Result<IMember> result)
        {
            // step1: create main methods
            this.CreateMainMethods(maps, result);

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
            return result.With(f => f.ArgumentMapped.MappingStates.HasFlag(ArgumentMappingState.Valid));
        }

        private Result<Property> GetInvalidProperties(Result<Property> result)
        {
            return result.With(f => f.ArgumentMapped.MappingStates.HasFlag(ArgumentMappingState.IsInvalid));
        }

        private void CreateMainMethods(IEnumerable<CommandMap> maps, Result<IMember> result)
        {
            foreach (var map in maps)
            {
                var mainMethod = map.Command.GetType().GetMethods().Where(f => f.Name.ToLower() == CommandParser.MAIN_METHOD_NAME && f.GetParameters().Length == 0).FirstOrDefault();
                if (mainMethod != null)
                    result.Add(new MethodMain(mainMethod.Name, map.Command, mainMethod));
            }
        }
    }
}
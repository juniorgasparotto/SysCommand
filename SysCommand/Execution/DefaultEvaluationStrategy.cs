using SysCommand.Parser;
using System.Collections.Generic;
using System.Linq;
using System;

namespace SysCommand
{
    public class DefaultEvaluationStrategy : IEvaluationStrategy
    {
        public Action<IMember> OnInvoke { get; set; }

        public virtual Result<IMember> Parse(string[] args, IEnumerable<CommandMap> maps, bool enableMultiAction)
        {
            var result = new Result<IMember>();
            var methods = maps.GetMethods();
            var argumentsRaw = CommandParser.ParseArgumentsRaw(args, methods);

            foreach (var item in maps)
            {
                var propertiesParsed = CommandParser.ParseArgumentMapped(argumentsRaw, item.Command.EnablePositionalArgs, item.Properties);
                var methodsParsed = CommandParser.ParseActionMapped(argumentsRaw, enableMultiAction, item.Methods);
                result.AddRange(propertiesParsed.Select(f => new Property(f)));
                result.AddRange(methodsParsed.Select(f => new Method(f)));
            }
                        
            return result;
        }

        public EvaluateState Evaluate(string[] args, IEnumerable<CommandMap> maps, Result<IMember> result)
        {
            // step1: get valid properties, methods and respective commands
            var properties = result.With<Property>();
            var propertiesValid = properties.WithValidProperties();
            
            var methods = result.With<Method>();
            var methodsValid = methods.WithValidMethods();

            var commandsValid = new List<object>();
            var commandsValidForProperties = propertiesValid.Select(f => f.ArgumentMapped.Map.Source);
            var commandsValidForMethods = methodsValid.Select(f => f.ActionMapped.ActionMap.Source);

            foreach(var cmd in commandsValidForProperties)
            { 
                if (!commandsValid.Contains(cmd))
                    commandsValid.Add(cmd);
            }

            foreach (var cmd in commandsValidForMethods)
            {
                if (!commandsValid.Contains(cmd))
                    commandsValid.Add(cmd);
            }

            // step2: invoke valid properties
            if (propertiesValid.Any())
                propertiesValid.Invoke(this.OnInvoke);

            // step3: invoke main method if exists any valid member
            var hasValid = propertiesValid.Any() || methodsValid.Any();
            if (hasValid)
            {
                var mainMethods = this.CreateMainMethods(commandsValid);
                result.AddRange(mainMethods);
                result.With<MethodMain>().Invoke(this.OnInvoke);
            }

            // step4: execute valid user methods and remove duplicites
            if (methodsValid.Any())
                methodsValid.TrimDuplicate().Invoke(this.OnInvoke);

            var notFoundActions = methods.Empty();
            var notFoundArguments = properties.Empty() || properties.All(f => f.ArgumentMapped.IsMapped == false);
            var existsActionsError = methodsValid.Empty() && methods.Any();

            if (notFoundActions && notFoundArguments)
                return EvaluateState.NotFound;
            else if (existsActionsError)
                return EvaluateState.HasInvalidMethod;

            return EvaluateState.Success;
        }

        

        //private Result<Property> GetInvalidProperties(Result<Property> result)
        //{
        //    return result.With(f => f.ArgumentMapped.MappingStates.HasFlag(ArgumentMappingState.IsInvalid));
        //}

        private IEnumerable<MethodMain> CreateMainMethods(IEnumerable<object> objs)
        {
            foreach (var obj in objs)
            {
                var mainMethod = obj.GetType().GetMethods().Where(f => f.Name.ToLower() == CommandParser.MAIN_METHOD_NAME && f.GetParameters().Length == 0).FirstOrDefault();
                if (mainMethod != null)
                    yield return new MethodMain(mainMethod.Name, obj, mainMethod);
            }
        }
    }
}
using SysCommand.Parser;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SysCommand.ConsoleApp
{
    public class DefaultMessageFormatter : IMessageFormatter
    {
        public virtual void ShowErrors(ApplicationResult appResult)
        {
            var strBuilder = this.GetErrors(appResult.EvaluateResult.Errors);
            appResult.App.Console.Write(strBuilder);
        }

        public virtual void ShowNotFound(ApplicationResult appResult)
        {
            appResult.App.Console.Error(Strings.NotFoundMessage, false);
        }

        public virtual string GetMethodSpecification(ActionMap map)
        {
            var format = "{0}({1})";
            string args = null;
            foreach (var arg in map.ArgumentsMaps)
            {
                var typeName = AppHelpers.CSharpName(arg.Type);
                args += args == null ? typeName : ", " + typeName;
            }
            return string.Format(format, map.ActionName, args);
        }

        public virtual string GetPropertyErrorDescription(ArgumentMapped argumentMapped)
        {
            if (argumentMapped.MappingStates.HasFlag(ArgumentMappingState.ArgumentAlreadyBeenSet))
                return string.Format("The argument '{0}' has already been set", argumentMapped.GetArgumentNameInputted());
            else if (argumentMapped.MappingStates.HasFlag(ArgumentMappingState.ArgumentNotExistsByName))
                return string.Format("The argument '{0}' does not exist", argumentMapped.GetArgumentNameInputted());
            else if (argumentMapped.MappingStates.HasFlag(ArgumentMappingState.ArgumentNotExistsByValue))
                return string.Format("Could not find an argument to the specified value: {0}", argumentMapped.Raw);
            else if (argumentMapped.MappingStates.HasFlag(ArgumentMappingState.ArgumentIsRequired))
                return string.Format("The argument '{0}' is required", argumentMapped.GetArgumentNameInputted());
            else if (argumentMapped.MappingStates.HasFlag(ArgumentMappingState.ArgumentHasInvalidInput))
                return string.Format("The argument '{0}' is invalid", argumentMapped.GetArgumentNameInputted());
            else if (argumentMapped.MappingStates.HasFlag(ArgumentMappingState.ArgumentHasUnsupportedType))
                return string.Format("The argument '{0}' is unsupported", argumentMapped.GetArgumentNameInputted());
            return null;
        }

        private StringBuilder GetErrors(IEnumerable<CommandError> commandsErrors)
        {
            var strBuilder = new StringBuilder();
            var count = commandsErrors.Count();
            var iErr = 0;
            foreach (var commandError in commandsErrors)
            {
                strBuilder.AppendLine(string.Format("There are errors in command: {0}", commandError.Command.GetType().Name));
                var hasPropertyError = commandError.Properties.Any();
                var hasMethodError = commandError.Methods.Any();

                if (hasPropertyError)
                {
                    this.ShowInvalidProperties(strBuilder, commandError.Properties);
                }

                if (hasMethodError)
                { 
                    if (hasPropertyError)
                        strBuilder.AppendLine();

                    this.ShowInvalidMethods(strBuilder, commandError.Methods);
                }

                if (++iErr < count)
                    strBuilder.AppendLine("\r\n");
            }

            return strBuilder;
        }

        private void ShowInvalidMethods(StringBuilder strBuilder, IEnumerable<ActionMapped> methodsInvalid)
        {
            var iErr = 0;
            var count = methodsInvalid.Count();

            foreach (var invalid in methodsInvalid)
            {
                strBuilder.AppendLine(string.Format("Error in method: {0}", GetMethodSpecification(invalid.ActionMap)));
                this.ShowInvalidProperties(strBuilder, invalid.Arguments.Where(f=>f.MappingStates.HasFlag(ArgumentMappingState.IsInvalid)));
                if (++iErr < count)
                    strBuilder.AppendLine("\r\n");
            }
        }

        private void ShowInvalidProperties(StringBuilder strBuilder, IEnumerable<ArgumentMapped> properties)
        {
            var iErr = 0;
            var count = properties.Count();

            foreach (var arg in properties)
            {
                var argErro = GetPropertyErrorDescription(arg);
                strBuilder.Append(string.Format("{0}", argErro));
                if (++iErr < count)
                    strBuilder.AppendLine();
            }
        }        
    }
}
using SysCommand.Parsing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SysCommand.Execution;
using SysCommand.Mapping;
using SysCommand.Helpers;

namespace SysCommand.ConsoleApp
{
    public class DefaultDescriptor : IDescriptor
    {
        public virtual void ShowErrors(ApplicationResult appResult)
        {
            var strBuilder = this.GetErrors(appResult.ExecutionResult.Errors);
            appResult.App.Console.Write(strBuilder);
        }

        public virtual void ShowNotFound(ApplicationResult appResult)
        {
            appResult.App.Console.Error(Strings.NotFoundMessage, false);
        }

        public virtual void ShowMethodReturn(ApplicationResult appResult, IMemberResult method, object value)
        {
            appResult.App.Console.Write(method.Value);
        }

        public virtual string GetMethodSpecification(ActionMap map)
        {
            var format = "{0}({1})";
            string args = null;
            foreach (var arg in map.ArgumentsMaps)
            {
                var typeName = ReflectionHelper.CSharpName(arg.Type);
                args += args == null ? typeName : ", " + typeName;
            }
            return string.Format(format, map.ActionName, args);
        }

        public virtual string GetPropertyErrorDescription(ArgumentParsed argumentParsed)
        {
            if (argumentParsed.ParsingStates.HasFlag(ArgumentParsedState.ArgumentAlreadyBeenSet))
                return string.Format(Strings.ArgumentAlreadyBeenSet, argumentParsed.GetArgumentNameInputted());
            else if (argumentParsed.ParsingStates.HasFlag(ArgumentParsedState.ArgumentNotExistsByName))
                return string.Format(Strings.ArgumentNotExistsByName, argumentParsed.GetArgumentNameInputted());
            else if (argumentParsed.ParsingStates.HasFlag(ArgumentParsedState.ArgumentNotExistsByValue))
                return string.Format(Strings.ArgumentNotExistsByValue, argumentParsed.Raw);
            else if (argumentParsed.ParsingStates.HasFlag(ArgumentParsedState.ArgumentIsRequired))
                return string.Format(Strings.ArgumentIsRequired, argumentParsed.GetArgumentNameInputted());
            else if (argumentParsed.ParsingStates.HasFlag(ArgumentParsedState.ArgumentHasInvalidInput))
                return string.Format(Strings.ArgumentHasInvalidInput, argumentParsed.GetArgumentNameInputted());
            else if (argumentParsed.ParsingStates.HasFlag(ArgumentParsedState.ArgumentHasUnsupportedType))
                return string.Format(Strings.ArgumentHasUnsupportedType, argumentParsed.GetArgumentNameInputted());
            return null;
        }

        private StringBuilder GetErrors(IEnumerable<ExecutionError> commandsErrors)
        {
            var strBuilder = new StringBuilder();
            var count = commandsErrors.Count();
            var iErr = 0;
            foreach (var commandError in commandsErrors)
            {
                strBuilder.AppendLine(string.Format("There are errors in command: {0}", commandError.Command.GetType().Name));
                var hasPropertyError = commandError.PropertiesInvalid.Any();
                var hasMethodError = commandError.MethodsInvalid.Any();

                if (hasPropertyError)
                {
                    this.ShowInvalidProperties(strBuilder, commandError.PropertiesInvalid);
                }

                if (hasMethodError)
                { 
                    if (hasPropertyError)
                        strBuilder.AppendLine();

                    this.ShowInvalidMethods(strBuilder, commandError.MethodsInvalid);
                }

                if (++iErr < count)
                    strBuilder.AppendLine("\r\n");
            }

            return strBuilder;
        }

        private void ShowInvalidMethods(StringBuilder strBuilder, IEnumerable<ActionParsed> methodsInvalid)
        {
            var iErr = 0;
            var count = methodsInvalid.Count();

            foreach (var invalid in methodsInvalid)
            {
                strBuilder.AppendLine(string.Format("Error in method: {0}", GetMethodSpecification(invalid.ActionMap)));
                this.ShowInvalidProperties(strBuilder, invalid.Arguments.Where(f=>f.ParsingStates.HasFlag(ArgumentParsedState.IsInvalid)));
                if (++iErr < count)
                    strBuilder.AppendLine("\r\n");
            }
        }

        private void ShowInvalidProperties(StringBuilder strBuilder, IEnumerable<ArgumentParsed> properties)
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
using SysCommand.Parsing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SysCommand.Execution;
using SysCommand.Mapping;
using SysCommand.Helpers;
using System;

namespace SysCommand.ConsoleApp
{
    public class DefaultDescriptor : IDescriptor
    {
        private const int PADDING_SPACE_PROPERTIES = 4;
        private const int PADDING_SPACE_METHOD = 4;
        private const int PADDING_SPACE_METHOD_PARAMS = 8;

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

        public string GetArgumentNameWithPrefix(ArgumentMap arg)
        {
            string key = null;
            var shortName = arg.ShortName != null ? arg.ShortName.ToString() : null;
            if (!string.IsNullOrWhiteSpace(shortName) && !string.IsNullOrWhiteSpace(arg.LongName))
                key = "-" + arg.ShortName + ", --" + arg.LongName;
            else if (!string.IsNullOrWhiteSpace(shortName))
                key = "-" + arg.ShortName;
            else if (!string.IsNullOrWhiteSpace(arg.LongName))
                key = "--" + arg.LongName;

            return key;
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

        public string GetHelpText(IEnumerable<CommandMap> commandMaps)
        {
            var strBuilder = new StringBuilder();
            var last = commandMaps.LastOrDefault();
            foreach (var cmd in commandMaps)
            {
                strBuilder.Append(this.GetHelpText(cmd));
                if (cmd != last)
                { 
                    strBuilder.AppendLine();
                    strBuilder.AppendLine();
                }
            }
            return strBuilder.ToString();
        }

        public string GetHelpText(CommandMap commandMap)
        {
            var strBuilder = new StringBuilder();
            var commandDesc = commandMap.Command.GetType().Name + ":";

            var hasProperties = commandMap.Properties.Any();
            var hasMethods = commandMap.Methods.Any();
            if (hasProperties || hasMethods)
            {
                strBuilder.Append(commandDesc);
                strBuilder.AppendLine();
                if (hasProperties)
                { 
                    strBuilder.Append(this.GetHelpText(commandMap.Properties, PADDING_SPACE_PROPERTIES));
                    if (hasMethods)
                        strBuilder.AppendLine();
                }

                if (hasMethods)
                    strBuilder.Append(this.GetHelpText(commandMap.Methods));
            }
            else
            {
                strBuilder.Append(commandDesc);
            }

            return strBuilder.ToString();
        }

        public string GetHelpText(IEnumerable<ActionMap> actionsMap)
        {
            var strBuilder = new StringBuilder();
            var last = actionsMap.Last();
            foreach (var action in actionsMap)
            { 
                strBuilder.Append(this.GetHelpText(action, PADDING_SPACE_METHOD, PADDING_SPACE_METHOD_PARAMS));
                if (action != last)
                    strBuilder.AppendLine();
            }
            return strBuilder.ToString();
        }

        public string GetHelpText(ActionMap actionMap, int padding, int paddingParams)
        {
            var strBuilder = new StringBuilder();
            var defaultHelp = actionMap.IsDefault ? " *" : "";
            var actionDesc = string.Format("{0}{1}", actionMap.ActionName, defaultHelp);
            actionDesc = actionDesc.PadLeft(actionDesc.Length + padding, ' ');

            strBuilder.Append(actionDesc);
            if (actionMap.ArgumentsMaps.Any())
            { 
                strBuilder.AppendLine();
                strBuilder.Append(this.GetHelpText(actionMap.ArgumentsMaps, paddingParams));
            }

            return strBuilder.ToString();
        }

        public string GetHelpText(IEnumerable<ArgumentMap> argumentMap, int padding)
        {
            var dicProperty = new Dictionary<string, string>();
            foreach (var parameter in argumentMap)
            {
                var key = this.GetArgumentNameWithPrefix(parameter);
                dicProperty[key] = this.GetHelpText(parameter);
            }
            return ConsoleAppHelper.GetConsoleHelper(dicProperty, padding);
        }

        public string GetHelpText(ArgumentMap argumentMap)
        {
            var argDesc = string.IsNullOrWhiteSpace(argumentMap.HelpText) ? "--" : argumentMap.HelpText;
            return argDesc;
        }   
    }
}
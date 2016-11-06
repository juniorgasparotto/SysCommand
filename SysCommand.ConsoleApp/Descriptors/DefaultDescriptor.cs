using SysCommand.Parsing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SysCommand.Execution;
using SysCommand.Mapping;
using SysCommand.Helpers;
using System;
using SysCommand.ConsoleApp.View;
using System.Collections;

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

        public string GetHelpText(IEnumerable<CommandMap> commandMaps)
        {
            const int WIDTH_USAGE_LEFT = 10;
            const int WIDTH_USAGE_RIGHT = 60;
            const int WIDTH_COMMAND = 80;
            const int WIDTH_MEMBER_LEFT = 0;
            const int WIDTH_MEMBER_RIGHT = 40;
            const int WIDTH_FOOTER = 50;
            const int PADDING_RIGHT = 4;

            var strBuilder = new StringBuilder();

            var usage = this.GetUsage(commandMaps);
            if (usage != null)
            {
                var tableUsage = new TableView(strBuilder);
                tableUsage.AddLineSeparator = false;
                tableUsage.AddColumnSeparator = false;

                tableUsage.AddColumnDefinition(null, WIDTH_USAGE_LEFT, 0, PADDING_RIGHT);
                tableUsage.AddColumnDefinition(null, WIDTH_USAGE_RIGHT);
                //tableUsage.AddRow()
                //    .AddColumnInRow("usage: git")
                //    .AddColumnInRow("[--version] [--help] [-C <path>] [-c name=value] [--exec-path[=<path>]] [--html-path] [--man-path] [--info-path] [-p | --paginate | --no-pager] --no-replace-objects] [--bare] [--git-dir=<path>] [--work-tree=<path>] [--namespace=<name>] <command> [<args>]");

                tableUsage.AddRow()
                    .AddColumnInRow(Strings.HelpUsageLabel)
                    .AddColumnInRow(usage);

                tableUsage.AddRowSummary("");
                tableUsage.Build();
            }

            var tableHelp = new TableView(strBuilder);
            tableHelp.AddLineSeparator = false;
            tableHelp.AddColumnSeparator = false;
            tableHelp.AddColumnDefinition(null, WIDTH_MEMBER_LEFT, 3, PADDING_RIGHT);
            tableHelp.AddColumnDefinition(null, WIDTH_MEMBER_RIGHT);

            foreach (var cmd in commandMaps)
            {
                var hasProperty = cmd.Properties.Any();
                var hasMethod = cmd.Methods.Any();

                if (hasProperty || hasMethod)
                {
                    tableHelp.AddRowSummary("");
                    tableHelp.AddRowSummary(cmd.Command.HelpText ?? cmd.Command.GetType().Name, WIDTH_COMMAND);
                    tableHelp.AddRowSummary("");

                    foreach (var property in cmd.Properties)
                    {
                        tableHelp.AddRow()
                            .AddColumnInRow(this.GetArgumentNameWithPrefix(property))
                            .AddColumnInRow(this.GetArgumentHelpText(property));
                    }

                    if (hasProperty && hasMethod && cmd.Methods.First().ArgumentsMaps.Any())
                        tableHelp.AddRowSummary("");

                    var lastMethod = cmd.Methods.LastOrDefault();
                    int index = 0;
                    foreach (var method in cmd.Methods)
                    {
                        var methodName = method.ActionName;
                        if (method.IsDefault)
                            methodName += " *";

                        var next = cmd.Methods.ElementAtOrDefault(index + 1);
                        tableHelp.AddRow()
                            .AddColumnInRow(methodName)
                            .AddColumnInRow(method.HelpText);

                        foreach (var arg in method.ArgumentsMaps)
                        {
                            tableHelp.AddRow()
                                .AddColumnInRow("   " + this.GetArgumentNameWithPrefix(arg))
                                .AddColumnInRow(this.GetArgumentHelpText(arg));
                        }

                        if (method != lastMethod)
                        {
                            var addBreakLine =
                                   method.ArgumentsMaps.Any()
                                || (next != null && next.ArgumentsMaps.Any());

                            if (addBreakLine)
                                tableHelp.AddRowSummary("");
                        }

                        index++;
                    }
                }
            }
            if (strBuilder.Length > 0)
            {
                tableHelp.AddRowSummary("");
                tableHelp.AddRowSummary(Strings.HelpFooterDesc, WIDTH_FOOTER);
            }
           
            var output = tableHelp.Build().ToString();
            if (output.Length == 0)
                return Strings.HelpEmpty;
            else
                return output;
        }

        public string GetHelpText(IEnumerable<CommandMap> commandMaps, string actionName)
        {
            const int WIDTH_USAGE_LEFT = 0;
            const int WIDTH_USAGE_RIGHT = 60;
            const int PADDING_RIGHT = 4;
            const int WIDTH_METHOD_DESC = 80;

            var actions = commandMaps
                .GetMethods()
                .Where(f => f.ActionName.ToLower() == actionName.ToLower())
                .OrderBy(f => string.IsNullOrWhiteSpace(f.HelpText) ? 0 : 1)
                .ToList();
            var strBuilder = new StringBuilder();
            var last = actions.LastOrDefault();

            foreach (var action in actions)
            {
               
                var usage = this.GetUsage(action.ArgumentsMaps);
                var tableUsage = new TableView(strBuilder);
                tableUsage.AddLineSeparator = false;
                tableUsage.AddColumnSeparator = false;

                tableUsage.AddColumnDefinition(null, WIDTH_USAGE_LEFT, 0, PADDING_RIGHT);
                tableUsage.AddColumnDefinition(null, WIDTH_USAGE_RIGHT);

                if (!string.IsNullOrWhiteSpace(action.HelpText))
                    tableUsage.AddRowSummary(action.HelpText, WIDTH_METHOD_DESC);

                var addSpace = !string.IsNullOrWhiteSpace(action.HelpText);
                tableUsage.AddRow()
                    .AddColumnInRow((addSpace ? "   " : "") + string.Format(Strings.HelpUsageActionLabel, action.ActionName))
                    .AddColumnInRow(usage);

                if (action != null)
                { 
                    tableUsage.AddRowSummary("");
                    tableUsage.AddRowSummary("");
                }

                tableUsage.Build();
            }

            if (strBuilder.Length == 0)
                return Strings.HelpNoActionFound;
            else
                return strBuilder.ToString();
        }

        public string GetUsage(IEnumerable<ArgumentMap> properties)
        {
            string usage = null;
            properties = properties.OrderBy(f => f.IsOptional ? 1 : 0);
            var last = properties.LastOrDefault();
            foreach (var prop in properties)
            {
                var propUsage = this.GetArgumentNameWithPrefix(prop, false, true);
                if (prop.IsOptional)
                    propUsage = "[" + propUsage + "]";

                if (last != prop)
                    propUsage += " ";

                usage += propUsage;
            }
            return usage;
        }

        public string GetUsage(IEnumerable<CommandMap> commandMaps)
        {
            string usage = this.GetUsage(commandMaps.GetProperties());
            if (commandMaps.GetMethods().Any())
            {
                var actionsUsage = string.Format("<{0}[{1}]>", Strings.HelpUsageMethodsLabel, Strings.HelpUsageMethodsParamsLabel);
                usage +=  (usage != null) ? " " + actionsUsage : actionsUsage;
            }

            return usage;
        }

        private StringBuilder GetErrors(IEnumerable<ExecutionError> commandsErrors)
        {
            var strBuilder = new StringBuilder();
            var count = commandsErrors.Count();
            var iErr = 0;
            foreach (var commandError in commandsErrors)
            {
                strBuilder.AppendLine(string.Format(Strings.ErrorInCommand, commandError.Command.GetType().Name));
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
                strBuilder.AppendLine(string.Format(Strings.ErrorInvalidMethod, GetMethodSpecification(invalid.ActionMap)));
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

        private string GetPropertyErrorDescription(ArgumentParsed argumentParsed)
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

        private string GetArgumentNameWithPrefix(ArgumentMap arg, bool includeTypeDesc = false, bool includeSampleValue = false)
        {
            string key = null;
            var shortName = arg.ShortName != null ? arg.ShortName.ToString() : null;
            if (!string.IsNullOrWhiteSpace(shortName) && !string.IsNullOrWhiteSpace(arg.LongName))
                key = "-" + arg.ShortName + ", --" + arg.LongName;
            else if (!string.IsNullOrWhiteSpace(shortName))
                key = "-" + arg.ShortName;
            else if (!string.IsNullOrWhiteSpace(arg.LongName))
                key = "--" + arg.LongName;

            if (includeSampleValue)
            {
                var sample = this.GetExampleValueForType(arg.Type);
                if (sample != null)
                    key = string.Format("{0}=<{1}>", key, sample);
            }

            if (includeTypeDesc)
                return key + " (" + ReflectionHelper.CSharpName(arg.Type) + ")";

            return key;
        }

        private string GetExampleValueForType(Type type)
        {
            Type typeOriginal = ReflectionHelper.GetTypeOrTypeOfNullable(type);

            if (typeOriginal != typeof(bool))
            {
                var listNumbers = new List<Type>()
                {
                    typeof(decimal),
                    typeof(int),
                    typeof(double),
                    typeof(byte),
                    typeof(short),
                    typeof(ushort),
                    typeof(uint),
                    typeof(long),
                    typeof(ulong),
                    typeof(float)
                };

                if (listNumbers.Contains(typeOriginal))
                {
                    return Strings.HelpUsageNumberLabel;
                }
                else if (typeOriginal == typeof(char))
                {
                    return Strings.HelpUsageLetterLabel;
                }
                else if (typeOriginal == typeof(string))
                {
                    return Strings.HelpUsagePhraseLabel;
                }
                else if (ReflectionHelper.IsEnum(typeOriginal))
                {
                    var values = Enum.GetNames(typeOriginal);
                    return string.Join("|", values);
                }
                else if (typeof(IEnumerable).IsAssignableFrom(typeOriginal))
                {
                    Type typeList;
                    var isArray = type.IsArray && type.GetElementType() != null;
                    if (isArray)
                        typeList = typeOriginal.GetElementType();
                    else
                        typeList = typeOriginal.GetGenericArguments().FirstOrDefault();
                    var sampleType = this.GetExampleValueForType(typeList);
                    return string.Format(Strings.HelpUsageListOfLabel, sampleType);
                }
            }

            return null;
        }

        private string GetArgumentHelpText(ArgumentMap arg)
        {
            string help = null;
            if (!string.IsNullOrWhiteSpace(arg.HelpText))
                help = arg.HelpText;
            //else if (this.parent.arguments is IHelp)
            //    help = ((IHelp)this.parent.arguments).GetHelp(property.Name);

            if (arg.ShowHelpComplement)
            {
                if (arg.IsOptional)
                    help = this.ConcatHelpWithOptional(help, arg.DefaultValue);
                else
                    help = this.ConcatHelpWithRequired(help);
            }

            return help;
        }

        private string ConcatHelpWithOptional(string help, object defaultValue)
        {
            if (defaultValue != null)
                return StringHelper.ConcatFinalPhase(help, string.Format(Strings.HelpArgDescOptionalWithDefaultValue, defaultValue));
            return StringHelper.ConcatFinalPhase(help, Strings.HelpArgDescOptionalWithoutDefaultValue);
        }

        private string ConcatHelpWithRequired(string help)
        {
            return StringHelper.ConcatFinalPhase(help, Strings.HelpArgDescRequired);
        }
    }
}
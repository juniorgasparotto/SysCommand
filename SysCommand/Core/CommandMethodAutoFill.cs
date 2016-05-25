using Fclp;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace SysCommand
{
    public class CommandMethodAutoFill
    {
        private CommandAction.InstanceAction action;
        private Dictionary<string, string> CommandsParseds = new Dictionary<string, string>();

        public CommandMethodAutoFill(CommandAction.InstanceAction action)
        {
            this.action = action;
        }

        protected virtual void Setup(MethodInfo method, ParameterInfo parameter, ArgumentAttribute attribute) { }

        public void AutoSetup()
        {
            foreach (var parameter in this.action.MethodInfo.GetParameters())
            {
                var parameterAttr = Attribute.GetCustomAttribute(parameter, typeof(ArgumentAttribute)) as ArgumentAttribute;
                this.Set(this.action.MethodInfo, parameter, parameterAttr);
            }
        }

        public string GetCommandsParsed()
        {
            var argsUseds = default(string);
            foreach (var arg in this.CommandsParseds)
            {
                var command = string.Format("{0} \"{1}\"", arg.Key, arg.Value.Replace("\\\"", "\\\"").Replace("'", "\'"));
                argsUseds += (argsUseds == null) ? command : " " + command;
            }

            return argsUseds;
        }

        private void Set(MethodInfo method, ParameterInfo parameter, ArgumentAttribute attribute)
        {
            // create the type-specific type of the helper
            var helperType = typeof(CommandMethodAutoFillGeneric<>).MakeGenericType(parameter.ParameterType);
            // create an instance of the helper
            // and upcast to base class
            var helper = (CommandMethodAutoFill)Activator.CreateInstance(helperType, this);
            // call base method
            helper.Setup(method, parameter, attribute);
        }

        // A subclass with a static type parameter
        private class CommandMethodAutoFillGeneric<TMember> : CommandMethodAutoFill
        {
            private CommandMethodAutoFill parent;

            public CommandMethodAutoFillGeneric(CommandMethodAutoFill parent)
                : base(null)
            {
                this.parent = parent;                
            }

            protected override void Setup(MethodInfo method, ParameterInfo parameter, ArgumentAttribute attribute)
            {
                if (attribute == null)
                    attribute = new ArgumentAttribute();

                ICommandLineOptionFluent<TMember> setup;
                var hasValue = !(parameter.DefaultValue is DBNull);
                var propertyValue = hasValue ? (TMember)parameter.DefaultValue : default(TMember);

                if (attribute.ShortName != default(char) && !string.IsNullOrWhiteSpace(attribute.LongName))
                    setup = this.parent.action.Parser.Setup<TMember>(attribute.ShortName, attribute.LongName);
                else if (attribute.ShortName != default(char))
                    setup = this.parent.action.Parser.Setup<TMember>(attribute.ShortName);
                else if (!string.IsNullOrWhiteSpace(attribute.LongName))
                    setup = this.parent.action.Parser.Setup<TMember>(attribute.LongName);
                else
                {
                    attribute.LongName = AppHelpers.ToLowerSeparate(parameter.Name, '-');
                    setup = this.parent.action.Parser.Setup<TMember>(attribute.LongName);
                }

                var help = default(string);
                if (!string.IsNullOrWhiteSpace(attribute.Help))
                    help = attribute.Help;
                else if (this.parent.action.Object is IHelp)
                    help = ((IHelp)this.parent.action.Object).GetHelp(parameter.Name);

                if (hasValue)
                {
                    setup.SetDefault(propertyValue);
                    setup.WithDescription(this.ConcatHelpWithDefaultValue(help, propertyValue));
                }
                else
                {
                    setup.WithDescription(this.ConcatHelpWithRequired(help));
                    if (this.parent.action.RequestAction != null)
                        setup.Required();
                }

                setup.Callback(value =>
                {
                    if (value != null && !value.Equals(propertyValue))
                    {
                        if (!string.IsNullOrWhiteSpace(attribute.LongName))
                            this.parent.CommandsParseds["--" + attribute.LongName] = value.ToString();
                        else
                            this.parent.CommandsParseds["-" + attribute.ShortName.ToString()] = value.ToString();
                    }

                    this.parent.action.ParametersParseds[parameter] = value;
                });
            }

            private string ConcatHelpWithDefaultValue(string help, object defaultValue)
            {
                if (string.IsNullOrWhiteSpace(help))
                    help = "";

                help = help.Trim();
                var defaultValueStr = "The default value is '" + defaultValue + "'.";

                if (help.LastOrDefault() == '.')
                    return help + " " + defaultValueStr;
                else if (help.Length > 0)
                    return help + ". " + defaultValueStr;
                else
                    return defaultValueStr;
            }

            private string ConcatHelpWithRequired(string help)
            {
                if (string.IsNullOrWhiteSpace(help))
                    help = "";

                help = help.Trim();
                var defaultValueStr = "This argument is required.";

                if (help.LastOrDefault() == '.')
                    return help + " " + defaultValueStr;
                else if (help.Length > 0)
                    return help + ". " + defaultValueStr;
                else
                    return defaultValueStr;
            }
        }
    }
}
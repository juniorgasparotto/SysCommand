using Fclp;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace SysCommand
{
    public class CommandPropertyAutoFill
    {
        private FluentCommandLineParser parser;
        private object arguments;
        private string[] args;
        private Dictionary<string, string> CommandsParseds = new Dictionary<string, string>();

        public bool HasFill { get; private set; }

        public CommandPropertyAutoFill(FluentCommandLineParser parser, object argumentsToUpdate, string[] args)
        {
            this.parser = parser;
            this.arguments = argumentsToUpdate;
            this.args = args;
        }

        protected virtual void Setup(PropertyInfo property, ArgumentAttribute attribute) {}
        protected virtual void Setup(PropertyInfo property) {}

        public void AutoSetup()
        {
            PropertyInfo[] properties = arguments.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                var attribute = Attribute.GetCustomAttribute(property, typeof(ArgumentAttribute)) as ArgumentAttribute;
                this.Set(property, attribute);
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

        private void Set(PropertyInfo property, ArgumentAttribute attribute)
        {
            // create the type-specific type of the helper
            var helperType = typeof(CommandPropertyAutoFillGeneric<>).MakeGenericType(property.PropertyType);
            // create an instance of the helper
            // and upcast to base class
            var helper = (CommandPropertyAutoFill)Activator.CreateInstance(helperType, this);
            // call base method
            if (attribute != null)
                helper.Setup(property, attribute);
            else
                helper.Setup(property);
        }

        // A subclass with a static type parameter
        private class CommandPropertyAutoFillGeneric<TProperty> : CommandPropertyAutoFill
        {
            private CommandPropertyAutoFill parent;

            public CommandPropertyAutoFillGeneric(CommandPropertyAutoFill parent)
                : base(null, null, null)
            {
                this.parent = parent;                
            }

            protected override void Setup(PropertyInfo property, ArgumentAttribute attribute)
            {
                ICommandLineOptionFluent<TProperty> setup;
                
                var propertyValueObj = property.GetValue(this.parent.arguments);
                var propertyValue = default(TProperty);
                if (propertyValueObj != null)
                    propertyValue = (TProperty)propertyValueObj;

                if (attribute.ShortName != default(char) && !string.IsNullOrWhiteSpace(attribute.LongName))
                    setup = this.parent.parser.Setup<TProperty>(attribute.ShortName, attribute.LongName);
                else if (attribute.ShortName != default(char))
                    setup = this.parent.parser.Setup<TProperty>(attribute.ShortName);
                else if (!string.IsNullOrWhiteSpace(attribute.LongName))
                    setup = this.parent.parser.Setup<TProperty>(attribute.LongName);
                else
                {
                    attribute.LongName = AppHelpers.ToLowerSeparate(property.Name, '-');                
                    setup = this.parent.parser.Setup<TProperty>(attribute.LongName);
                }

                if (attribute.IsRequired)
                    setup.Required();

                if (attribute.Default != null && (propertyValue == null || propertyValue.Equals(default(TProperty))))
                    setup.SetDefault((TProperty)attribute.Default);

                setup.Callback(value =>
                    {
                        if (!value.Equals(attribute.Default))
                        {
                            if (!string.IsNullOrWhiteSpace(attribute.LongName))
                                this.parent.CommandsParseds["--" + attribute.LongName] = value.ToString();
                            else 
                                this.parent.CommandsParseds["-" + attribute.ShortName.ToString()] = value.ToString();
                        }

                        this.parent.HasFill = true;
                        property.SetValue(this.parent.arguments, value, null); // null means no indexes
                    });

                var help = default(string);
                if (!string.IsNullOrWhiteSpace(attribute.Help))
                    help = attribute.Help;
                else if (this.parent.arguments is IHelp)
                    help = ((IHelp)this.parent.arguments).GetHelp(property.Name);
                    
                if (help != null)
                {
                    if (attribute.ShowDefaultValueInHelp)
                        help = this.ConcatHelpWithDefaultValue(help, CommandStorage.GetValueForArgsType<TProperty>(property));
                    setup.WithDescription(help);
                }
            }

            protected override void Setup(PropertyInfo property)
            {
                ICommandLineOptionFluent<TProperty> setup;
                var name = AppHelpers.ToLowerSeparate(property.Name, '-');

                setup = this.parent.parser.Setup<TProperty>(name);
                setup.Callback(value =>
                {
                    this.parent.HasFill = true;
                    this.parent.CommandsParseds["--" + name] = value.ToString();
                    property.SetValue(this.parent.arguments, value, null); // null means no indexes
                });

                string help = null;
                if (this.parent.arguments is IHelp)
                    help = ((IHelp)this.parent.arguments).GetHelp(property.Name);

                var helpDefaultValue = CommandStorage.GetValueForArgsType<TProperty>(property);
                if (helpDefaultValue != null && !helpDefaultValue.Equals(default(TProperty)))
                    setup.WithDescription(this.ConcatHelpWithDefaultValue(help, helpDefaultValue));
                else
                    setup.WithDescription(help);
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
        }
    }
}
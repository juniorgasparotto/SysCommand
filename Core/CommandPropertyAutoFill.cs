using Fclp;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SysCommand
{
    public class CommandPropertyAutoFill
    {
        private FluentCommandLineParser parser;
        private IArguments arguments;
        private string[] args;
        private Dictionary<string, string> CommandsKeysValues = new Dictionary<string, string>();

        public bool HasFill 
        {
            get
            {
                if (this.CommandsKeysValues.Count > 0)
                    return true;
                return false;
            }
        }

        public CommandPropertyAutoFill(FluentCommandLineParser parser, IArguments argumentsToUpdate, string[] args)
        {
            this.parser = parser;
            this.arguments = argumentsToUpdate;
            this.args = args;
        }

        protected virtual void Setup(PropertyInfo property, CommandPropertyAttribute attribute)
        {

        }

        public void AutoSetup()
        {
            PropertyInfo[] properties = arguments.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                var attribute = Attribute.GetCustomAttribute(property, typeof(CommandPropertyAttribute)) as CommandPropertyAttribute;
                if (attribute != null)
                    this.Set(property, attribute);
            }

            this.arguments.Command = this.GetCommandsUsed();
        }

        private string GetCommandsUsed()
        {
            var argsUseds = default(string);
            foreach (var arg in this.CommandsKeysValues)
            {
                var command = string.Format("--{0} \"{1}\"", arg.Key, arg.Value);
                argsUseds += (argsUseds == null) ? command : " " + command;
            }

            return argsUseds;
        }

        private void Set(PropertyInfo property, CommandPropertyAttribute attribute)
        {
            // create the type-specific type of the helper
            var helperType = typeof(CommandLineParserAutoSetupGeneric<>).MakeGenericType(property.PropertyType);
            // create an instance of the helper
            // and upcast to base class
            var helper = (CommandPropertyAutoFill)Activator.CreateInstance(helperType, this);
            // call base method
            helper.Setup(property, attribute);
        }

        // A subclass with a static type parameter
        private class CommandLineParserAutoSetupGeneric<TProperty> : CommandPropertyAutoFill
        {
            private CommandPropertyAutoFill parent;

            public CommandLineParserAutoSetupGeneric(CommandPropertyAutoFill parent)
                : base(null, null, null)
            {
                this.parent = parent;
            }

            protected override void Setup(PropertyInfo property, CommandPropertyAttribute attribute)
            {
                // create an Action<T> and downcast
                ICommandLineOptionFluent<TProperty> setup;

                if (attribute.ShortName != default(char))
                    setup = this.parent.parser.Setup<TProperty>(attribute.ShortName, attribute.LongName);
                else 
                    setup = this.parent.parser.Setup<TProperty>(attribute.LongName);

                if (attribute.IsRequired)
                    setup.Required();

                setup.Callback(value =>
                    {
                        this.parent.CommandsKeysValues[attribute.LongName] = value.ToString();
                        property.SetValue(this.parent.arguments, value, null); // null means no indexes
                    });

                if (!string.IsNullOrWhiteSpace(attribute.Help))
                    setup.WithDescription(attribute.Help);
                else
                    setup.WithDescription(this.parent.arguments.GetHelp(property.Name));
            }
        }
    }
}
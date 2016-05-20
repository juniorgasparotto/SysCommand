using Fclp;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SysCommand
{
    public class CommandPropertyAutoFill
    {
        private FluentCommandLineParser parser;
        private object arguments;
        private string[] args;
        private Dictionary<string, string> CommandsParseds = new Dictionary<string, string>();

        public bool HasFill 
        {
            get
            {
                if (this.CommandsParseds.Count > 0)
                    return true;
                return false;
            }
        }

        public CommandPropertyAutoFill(FluentCommandLineParser parser, object argumentsToUpdate, string[] args)
        {
            this.parser = parser;
            this.arguments = argumentsToUpdate;
            this.args = args;
        }

        protected virtual void Setup(PropertyInfo property, CommandPropertyAttribute attribute) {}
        protected virtual void Setup(PropertyInfo property) {}

        public void AutoSetup()
        {
            PropertyInfo[] properties = arguments.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                var attribute = Attribute.GetCustomAttribute(property, typeof(CommandPropertyAttribute)) as CommandPropertyAttribute;
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

        private void Set(PropertyInfo property, CommandPropertyAttribute attribute)
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

            protected override void Setup(PropertyInfo property, CommandPropertyAttribute attribute)
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

                        property.SetValue(this.parent.arguments, value, null); // null means no indexes
                    });

                if (!string.IsNullOrWhiteSpace(attribute.Help))
                    setup.WithDescription(attribute.Help);
                else if (this.parent.arguments is IHelp)
                    setup.WithDescription(((IHelp)this.parent.arguments).GetHelp(property.Name));
            }

            protected override void Setup(PropertyInfo property)
            {
                ICommandLineOptionFluent<TProperty> setup;
                var name = AppHelpers.ToLowerSeparate(property.Name, '-');

                setup = this.parent.parser.Setup<TProperty>(name);
                setup.Callback(value =>
                {
                    this.parent.CommandsParseds["--" + name] = value.ToString();
                    property.SetValue(this.parent.arguments, value, null); // null means no indexes
                });

                string help = null;
                if (this.parent.arguments is IHelp)
                    help = ((IHelp)this.parent.arguments).GetHelp(property.Name);
                
                setup.WithDescription(help ?? "?");
            }
        }
    }
}
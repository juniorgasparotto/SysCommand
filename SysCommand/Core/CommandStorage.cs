using System.Collections.Generic;
using System.IO;
using System;
using Newtonsoft.Json;
using System.Linq.Expressions;
using System.Reflection;

namespace SysCommand
{
    public class CommandStorage
    {
        public Dictionary<string, Dictionary<string, Item>> All { get; private set; }

        public class Item
        {
            public string Command { get; set; }
            public object Object { get; set; }
        }

        public CommandStorage()
        {
            this.All = new Dictionary<string, Dictionary<string, Item>>();
        }

        public void Remove(string commandName)
        {
            this.All.Remove(commandName);
        }

        public Item GetArguments(string commandName, Type argsType)
        {
            if (this.All.ContainsKey(commandName) && this.All[commandName].ContainsKey(argsType.FullName))
                return this.All[commandName][argsType.FullName];
            return null;
        }

        public void SetArguments(string commandName, Item item)
        {
            if (item == null || item.Object == null)
                throw new Exception("Parameter 'item' or 'item.Object' can't be null in Arguments.History.SetCommandArguments.");

            if (!this.All.ContainsKey(commandName))
                this.All.Add(commandName, new Dictionary<string, Item>());
            this.All[commandName][item.Object.GetType().FullName] = item;
        }

        public static object GetValueForArgsType<TArgs>(Expression<Func<TArgs, object>> expression, string commandName = null)
        {
            return GetValueForArgsType<TArgs, object>(expression, commandName);
        }

        public static TProp GetValueForArgsType<TArgs, TProp>(Expression<Func<TArgs, object>> expression, string commandName = null)
        {
            if (commandName == null)
                commandName = App.COMMAND_NAME_DEFAULT;

            var argsItem = App.Current.GetOrCreateObjectFile<CommandStorage>().GetArguments(commandName, typeof(TArgs));

            if (argsItem != null && argsItem.Object != null)
            {
                var prop = AppHelpers.GetPropertyInfo<TArgs>(expression);
                return (TProp)prop.GetValue(argsItem.Object);
            }
            else
            {
                var prop = AppHelpers.GetPropertyInfo<TArgs>(expression);
                var attribute = Attribute.GetCustomAttribute(prop, typeof(ArgumentAttribute)) as ArgumentAttribute;
                if (attribute != null && attribute.Default != null)
                    return (TProp)attribute.Default;
            }

            return default(TProp);
        }

        public static TProp GetValueForArgsType<TProp>(PropertyInfo prop, string commandName = null)
        {
            if (commandName == null)
                commandName = App.COMMAND_NAME_DEFAULT;

            var argsItem = App.Current.GetOrCreateObjectFile<CommandStorage>().GetArguments(commandName, prop.DeclaringType);

            if (argsItem != null && argsItem.Object != null)
            {
                return (TProp)prop.GetValue(argsItem.Object);
            }
            else
            {
                var attribute = Attribute.GetCustomAttribute(prop, typeof(ArgumentAttribute)) as ArgumentAttribute;
                if (attribute != null && attribute.Default != null)
                    return (TProp)attribute.Default;
            }

            return default(TProp);
        }
    }
}

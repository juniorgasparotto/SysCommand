using System.Collections.Generic;
using System.IO;
using System;
using Newtonsoft.Json;

namespace SysCommand
{
    public class ArgumentsHistory
    {
        public Dictionary<string, Dictionary<string, ArgumentsHistoryItem>> CommandsHistories { get; private set; }

        public class ArgumentsHistoryItem
        {
            public string Command { get; set; }
            public object Object { get; set; }
        }

        public ArgumentsHistory()
        {
            this.CommandsHistories = new Dictionary<string, Dictionary<string, ArgumentsHistoryItem>>();
        }

        public void DeleteCommand(string commandName)
        {
            this.CommandsHistories.Remove(commandName);
        }

        public ArgumentsHistoryItem GetCommandArguments(string commandName, Type type)
        {
            if (this.CommandsHistories.ContainsKey(commandName) && this.CommandsHistories[commandName].ContainsKey(type.FullName))
                return this.CommandsHistories[commandName][type.FullName];
            return null;
        }

        public void SetCommandArguments(string commandName, ArgumentsHistoryItem item)
        {
            if (item == null || item.Object == null)
                throw new Exception("Parameter 'item' or 'item.Object' can't be null in Arguments.History.SetCommandArguments.");

            if (!this.CommandsHistories.ContainsKey(commandName))
                this.CommandsHistories.Add(commandName, new Dictionary<string, ArgumentsHistoryItem>());
            this.CommandsHistories[commandName][item.Object.GetType().FullName] = item;
        }
    }
}

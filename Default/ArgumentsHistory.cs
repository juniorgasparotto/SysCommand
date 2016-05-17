using System.Collections.Generic;
using System.IO;
using System;
using Newtonsoft.Json;

namespace SysCommand
{
    public class ArgumentsHistory : Config
    {
        public Dictionary<string, Dictionary<string, IArguments>> CommandsHistories { get; private set; }

        public ArgumentsHistory(string fileName) : base(fileName)
        {
            this.CommandsHistories = new Dictionary<string, Dictionary<string, IArguments>>();
        }

        public void DeleteCommand(string commandName)
        {
            this.CommandsHistories.Remove(commandName);
        }

        public IArguments GetCommandArguments(string commandName, Type type)
        {
            if (this.CommandsHistories.ContainsKey(commandName) && this.CommandsHistories[commandName].ContainsKey(type.FullName))
                return this.CommandsHistories[commandName][type.FullName];
            return null;
        }

        public void SetCommandArguments(string commandName, IArguments parameters)
        {
            if (!this.CommandsHistories.ContainsKey(commandName))
                this.CommandsHistories.Add(commandName, new Dictionary<string, IArguments>());
            this.CommandsHistories[commandName][parameters.GetType().FullName] = parameters;
        }
    }
}

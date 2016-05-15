using System.Collections.Generic;
using System.IO;
using System;
using Newtonsoft.Json;

namespace SysCommand
{
    public class Configuration
    {
#if DEBUG
        private const string DEFAULT_FILENAME = @"..\..\configuration.json";
#else
        private const string DEFAULT_FILENAME = "configuration.json";
#endif

        public Dictionary<string, Dictionary<Type, IArguments>> Commands { get; private set; }

        private Configuration()
        {
            this.Commands = new Dictionary<string, Dictionary<Type, IArguments>>();
        }

        public void Save(string fileName = DEFAULT_FILENAME)
        {
            File.WriteAllText(fileName, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public void DeleteCommand(string commandName)
        {
            this.Commands.Remove(commandName);
        }

        public IArguments GetCommandParameters(string commandName, Type type)
        {
            if (this.Commands.ContainsKey(commandName) && this.Commands[commandName].ContainsKey(type))
                return this.Commands[commandName][type];
            return null;
        }

        public void SetCommandParameters(string commandName, IArguments parameters)
        {
            if (!this.Commands.ContainsKey(commandName))
                this.Commands.Add(commandName, new Dictionary<Type, IArguments>());
            this.Commands[commandName][parameters.GetType()] = parameters;
        }

        public static void Save(Configuration pSettings, string fileName = DEFAULT_FILENAME)
        {
            File.WriteAllText(fileName, JsonConvert.SerializeObject(pSettings, Formatting.Indented));
        }

        public static Configuration Get(string fileName = DEFAULT_FILENAME)
        {
            if (File.Exists(fileName))
                return JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(fileName));
            return null;
        }
    }
}

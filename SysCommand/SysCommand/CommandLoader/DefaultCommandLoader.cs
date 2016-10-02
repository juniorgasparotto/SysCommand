using System.Collections.Generic;
using System.Linq;
using System;

namespace SysCommand
{
    public sealed class DefaultCommandLoader
    {
        public List<Type> IgnoredCommands { get; private set; }

        public DefaultCommandLoader(IEnumerable<Type> ignoredCommands = null)
        {
            this.IgnoredCommands = new List<Type>();
            if (ignoredCommands != null)
                this.IgnoredCommands.AddRange(ignoredCommands);
        }

        public void IgnoreCommand<T>()
        {
            this.IgnoredCommands.Add(typeof(T));
        }
        
        public IEnumerable<Command> GetFromAppDomain(bool isDebug)
        {
            var listOfCommands = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                                  from assemblyType in domainAssembly.GetTypes()
                                  where
                                         typeof(Command).IsAssignableFrom(assemblyType)
                                      && assemblyType.IsInterface == false
                                      && assemblyType.IsAbstract == false
                                  select assemblyType).ToList();

            var commandsList = listOfCommands.Select(f => (Command)Activator.CreateInstance(f)).OrderBy(f => f.OrderExecution).ToList();
            commandsList.RemoveAll(f => this.IgnoredCommands.Contains(f.GetType()) || (!isDebug && f.OnlyInDebug));
            return commandsList;
        }
    }
}

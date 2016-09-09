using System.Collections.Generic;
using System.Linq;
using System;

namespace SysCommand
{
    public class CommandAppDomainLoader
    {
        public List<Type> IgnoredCommands { get; private set; }

        public CommandAppDomainLoader(List<Type> ignoredCommands = null)
        {
            this.IgnoredCommands = ignoredCommands;
        }

        public void IgnoreCommand<T>()
        {
            this.IgnoredCommands.Add(typeof(T));
        }
        
        public IEnumerable<Command> GetFromAppDomain()
        {
            var listOfCommands = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                                  from assemblyType in domainAssembly.GetTypes()
                                  where
                                         typeof(Command).IsAssignableFrom(assemblyType)
                                      && assemblyType.IsInterface == false
                                      && assemblyType.IsAbstract == false
                                  select assemblyType).ToList();

            var commandsList = listOfCommands.Select(f => (Command)Activator.CreateInstance(f)).OrderBy(f => f.OrderExecution).ToList();
            commandsList.RemoveAll(f => this.IgnoredCommands.Contains(f.GetType()) || (!Debug.IsInDebug && f.OnlyInDebug));
            return commandsList;
        }
    }
}

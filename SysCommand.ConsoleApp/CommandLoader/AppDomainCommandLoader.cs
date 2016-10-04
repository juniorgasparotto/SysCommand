using System.Collections.Generic;
using System.Linq;
using System;

namespace SysCommand.ConsoleApp
{
    public sealed class AppDomainCommandLoader
    {
        public List<Type> IgnoredCommands { get; private set; }

        public AppDomainCommandLoader(IEnumerable<Type> ignoredCommands = null)
        {
            this.IgnoredCommands = new List<Type>();
            if (ignoredCommands != null)
                this.IgnoredCommands.AddRange(ignoredCommands);
        }

        public void IgnoreCommand<T>()
        {
            this.IgnoredCommands.Add(typeof(T));
        }
        
        public IEnumerable<CommandBase> GetFromAppDomain(bool isDebug)
        {
            var listOfCommands = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                                  from assemblyType in domainAssembly.GetTypes()
                                  where
                                         typeof(CommandBase).IsAssignableFrom(assemblyType)
                                      && assemblyType.IsInterface == false
                                      && assemblyType.IsAbstract == false
                                  select assemblyType).ToList();

            var commandsList = listOfCommands.Select(f => (CommandBase)Activator.CreateInstance(f)).OrderBy(f => f.OrderExecution).ToList();
            commandsList.RemoveAll(f => this.IgnoredCommands.Contains(f.GetType()) || (!isDebug && f.OnlyInDebug));
            
            // This "order" it's only has the best vision in debug
            // it's better see the System commands on the top the list.
            return commandsList.OrderByDescending(f => f.Tag);
        }
    }
}

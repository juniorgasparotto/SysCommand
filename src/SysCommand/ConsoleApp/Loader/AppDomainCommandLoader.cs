using SysCommand.Compatibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SysCommand.ConsoleApp.Loader
{
    public sealed class AppDomainCommandLoader
    {
        public List<Type> IgnoredCommands { get; }

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

        public IEnumerable<Type> GetFromAppDomain()
        {
            var assemblies = ReflectionCompatibility.GetAssemblies().ToList();
#if NETCORE
            assemblies.Add(this.GetType().GetTypeInfo().Assembly);
#endif
            var listOfCommands = (from domainAssembly in assemblies
                                  from assemblyType in domainAssembly.GetTypes()
                                  where
                                         typeof(Command).IsAssignableFrom(assemblyType)
                                      && assemblyType.IsInterface() == false
                                      && assemblyType.IsAbstract() == false
                                  select assemblyType).ToList();
            listOfCommands.RemoveAll(f => this.IgnoredCommands.Contains(f));
            return listOfCommands;
        }

    }
}

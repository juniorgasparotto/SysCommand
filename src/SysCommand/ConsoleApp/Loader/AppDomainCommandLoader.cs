using SysCommand.Compatibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SysCommand.ConsoleApp.Loader
{
    /// <summary>
    /// Is responsible for pick up Commands automatically and you can use it if you want to use. 
    /// Internally the system makes use of it if the commandsTypes parameter is null.
    /// </summary>
    public sealed class AppDomainCommandLoader
    {
        /// <summary>
        /// List of types to ignore
        /// </summary>
        public List<Type> IgnoredCommands { get; }

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="ignoredCommands">List of types to ignore</param>
        public AppDomainCommandLoader(IEnumerable<Type> ignoredCommands = null)
        {
            this.IgnoredCommands = new List<Type>();
            if (ignoredCommands != null)
                this.IgnoredCommands.AddRange(ignoredCommands);
        }

        /// <summary>
        /// Add ignored type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void IgnoreCommand<T>()
        {
            this.IgnoredCommands.Add(typeof(T));
        }

        /// <summary>
        /// Returns all commands except those in the ignore list.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Type> GetFromAppDomain()
        {
            var assemblies = ReflectionCompatibility.GetAssemblies().ToList();
#if NETCORE
            assemblies.Add(this.GetType().GetTypeInfo().Assembly);
#endif
            var listOfCommands = (from domainAssembly in assemblies.Distinct()
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

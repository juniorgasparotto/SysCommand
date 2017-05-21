using System.Collections.Generic;
using System.Linq;
using System;

namespace SysCommand.Mapping
{
    /// <summary>
    /// Extension with some search help functions from CommandMap
    /// </summary>
    public static class EnumerableCommandMapExtensions 
    {
        /// <summary>
        /// Get T in list
        /// </summary>
        /// <typeparam name="T">Type to be found</typeparam>
        /// <param name="maps">List o commands maps</param>
        /// <returns>Instance of CommadMap</returns>
        public static CommandMap GetCommandMap<T>(this IEnumerable<CommandMap> maps)
        {
            return maps.LastOrDefault(c => c.Command is T);
        }

        /// <summary>
        /// Get specific type in list
        /// </summary>
        /// <param name="maps">List o commands maps</param>
        /// <param name="type">Type to be found</param>
        /// <returns>Instance of CommadMap</returns>
        public static CommandMap GetCommandMap(this IEnumerable<CommandMap> maps, Type type)
        {
            return maps.LastOrDefault(c => c.Command.GetType() == type);
        }

        /// <summary>
        /// Get all commands in list of commands maps
        /// </summary>
        /// <param name="maps">List o commands maps</param>
        /// <returns>List of CommandBase</returns>
        public static IEnumerable<CommandBase> GetCommands(this IEnumerable<CommandMap> maps)
        {
            return maps.Select(c => c.Command);
        }

        /// <summary>
        /// Get all ActionMap in list of commands maps
        /// </summary>
        /// <param name="maps">List o commands maps</param>
        /// <returns>List of ActionMap</returns>
        public static IEnumerable<ActionMap> GetMethods(this IEnumerable<CommandMap> maps)
        {
            return maps.SelectMany(c => c.Methods);
        }

        /// <summary>
        /// Get all ArgumentMap in list of commands maps
        /// </summary>
        /// <param name="maps">List o commands maps</param>
        /// <returns>List of ArgumentMap</returns>
        public static IEnumerable<ArgumentMap> GetProperties(this IEnumerable<CommandMap> maps)
        {
            return maps.SelectMany(c => c.Properties);
        }
    }
}

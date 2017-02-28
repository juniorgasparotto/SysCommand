using System.Collections.Generic;
using System.Linq;
using System;

namespace SysCommand.Mapping
{
    public static class EnumerableCommandMapExtensions 
    {
        public static CommandMap GetCommandMap<T>(this IEnumerable<CommandMap> maps)
        {
            return maps.LastOrDefault(c => c.Command is T);
        }

        public static CommandMap GetCommandMap(this IEnumerable<CommandMap> maps, Type type)
        {
            return maps.LastOrDefault(c => c.Command.GetType() == type);
        }

        public static IEnumerable<CommandBase> GetCommands(this IEnumerable<CommandMap> maps)
        {
            return maps.Select(c => c.Command);
        }

        public static IEnumerable<ActionMap> GetMethods(this IEnumerable<CommandMap> maps)
        {
            return maps.SelectMany(c => c.Methods);
        }

        public static IEnumerable<ArgumentMap> GetProperties(this IEnumerable<CommandMap> maps)
        {
            return maps.SelectMany(c => c.Properties);
        }
    }
}

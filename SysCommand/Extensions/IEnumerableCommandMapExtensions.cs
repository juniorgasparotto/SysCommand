using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;
using SysCommand.Parser;

namespace SysCommand
{
    public static class IEnumerableCommandMapExtensions 
    {
        public static CommandMap GetMap<T>(this IEnumerable<CommandMap> maps)
        {
            return maps.Where(c => c.Command is T).FirstOrDefault();
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

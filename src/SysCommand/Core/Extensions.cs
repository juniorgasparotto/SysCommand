using System.Collections.Generic;
using System.Linq;
using System;
using SysCommand.ConsoleApp;

namespace SysCommand
{
    public static class Extensions
    {
        public static T Get<T>(this IEnumerable<Command> collection) where T : Command
        {
            return collection.Where(f => f is T).Cast<T>().FirstOrDefault();
        }
    }
}

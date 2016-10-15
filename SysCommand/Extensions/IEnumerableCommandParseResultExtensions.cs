using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;
using SysCommand.Parser;

namespace SysCommand
{
    public static class IEnumerableCommandParseResultExtensions 
    {
        public static IEnumerable<CommandParseResult> GetAllValid(this IEnumerable<CommandParseResult> list)
        {
            return list.Where(c => c.IsValid);
        }

        //public static IEnumerable<CommandParseResult> GetAllInvalid(this IEnumerable<CommandParseResult> list)
        //{
        //    return list.Where(c => !c.IsValid);
        //}

        public static IEnumerable<CommandParseResult> GetAllWithError(this IEnumerable<CommandParseResult> list)
        {
            return list.Where(c => c.HasError);
        }

        public static CommandParseResult GetCommandParseResult(this IEnumerable<CommandParseResult> list, object obj)
        {
            return list.Where(c => c.Command == obj).First();
        }

        public static Result<IMember> GetResult(this IEnumerable<CommandParseResult> list)
        {
            return new Result<IMember>(list.SelectMany(f => f.GetResult()));
        }

        public static IEnumerable<ArgumentMapped> GetProperties(this IEnumerable<CommandParseResult> list)
        {
            return list.SelectMany(a => a.Levels.SelectMany(b => b.Properties));
        }

        public static IEnumerable<ActionMapped> GetMethods(this IEnumerable<CommandParseResult> list)
        {
            return list.SelectMany(a => a.Levels.SelectMany(b => b.Methods));
        }

        public static IEnumerable<ArgumentMapped> GetPropertiesInvalid(this IEnumerable<CommandParseResult> list)
        {
            return list.SelectMany(a => a.Levels.SelectMany(b => b.PropertiesInvalid));
        }

        public static IEnumerable<ActionMapped> GetMethodsInvalid(this IEnumerable<CommandParseResult> list)
        {
            return list.SelectMany(a => a.Levels.SelectMany(b => b.MethodsInvalid));
        }
    }
}

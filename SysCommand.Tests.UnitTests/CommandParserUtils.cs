using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection;
using System.Text;
using System.Globalization;
using System.Collections;
using SysCommand.Mapping;
using SysCommand.Parsing;
using SysCommand.DefaultExecutor;

namespace SysCommand.Tests.UnitTests
{
    public static class CommandParserUtils
    {
        public static IEnumerable<ActionMap> GetActionsMapsFromTargetObject(object target, bool onlyWithAttribute = false, bool usePrefixInAllMethods = false, string prefix = null)
        {
            var argumentMapper = new ArgumentMapper();
            var actionMapper = new ActionMapper(argumentMapper);
            return actionMapper.Map(target, onlyWithAttribute, usePrefixInAllMethods, prefix);
        }

        public static IEnumerable<ArgumentRaw> ParseArgumentsRaw(string[] args, IEnumerable<ActionMap> actionsMaps = null)
        {
            var argumentRaw = new ArgumentRawParser();
            return argumentRaw.Parse(args, actionsMaps);
        }

        public static IEnumerable<ActionParsed> GetActionsParsed(IEnumerable<ArgumentRaw> argumentsRaw, bool enableMultiAction, IEnumerable<ActionMap> maps, out IEnumerable<ArgumentRaw> initialExtraArguments)
        {
            var parser = new ActionParser(new ArgumentParser());
            return parser.Parse(argumentsRaw, enableMultiAction, maps, out initialExtraArguments);
        }

        public static IEnumerable<ArgumentMap> GetArgumentsMapsFromMethod(object target, MethodInfo method)
        {
            var argumentMapper = new ArgumentMapper();
            return argumentMapper.GetFromMethod(target, method);
        }

        public static IEnumerable<ArgumentMap> GetArgumentsMapsFromProperties(object target, bool onlyWithAttribute = false)
        {
            var argumentMapper = new ArgumentMapper();
            return argumentMapper.Map(target, onlyWithAttribute);
        }

        public static IEnumerable<ArgumentMap> GetArgumentsMapsFromProperties(object target, PropertyInfo[] properties, bool onlyWithAttribute = false)
        {
            var argumentMapper = new ArgumentMapper();
            return argumentMapper.Map(target, properties, onlyWithAttribute);
        }

        public static IEnumerable<ArgumentParsed> GetArgumentsParsed(IEnumerable<ArgumentRaw> argumentsRaw, bool enablePositionalArgs, IEnumerable<ArgumentMap> maps)
        {
            var parser = new ArgumentParser();
            return parser.Parse(argumentsRaw, enablePositionalArgs, maps);
        }
    }
}

using System.Collections.Generic;
using System;
using System.Reflection;
using SysCommand.Mapping;
using SysCommand.Parsing;
using SysCommand.DefaultExecutor;
using SysCommand.Helpers;
using SysCommand.Execution;
using System.Linq;

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

        public static string GetMethodSpecification(MethodResult result)
        {
            var format = "{0}({1})";
            string args = null;
            foreach (var map in result.ActionParsed.ActionMap.ArgumentsMaps)
            {
                var typeName = ReflectionHelper.CSharpName(map.Type);
                var value = result.ActionParsed.Arguments.Where(f => f.Map == map).First();
                var valueWithDesc = value.GetArgumentNameInputted()  + " = " + (string.IsNullOrWhiteSpace(value.Raw) ? "?" : value.Raw);
                args += args == null ? typeName + " " + valueWithDesc : ", " + typeName + " " + valueWithDesc;
            }
            return string.Format(format, result.ActionParsed.ActionMap.ActionName, args);
        }

        public static string GetMethodSpecification(ActionMap map)
        {
            var format = "{0}({1})";
            string args = null;
            foreach (var arg in map.ArgumentsMaps)
            {
                var typeName = ReflectionHelper.CSharpName(arg.Type);
                args += args == null ? typeName : ", " + typeName;
            }
            return string.Format(format, map.ActionName, args);
        }

        public static string GetMethodSpecification2(ActionMap map)
        {
            var format = "{0}({1})";
            string args = null;
            foreach (var arg in map.ArgumentsMaps)
            {
                var typeName = ReflectionHelper.CSharpName(arg.Type);
                var argsDef = typeName + " " + GetArgsDefinition(arg);
                args += args == null ? argsDef : ", " + argsDef;
            }
            return string.Format(format, map.ActionName, args);
        }

        public static string GetArgsDefinition(ArgumentMap arg)
        {
            var isDefault = arg.IsOptional || arg.HasDefaultValue ? " = ?" : "";
            var typeName = ReflectionHelper.CSharpName(arg.Type);
            var delimiterLong = arg.LongName != null ? "--" + arg.LongName : "";
            var delimiterShort = arg.ShortName != null ? "-" + arg.ShortName : "";
            var delimiter = "";
            if (delimiterLong != "" && delimiterShort != "")
                delimiter = delimiterShort + "|" + delimiterLong;
            else if (delimiterLong != "")
                delimiter = delimiterLong;
            else
                delimiter = delimiterShort;
            return delimiter + isDefault;
        }
    }
}

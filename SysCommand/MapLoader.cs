using System.Collections.Generic;
using System.Linq;
using System;
using Fclp;
using System.Linq.Expressions;
using System.IO;

namespace SysCommand
{
    public class MapLoader
    {
        public List<Command> Commands { get; private set; }
        public List<ActionMap> ActionsMaps { get; private set; }
        public List<ArgumentMap> GlobalMaps { get; private set; }

        public void AddCommand(Command command)
        {
            this.ActionsMaps.AddRange(CommandParser.GetActionsMapsFromType(command.GetType(), command.OnlyMethodsWithAttribute, command.UsePrefixInAllMethods, command.PrefixMethods));
            this.GlobalMaps.AddRange(CommandParser.GetArgumentsMapsFromProperties(command.GetType(), command.OnlyPropertiesWithAttribute));
            this.Commands.Add(command);
        }

        public static MapLoader LoadAllCommandsInAppDomain(bool inDebug, List<Type> ignoredCommands)
        {
            var mapLoader = new MapLoader();

            var listOfCommands = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                                  from assemblyType in domainAssembly.GetTypes()
                                  where
                                         typeof(Command).IsAssignableFrom(assemblyType)
                                      && assemblyType.IsInterface == false
                                      && assemblyType.IsAbstract == false
                                  select assemblyType).ToList();

            var commands = listOfCommands.Select(f => (Command)Activator.CreateInstance(f)).OrderBy(f => f.OrderExecution).ToList();
            commands.RemoveAll(f => ignoredCommands.Contains(f.GetType()) || (!inDebug && f.OnlyInDebug));

            foreach (var command in commands)
                mapLoader.AddCommand(command);

            return mapLoader;
        }
    }
}

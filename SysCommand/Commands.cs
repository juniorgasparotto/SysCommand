using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;

namespace SysCommand
{
    public class Commands : IEnumerable<Command4>
    {
        private List<Command4> commands;

        public List<ActionMap> ActionsMaps { get; private set; }
        public List<ArgumentMap> GlobalMaps { get; private set; }

        public Command4 this[int index]
        {
            get
            {
                return commands[index];
            }
        }

        public void Add(Command4 command)
        {
            this.ActionsMaps.AddRange(CommandParser.GetActionsMapsFromType(command.GetType(), command.OnlyMethodsWithAttribute, command.UsePrefixInAllMethods, command.PrefixMethods));
            this.GlobalMaps.AddRange(CommandParser.GetArgumentsMapsFromProperties(command.GetType(), command.OnlyPropertiesWithAttribute));
            commands.Add(command);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.commands.GetEnumerator();
        }

        public IEnumerator<Command4> GetEnumerator()
        {
            return ((IEnumerable<Command4>)commands).GetEnumerator();
        }

        public static Commands LoadAllCommandsInAppDomain(bool inDebug, List<Type> ignoredCommands)
        {
            var commands = new Commands();

            var listOfCommands = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                                  from assemblyType in domainAssembly.GetTypes()
                                  where
                                         typeof(Command4).IsAssignableFrom(assemblyType)
                                      && assemblyType.IsInterface == false
                                      && assemblyType.IsAbstract == false
                                  select assemblyType).ToList();

            var commandsList = listOfCommands.Select(f => (Command4)Activator.CreateInstance(f)).OrderBy(f => f.OrderExecution).ToList();
            commandsList.RemoveAll(f => ignoredCommands.Contains(f.GetType()) || (!inDebug && f.OnlyInDebug));

            foreach (var command in commandsList)
                commands.Add(command);

            return commands;
        }

    }
}

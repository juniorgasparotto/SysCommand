using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;
using SysCommand.Parser;

namespace SysCommand
{
    public class DefaultMappingStrategy : IMappingStrategy
    {
        public CommandMap CreateMap(CommandBase command)
        {
            var mapCommand = new CommandMap(command);
            mapCommand.Methods.AddRange(CommandParser.GetActionsMapsFromSourceObject(command, command.OnlyMethodsWithAttribute, command.UsePrefixInAllMethods, command.PrefixMethods));
            mapCommand.Properties.AddRange(CommandParser.GetArgumentsMapsFromProperties(command, command.OnlyPropertiesWithAttribute));
            return mapCommand;
        }

        public IEnumerable<CommandMap> DoMappping(IEnumerable<CommandBase> commands)
        {
            foreach (var command in commands)
                yield return this.CreateMap(command);
        }
    }
}

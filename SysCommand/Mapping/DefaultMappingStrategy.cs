using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;
using SysCommand.Parsing;

namespace SysCommand
{
    public class DefaultMappingStrategy : IMappingStrategy
    {
        public CommandMap CreateMap(CommandBase command)
        {
            var mapCommand = new CommandMap(command);
            mapCommand.Methods.AddRange(ParserUtils.GetActionsMapsFromSourceObject(command, command.OnlyMethodsWithAttribute, command.UsePrefixInAllMethods, command.PrefixMethods));
            mapCommand.Properties.AddRange(ParserUtils.GetArgumentsMapsFromProperties(command, command.OnlyPropertiesWithAttribute));
            return mapCommand;
        }

        public IEnumerable<CommandMap> DoMappping(IEnumerable<CommandBase> commands)
        {
            foreach (var command in commands)
                yield return this.CreateMap(command);
        }
    }
}

using SysCommand.Mapping;
using SysCommand.Parsing;
using SysCommand.Helpers;
using System.Collections.Generic;

namespace SysCommand.DefaultExecutor
{
    public class CommandMapper
    {
        public ArgumentMapper ArgumentMapper { get; private set;  }
        public ActionMapper ActionMapper { get; private set; }

        public CommandMapper(ArgumentMapper argumentMapper, ActionMapper actionMapper)
        {
            this.ArgumentMapper = argumentMapper;
            this.ActionMapper = actionMapper;
        }

        public IEnumerable<CommandMap> Map(IEnumerable<CommandBase> commands)
        {
            foreach (var command in commands)
                yield return this.CreateMap(command);
        }

        private CommandMap CreateMap(CommandBase command)
        {
            var mapCommand = new CommandMap(command);
            mapCommand.Methods.AddRange(this.ActionMapper.Map(command, command.OnlyMethodsWithAttribute, command.UsePrefixInAllMethods, command.PrefixMethods));
            mapCommand.Properties.AddRange(this.ArgumentMapper.Map(command, command.OnlyPropertiesWithAttribute));
            return mapCommand;
        }
    }
}

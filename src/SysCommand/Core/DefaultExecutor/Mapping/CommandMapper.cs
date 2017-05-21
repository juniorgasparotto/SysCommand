using SysCommand.Mapping;
using System.Collections.Generic;

namespace SysCommand.DefaultExecutor
{
    /// <summary>
    /// Represent a mapper of list of CommandMap
    /// </summary>
    public class CommandMapper
    {
        public ArgumentMapper ArgumentMapper { get; private set;  }
        public ActionMapper ActionMapper { get; private set; }

        /// <summary>
        /// Initialize the mapper
        /// </summary>
        /// <param name="argumentMapper">Mapper of arguments</param>
        /// <param name="actionMapper">Mapper of actions</param>
        public CommandMapper(ArgumentMapper argumentMapper, ActionMapper actionMapper)
        {
            this.ArgumentMapper = argumentMapper;
            this.ActionMapper = actionMapper;
        }

        /// <summary>
        /// Create a map of commands
        /// </summary>
        /// <param name="commands">Commands to be mapped</param>
        /// <returns>List of CommandMap</returns>
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

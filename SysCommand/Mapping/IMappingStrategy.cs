using System.Collections.Generic;

namespace SysCommand.Mapping
{
    public interface IMappingStrategy
    {
        IEnumerable<CommandMap> DoMappping(IEnumerable<CommandBase> commands);
    }
}
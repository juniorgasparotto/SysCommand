using System.Collections.Generic;

namespace SysCommand
{
    public interface IMappingStrategy
    {
        IEnumerable<CommandMap> DoMappping(IEnumerable<CommandBase> commands);
    }
}
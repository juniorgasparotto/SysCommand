using System.Collections.Generic;

namespace SysCommand
{
    public interface IMapper
    {
        IEnumerable<CommandMap> CreateMap(IEnumerable<CommandBase> commands);
    }
}
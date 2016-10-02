using System.Collections.Generic;

namespace SysCommand
{
    public interface IExecutionStrategy
    {
        Map CreateMap(IEnumerable<Command> commands);
        ExecutionState Execute(string[] args, Map map, Result<IMember> result);
        Result<IMember> Parse(string[] args, Map map, bool enableMultiAction);
    }
}
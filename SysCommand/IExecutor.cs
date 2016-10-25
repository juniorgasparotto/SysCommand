using SysCommand.Execution;
using SysCommand.Mapping;
using SysCommand.Parsing;
using System;
using System.Collections.Generic;

namespace SysCommand
{
    public interface IExecutor
    {
        IEnumerable<CommandMap> GetMaps(IEnumerable<CommandBase> commands);
        ParseResult Parse(string[] args, IEnumerable<CommandMap> commandsMap, bool enableMultiAction);
        ExecutionResult Execute(ParseResult parseResult, Action<IMemberResult> onInvoke);
    }
}
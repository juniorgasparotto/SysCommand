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
        IEnumerable<ArgumentRaw> ParseRaw(string[] args, IEnumerable<CommandMap> commandsMap);
        ParseResult Parse(string[] args, IEnumerable<ArgumentRaw> argumentsRaw, IEnumerable<CommandMap> commandsMap, bool enableMultiAction);
        ExecutionResult Execute(ParseResult parseResult, Action<IMemberResult, ExecutionScope> onInvoke);
    }
}
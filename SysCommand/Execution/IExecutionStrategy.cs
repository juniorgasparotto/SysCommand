using SysCommand.Parsing;
using System;

namespace SysCommand.Execution
{
    public interface IExecutionStrategy
    {
        ExecutionResult Execute(ParseResult parseResult, Action<IMemberResult> onInvoke);
    }
}
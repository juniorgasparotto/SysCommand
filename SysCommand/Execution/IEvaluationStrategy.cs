using System;
using System.Collections.Generic;

namespace SysCommand
{
    public interface IEvaluationStrategy
    {
        Action<IMember> OnInvoke { get; set; }
        IEnumerable<CommandParseResult> Parse(string[] args, IEnumerable<CommandMap> maps, bool enableMultiAction);
        EvaluateState Evaluate(string[] args, IEnumerable<CommandMap> maps, IEnumerable<CommandParseResult> result);
    }
}
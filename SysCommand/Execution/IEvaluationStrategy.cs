using System;
using System.Collections.Generic;

namespace SysCommand
{
    public interface IEvaluationStrategy
    {
        Action<IMember> OnInvoke { get; set; }
        EvaluateState Evaluate(string[] args, IEnumerable<CommandMap> maps, Result<IMember> result);
        Result<IMember> Parse(string[] args, IEnumerable<CommandMap> maps, bool enableMultiAction);
    }
}
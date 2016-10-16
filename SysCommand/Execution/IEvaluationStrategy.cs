using System;
using System.Collections.Generic;

namespace SysCommand
{
    public interface IEvaluationStrategy
    {
        Action<IMember> OnInvoke { get; set; }
        ParseResult Parse(string[] args, IEnumerable<CommandMap> maps, bool enableMultiAction);
        EvaluateResult Evaluate(string[] args, IEnumerable<CommandMap> maps, ParseResult parseResult);
    }
}
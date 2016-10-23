using SysCommand.Parsing;
using System;
using System.Collections.Generic;

namespace SysCommand.Evaluation
{
    public interface IEvaluationStrategy
    {
        Action<IMember> OnInvoke { get; set; }
        ParseResult Parse(string[] args, IEnumerable<CommandMap> maps, bool enableMultiAction);
        EvaluateResult Evaluate(ParseResult parseResult);
    }
}
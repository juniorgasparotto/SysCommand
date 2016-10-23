using SysCommand.Mapping;
using SysCommand.Parsing;
using System;
using System.Collections.Generic;

namespace SysCommand.Evaluation
{
    public interface IEvaluationStrategy
    {
        EvaluateResult Evaluate(ParseResult parseResult, Action<IMemberResult> onInvoke);
    }
}
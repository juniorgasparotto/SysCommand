using System.Collections.Generic;

namespace SysCommand
{
    public interface IEvaluationStrategy
    {
        EvalState Eval(string[] args, IEnumerable<CommandMap> maps, Result<IMember> result);
        Result<IMember> Parse(string[] args, IEnumerable<CommandMap> maps, bool enableMultiAction);
    }
}
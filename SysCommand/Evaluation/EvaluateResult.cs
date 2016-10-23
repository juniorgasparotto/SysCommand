using SysCommand.Parsing;
using System.Collections.Generic;
using System.Linq;
using System;

namespace SysCommand.Evaluation
{
    public class EvaluateResult
    {
        public Result<IMember> Result { get; set; }
        public IEnumerable<CommandError> Errors { get; set; }
        public EvaluateState State { get; set; }

        public EvaluateResult()
        {

        }
    }
}
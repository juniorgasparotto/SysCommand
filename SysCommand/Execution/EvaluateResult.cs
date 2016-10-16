using SysCommand.Parser;
using System.Collections.Generic;
using System.Linq;
using System;

namespace SysCommand
{
    public class EvaluateResult
    {
        public Result<IMember> Result { get; internal set; }
        public EvaluateState State { get; internal set; }

        public EvaluateResult()
        {

        }
    }
}
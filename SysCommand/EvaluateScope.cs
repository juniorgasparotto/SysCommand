using System;
using System.Collections.Generic;

namespace SysCommand
{
    public class EvaluateScope
    {
        public string[] Args { get; internal set; }
        public IEnumerable<CommandMap> Maps { get; internal set; }
        public ParseResult ParseResult { get; internal set; }
        public EvaluateResult EvaluateResult { get; internal set; }
    }
}

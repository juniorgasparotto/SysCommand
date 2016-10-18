using System;
using System.Collections.Generic;

namespace SysCommand
{
    public class EvaluateScope
    {
        public string[] Args { get; set; }
        public IEnumerable<CommandMap> Maps { get; set; }
        public ParseResult ParseResult { get; set; }
        public EvaluateResult EvaluateResult { get; set; }
    }
}

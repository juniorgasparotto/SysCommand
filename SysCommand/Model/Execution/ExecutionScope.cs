using SysCommand.Parsing;

namespace SysCommand.Execution
{
    public class ExecutionScope
    {
        public ParseResult ParseResult { get; set; }
        public ExecutionResult ExecutionResult { get; set; }
    }
}

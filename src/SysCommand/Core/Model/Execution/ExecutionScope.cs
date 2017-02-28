using SysCommand.Parsing;

namespace SysCommand.Execution
{
    public class ExecutionScope
    {
        public ParseResult ParseResult { get; private set; }
        public ExecutionResult ExecutionResult { get; private set; }
        public bool IsStopped { get; private set; }

        public ExecutionScope(ParseResult parseResult, ExecutionResult executionResult)
        {
            this.ParseResult = parseResult;
            this.ExecutionResult = executionResult;
        }

        public void StopPropagation()
        {
            this.IsStopped = true;
        }
    }
}

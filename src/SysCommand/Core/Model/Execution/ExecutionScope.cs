using SysCommand.Parsing;

namespace SysCommand.Execution
{
    /// <summary>
    /// Scope of execution request
    /// </summary>
    public class ExecutionScope
    {
        /// <summary>
        /// Parse resume
        /// </summary>
        public ParseResult ParseResult { get; private set; }

        /// <summary>
        /// Execution resume
        /// </summary>
        public ExecutionResult ExecutionResult { get; private set; }

        /// <summary>
        /// Checks whether execution can continue.
        /// </summary>
        public bool IsStopped { get; private set; }

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="parseResult">Parse resume</param>
        /// <param name="executionResult">Execution resume</param>
        public ExecutionScope(ParseResult parseResult, ExecutionResult executionResult)
        {
            this.ParseResult = parseResult;
            this.ExecutionResult = executionResult;
        }

        /// <summary>
        /// Stop execution
        /// </summary>
        public void StopPropagation()
        {
            this.IsStopped = true;
        }
    }
}

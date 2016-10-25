using System.Collections.Generic;

namespace SysCommand.Execution
{
    public class ExecutionResult
    {
        public IEnumerable<IMemberResult> Results { get; set; }
        public IEnumerable<ExecutionError> Errors { get; set; }
        public ExecutionState State { get; set; }

        public ExecutionResult()
        {

        }
    }
}
using System.Collections.Generic;

namespace SysCommand.Execution
{
    /// <summary>
    /// Summary of execution
    /// </summary>
    public class ExecutionResult
    {
        /// <summary>
        /// All results if exists
        /// </summary>
        public IEnumerable<IMemberResult> Results { get; set; }

        /// <summary>
        /// All errors if exists
        /// </summary>
        public IEnumerable<ExecutionError> Errors { get; set; }

        /// <summary>
        /// Execution state
        /// </summary>
        public ExecutionState State { get; set; }
    }
}
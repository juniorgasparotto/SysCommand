using SysCommand.Parsing;
using System.Collections.Generic;

namespace SysCommand.Execution
{
    /// <summary>
    /// Summary of execution errors
    /// </summary>
    public class ExecutionError
    {
        /// <summary>
        /// Command that gave problem
        /// </summary>
        public CommandBase Command { get; set; }

        /// <summary>
        /// All invalid properties
        /// </summary>
        public IEnumerable<ArgumentParsed> PropertiesInvalid { get; set; }

        /// <summary>
        /// All invalid methods
        /// </summary>
        public IEnumerable<ActionParsed> MethodsInvalid { get; set; }
    }
}
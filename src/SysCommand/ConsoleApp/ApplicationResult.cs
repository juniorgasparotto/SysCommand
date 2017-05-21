using System.Collections.Generic;
using SysCommand.Execution;
using SysCommand.Parsing;

namespace SysCommand.ConsoleApp
{
    /// <summary>
    /// Represent the resume of all execution
    /// </summary>
    public sealed class ApplicationResult
    {
        /// <summary>
        /// App reference
        /// </summary>
        public App App { get; internal set; }

        /// <summary>
        /// Current arguments. These arguments can be changed by "RedirectResult"
        /// </summary>
        public string[] Args { get; internal set; }

        /// <summary>
        /// Original arguments
        /// </summary>
        public string[] ArgsOriginal { get; internal set; }

        /// <summary>
        /// List of ArgumentRaw
        /// </summary>
        public IEnumerable<ArgumentRaw> ArgumentsRaw { get; internal set; }

        /// <summary>
        /// Resume of parse
        /// </summary>
        public ParseResult ParseResult { get; internal set; }

        /// <summary>
        /// Resume of execution
        /// </summary>
        public ExecutionResult ExecutionResult { get; internal set; }
        internal ApplicationResult() { }
    }
}
using SysCommand.Execution;
using SysCommand.Parsing;

namespace SysCommand.ConsoleApp
{
    public sealed class ApplicationResult
    {
        public App App { get; internal set; }
        public string[] Args { get; internal set; }
        public string[] ArgsOriginal { get; internal set; }
        public ParseResult ParseResult { get; internal set; }
        public ExecutionResult ExecutionResult { get; internal set; }
        internal ApplicationResult() { }
    }
}
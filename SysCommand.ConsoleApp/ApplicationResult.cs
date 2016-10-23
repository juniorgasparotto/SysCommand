using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using System.Reflection;
using SysCommand.Evaluation;
using SysCommand.Parsing;

namespace SysCommand.ConsoleApp
{
    public sealed class ApplicationResult
    {
        public App App { get; internal set; }
        public string[] Args { get; internal set; }
        public string[] ArgsOriginal { get; internal set; }
        public ParseResult ParseResult { get; internal set; }
        public EvaluateResult EvaluateResult { get; internal set; }
        internal ApplicationResult() { }
    }
}
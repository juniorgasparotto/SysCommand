using System;
using System.Reflection;

namespace SysCommand.ConsoleApp.View.TemplatesGenerator.Razor
{
    internal class CSharpCompilerResult
    {
        public Assembly Assembly { get; set; }
        public Type CompiledType { get; set; }
        public CSharpCompilerException ProcessingException { get; set; }
    }
}
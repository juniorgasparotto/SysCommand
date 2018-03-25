using System;
using System.Collections.Generic;

namespace SysCommand.ConsoleApp.View.TemplatesGenerator.Razor
{
    internal class RazorCompilerException : Exception
    {
        public string GeneratedCode { get; private set; }
        public IEnumerable<string> Messages { get; private set; }

        public RazorCompilerException(IEnumerable<string> messages, string generatedCode)
            : base(FormatMessage(messages))
        {
            Messages = messages ?? throw new ArgumentNullException(nameof(messages));
            GeneratedCode = generatedCode;
        }

        public override string Message
        {
            get
            {
                return string.Format("Razor parsing error: {0}", FormatMessage(Messages));
            }
        }

        private static string FormatMessage(IEnumerable<string> messages)
        {
            return String.Join(Environment.NewLine, messages);
        }
    }
}
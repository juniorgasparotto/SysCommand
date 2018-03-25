using System;
using System.Collections.Generic;

namespace SysCommand.ConsoleApp.View.TemplatesGenerator.Razor
{
    internal class CSharpCompilerException : Exception
    {
        public CSharpCompilerException(IEnumerable<string> messages)
            : base(FormatMessage(messages))
        {
            if (messages == null)
            {
                throw new ArgumentNullException(nameof(messages));
            }

            Messages = messages;
        }

        public string GeneratedCode { get; private set; }

        public IEnumerable<string> Messages { get; private set; }

        public override string Message
        {
            get
            {
                return string.Format("CSharp compiler error: {0}", FormatMessage(Messages));
            }
        }

        private static string FormatMessage(IEnumerable<string> messages)
        {
            return String.Join(Environment.NewLine, messages);
        }
    }
}
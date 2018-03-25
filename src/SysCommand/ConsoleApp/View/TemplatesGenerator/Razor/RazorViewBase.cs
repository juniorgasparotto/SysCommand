using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SysCommand.ConsoleApp.View.TemplatesGenerator.Razor
{
    public abstract class RazorViewBase
    {
        private static readonly Encoding UTF8NoBOM = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);
        private MemoryStream memoryStream;
        private StreamWriter output;

        public abstract Task ExecuteAsync();

        public RazorViewBase()
        {
            this.memoryStream = new MemoryStream();
            this.output = new StreamWriter(memoryStream, UTF8NoBOM, 4096, true);
        }

        protected void Write(object value)
        {
            WriteTo(output, value?.ToString());
        }

        protected void Write(string value)
        {
            WriteTo(output, value);
        }

        protected void WriteTo(TextWriter writer, object value)
        {
            WriteLiteralTo(writer, value.ToString());
        }

        protected void WriteTo(TextWriter writer, string value)
        {
            WriteLiteralTo(writer, value);
        }

        protected void WriteLiteral(string value)
        {
            WriteLiteralTo(output, value);
        }

        protected void WriteLiteralTo(TextWriter writer, object value)
        {
            WriteLiteralTo(writer, Convert.ToString(value, CultureInfo.InvariantCulture));
        }

        protected void WriteLiteralTo(TextWriter writer, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                writer.Write(value);
            }
        }
        
        public override string ToString()
        {
            output.Flush();
            memoryStream.Seek(0, SeekOrigin.Begin);
            return new StreamReader(memoryStream).ReadToEnd();
        }
    }
}
using Microsoft.AspNetCore.Razor.Language;
using System.IO;
using System.Text;

namespace SysCommand.ConsoleApp.View.TemplatesGenerator.Razor
{
    internal class ContentRazorProjectItem : RazorProjectItem
    {
        public override string BasePath { get; }
        public override string FilePath { get; }
        public override string PhysicalPath { get; }
        public override bool Exists => true;
        public string Content { get; set; }

        public ContentRazorProjectItem(string content)
        {
            Content = content;
            FilePath = Path.GetRandomFileName();
            BasePath = "/";
            PhysicalPath = Path.Combine(BasePath, FilePath);
        }

        public override Stream Read()
        {
            // Act like a file and have a UTF8 BOM.
            var preamble = Encoding.UTF8.GetPreamble();
            var contentBytes = Encoding.UTF8.GetBytes(Content);
            var buffer = new byte[preamble.Length + contentBytes.Length];
            preamble.CopyTo(buffer, 0);
            contentBytes.CopyTo(buffer, preamble.Length);

            var stream = new MemoryStream(buffer);

            return stream;
        }
    }
}

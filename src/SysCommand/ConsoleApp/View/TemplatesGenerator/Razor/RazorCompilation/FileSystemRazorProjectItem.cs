using Microsoft.AspNetCore.Razor.Language;
using System.IO;
using System.Text;

namespace SysCommand.ConsoleApp.View.TemplatesGenerator.Razor
{
    internal class FileSystemRazorProjectItem : RazorProjectItem
    {
        private readonly RazorProjectItem _source;

        public override string BasePath => _source.BasePath;
        public override string FilePath => _source.FilePath;
        public override bool Exists => _source.Exists;

        // Mask the full name since we don't want a developer's local file paths to be commited.
        public override string PhysicalPath => _source.FileName;

        public FileSystemRazorProjectItem(RazorProjectItem item)
        {
            _source = item;
        }

        public override Stream Read()
        {
            var processedContent = ReadFile();
            return new MemoryStream(Encoding.UTF8.GetBytes(processedContent));
        }

        private string ReadFile()
        {
            var cshtmlContent = File.ReadAllText(_source.PhysicalPath);
            return cshtmlContent;
        }
    }
}

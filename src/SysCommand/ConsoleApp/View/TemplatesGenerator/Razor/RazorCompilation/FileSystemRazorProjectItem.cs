using Microsoft.AspNetCore.Razor.Language;
using System.IO;
using System.Text;

namespace SysCommand.ConsoleApp.View.TemplatesGenerator.Razor
{
    internal class FileSystemRazorProjectItem : RazorProjectItem
    {
        /// <summary>
        /// Initializes a new instance of <see cref="FileSystemRazorProjectItem"/>.
        /// </summary>
        /// <param name="basePath">The base path.</param>
        /// <param name="path">The path.</param>
        /// <param name="file">The <see cref="FileInfo"/>.</param>
        public FileSystemRazorProjectItem(string basePath, string path, FileInfo file)
        {
            BasePath = basePath;
            FilePath = path;
            File = file;
        }

        public FileInfo File { get; }

        public override string BasePath { get; }

        public override string FilePath { get; }

        public override bool Exists => File.Exists;

        public override string PhysicalPath => File.FullName;

        public override Stream Read() => new FileStream(PhysicalPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
    }
}

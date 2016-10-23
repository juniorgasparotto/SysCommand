using System;

namespace SysCommand.ConsoleApp
{
    public class ObjectFileAttribute : Attribute
    {
        public string FileName { get; set; }
        public string Folder { get; set; }
    }
}

using System;

namespace SysCommand.ConsoleApp.Files
{
    public class ObjectFileAttribute : Attribute
    {
        public string FileName { get; set; }
    }
}

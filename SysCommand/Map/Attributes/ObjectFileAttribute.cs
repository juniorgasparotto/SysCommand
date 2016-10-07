using System;

namespace SysCommand
{
    public class ObjectFileAttribute : Attribute
    {
        public string FileName { get; set; }
        public string Folder { get; set; }
    }
}

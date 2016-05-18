using System;

namespace SysCommand
{
    public class ObjectFileClassAttribute : Attribute
    {
        public string FileName { get; set; }
        public string Folder { get; set; }
    }
}

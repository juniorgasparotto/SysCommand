using System;

namespace SysCommand
{
    public class ConfigClassAttribute : Attribute
    {
        public string FileName { get; set; }
    }
}

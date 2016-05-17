using System;

namespace SysCommand
{
    public class CommandClassAttribute : Attribute
    {
        public int OrderExecution { get; set; }
    }
}

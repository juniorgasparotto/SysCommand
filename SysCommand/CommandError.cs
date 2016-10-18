using SysCommand.Parser;
using System.Collections.Generic;

namespace SysCommand
{
    public class CommandError
    {
        public CommandBase Command { get; set; }
        public List<ArgumentMapped> Properties { get; private set; }
        public List<ActionMapped> Methods { get; private set; }

        public CommandError()
        {
            this.Properties = new List<ArgumentMapped>();
            this.Methods = new List<ActionMapped>();
        }
    }
}
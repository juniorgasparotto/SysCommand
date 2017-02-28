using System.Collections.Generic;

namespace SysCommand.Mapping
{
    public class CommandMap
    {
        public CommandBase Command { get; private set; }
        public List<ActionMap> Methods { get; private set; }
        public List<ArgumentMap> Properties { get; private set; }

        internal CommandMap(CommandBase command)
        {
            this.Command = command;
            this.Methods = new List<ActionMap>();
            this.Properties = new List<ArgumentMap>();
        }
    }
}

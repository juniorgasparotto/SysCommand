using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;

namespace SysCommand
{
    public class MapCommand
    {
        public Command Command { get; private set; }
        public List<ActionMap> Methods { get; private set; }
        public List<ArgumentMap> Properties { get; private set; }

        internal MapCommand(Command command)
        {
            this.Command = command;
            this.Methods = new List<ActionMap>();
            this.Properties = new List<ArgumentMap>();
        }
    }
}

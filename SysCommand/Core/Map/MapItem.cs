using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;
using SysCommand.Parser;

namespace SysCommand
{
    public class MapItem
    {
        public Command Command { get; private set; }
        public List<ActionMap> Methods { get; private set; }
        public List<ArgumentMap> Properties { get; private set; }

        internal MapItem(Command command)
        {
            this.Command = command;
            this.Methods = new List<ActionMap>();
            this.Properties = new List<ArgumentMap>();
        }
    }
}

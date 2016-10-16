using SysCommand.Parser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SysCommand.ConsoleApp
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
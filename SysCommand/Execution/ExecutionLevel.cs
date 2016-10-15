using SysCommand.Parser;
using System.Collections.Generic;
using System.Linq;
using System;

namespace SysCommand
{
    public class ExecutionLevel
    {
        public List<CommandParse> CommandsParse { get; set; }
        public int Level { get; set; }

        public ExecutionLevel()
        {
            this.CommandsParse = new List<CommandParse>();
        }
    }
    
}
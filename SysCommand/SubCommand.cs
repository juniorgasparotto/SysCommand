using System.Collections.Generic;
using System.Linq;
using System;
using Fclp;
using System.Linq.Expressions;
using System.IO;

namespace SysCommand
{
    public class SubCommand
    {
        public string Name { get; set; }
        public int Position { get; set; }
        public List<string> Arguments { get; set; }

        public SubCommand()
        {
            this.Arguments = new List<string>();
        }
    }
}

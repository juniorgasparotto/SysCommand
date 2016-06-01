using System.Collections.Generic;
using System.Linq;
using System;
using Fclp;
using System.Linq.Expressions;
using System.IO;

namespace SysCommand
{
    public class RequestAction
    {
        private List<string> arguments;

        public string Name { get; set; }
        public int Position { get; set; }
        public string[] Arguments { get { return arguments.ToArray(); } }
        public Dictionary<string, string> Get { get; set; }
        
        public RequestAction()
        {
            this.arguments = new List<string>();
            this.Get = new Dictionary<string, string>();
        }

        public void Add(string arg)
        {
            arguments.Add(arg);
        }
    }
}

using Fclp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SysCommand
{
    public class ActionAttribute : Attribute
    {
        public string Name { get; set; }
        public bool UsePrefix { get; set; }
        public bool Ignore { get; set; }
        public bool IsDefault { get; set; }
        public bool EnablePositionalArgs { get; set; }

        public ActionAttribute()
        {
            this.UsePrefix = true;
            this.EnablePositionalArgs = true;
        }
    }
}

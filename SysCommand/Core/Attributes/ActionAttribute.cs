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
        public bool CanAddPrefix { get; set; }
        //public bool Ignore { get; set; }
        public bool IsDefault { get; set; }

        public ActionAttribute()
        {
            this.CanAddPrefix = true;
        }
    }
}

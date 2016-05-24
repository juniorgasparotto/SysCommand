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
        public string ActionName { get; set; }
    }
}

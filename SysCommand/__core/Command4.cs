using Fclp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SysCommand
{
    public abstract class Command4
    {
        public int OrderExecution { get; set; }
        public bool OnlyInDebug { get; set; }
        public bool UsePrefixInAllMethods { get; set; }
        public string PrefixMethods { get; set; }
        public bool OnlyMethodsWithAttribute { get; set; }
        public bool OnlyPropertiesWithAttribute { get; set; }
    }
}

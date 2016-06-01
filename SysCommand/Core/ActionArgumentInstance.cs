using Fclp;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace SysCommand
{
    public class ActionArgumentInstance
    {
        public ParameterInfo ParameterInfo { get; set; }
        public ArgumentAttribute ArgumentAttribute { get; set; }
        public ActionInstance Action { get; set; }
        public string LongName { get; set; }
        public char ShortName { get; set; }
        public string Help { get; set; }
        public object DefaultValue { get; set; }
        public string Name { get; set; }
        public bool HasDefault { get; set; }
        public object Value { get; set; }
        public bool HasParsed { get; set; }
    }
}
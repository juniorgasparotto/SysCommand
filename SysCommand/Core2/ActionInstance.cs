using Fclp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SysCommand
{
    public class ActionInstance
    {
        public string Name { get; set; }
        public List<ActionArgumentInstance> ActionArguments { get; set; }
        public Dictionary<ParameterInfo, object> ParametersParseds { get; set; }

        public RequestAction RequestAction { get; set; }
        public CommandAction Object { get; set; }
        public ActionAttribute Attribute { get; set; }
        public MethodInfo MethodInfo { get; set; }
        public FluentCommandLineParser Parser { get; set; }
        public ICommandLineParserResult Result { get; set; }
        public bool HasParsed { get; set; }
        public bool IsDefault { get; set; }

        public ActionInstance()
        {
            this.ParametersParseds = new Dictionary<ParameterInfo, object>();
            this.ActionArguments = new List<ActionArgumentInstance>();
        }
    }
}

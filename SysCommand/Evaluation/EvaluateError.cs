using SysCommand.Mapping;
using SysCommand.Parsing;
using System.Collections.Generic;

namespace SysCommand.Evaluation
{
    public class EvaluateError
    {
        public CommandBase Command { get; set; }
        public IEnumerable<ArgumentMapped> PropertiesInvalid { get; set; }
        public IEnumerable<ActionMapped> MethodsInvalid { get; set; }

        public EvaluateError()
        {
            
        }
    }
}
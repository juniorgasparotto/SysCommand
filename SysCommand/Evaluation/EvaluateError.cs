using SysCommand.Mapping;
using SysCommand.Parsing;
using System.Collections.Generic;

namespace SysCommand.Evaluation
{
    public class EvaluateError
    {
        public CommandBase Command { get; set; }
        public IEnumerable<ArgumentParsed> PropertiesInvalid { get; set; }
        public IEnumerable<ActionParsed> MethodsInvalid { get; set; }

        public EvaluateError()
        {
            
        }
    }
}
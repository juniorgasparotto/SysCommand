using SysCommand.Mapping;
using System.Collections.Generic;

namespace SysCommand.Parsing
{
    public class ActionParsed
    {
        private List<ArgumentRaw> argumentsRaw;

        public string Name { get; private set; }
        public ActionMap ActionMap { get; private set; }
        public ArgumentRaw ArgumentRawOfAction { get; private set; }
        public IEnumerable<ArgumentParsed> Arguments { get; set; }
        public IEnumerable<ArgumentParsed> ArgumentsExtras { get; internal set; }
        public int Level { get; private set; }
        public ActionParsedState ParsingStates { get; set; }

        public ActionParsed(string name, ActionMap actionMap, ArgumentRaw argumentRawOfAction, int level)
        {
            this.Name = name;
            this.ArgumentRawOfAction = argumentRawOfAction;
            this.ActionMap = actionMap;
            this.argumentsRaw = new List<ArgumentRaw>();
            this.Level = level;
        }

        public void AddArgumentRaw(ArgumentRaw argumentRaw)
        {
            this.argumentsRaw.Add(argumentRaw);
        }

        public IEnumerable<ArgumentRaw> GetArgumentsRaw()
        {
            return this.argumentsRaw;
        }

        public override string ToString()
        {
            return "[" + this.Name + "]";
        }
    }
}

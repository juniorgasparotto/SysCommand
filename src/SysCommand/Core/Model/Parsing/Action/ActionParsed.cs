using SysCommand.Mapping;
using System.Collections.Generic;

namespace SysCommand.Parsing
{
    /// <summary>
    /// Represents a parsed action
    /// </summary>
    public class ActionParsed
    {
        private List<ArgumentRaw> argumentsRaw;

        /// <summary>
        /// Action name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Action map
        /// </summary>
        public ActionMap ActionMap { get; private set; }

        /// <summary>
        /// ArgumentRaw where the name of this action was found.
        /// </summary>
        public ArgumentRaw ArgumentRawOfAction { get; private set; }

        /// <summary>
        /// List of parsed arguments
        /// </summary>
        public IEnumerable<ArgumentParsed> Arguments { get; set; }

        /// <summary>
        /// Extra arguments (Arguments that do not belong to this action)
        /// </summary>
        public IEnumerable<ArgumentParsed> ArgumentsExtras { get; internal set; }

        /// <summary>
        /// Position (between actions) where this action was found on the command line
        /// </summary>
        public int Level { get; private set; }

        /// <summary>
        /// Action state
        /// </summary>
        public ActionParsedState ParsingStates { get; set; }

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="name">Action name</param>
        /// <param name="actionMap">Action map</param>
        /// <param name="argumentRawOfAction">ArgumentRaw where the name of this action was found.</param>
        /// <param name="level">Position (between actions) where this action was found on the command line</param>
        public ActionParsed(string name, ActionMap actionMap, ArgumentRaw argumentRawOfAction, int level)
        {
            this.Name = name;
            this.ArgumentRawOfAction = argumentRawOfAction;
            this.ActionMap = actionMap;
            this.argumentsRaw = new List<ArgumentRaw>();
            this.Level = level;
        }

        /// <summary>
        /// Add a argument raw
        /// </summary>
        /// <param name="argumentRaw">Argument raw</param>
        public void AddArgumentRaw(ArgumentRaw argumentRaw)
        {
            this.argumentsRaw.Add(argumentRaw);
        }
        
        /// <summary>
        /// Get all arguments raw
        /// </summary>
        /// <returns></returns>
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

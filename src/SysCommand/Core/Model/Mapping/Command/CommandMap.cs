using System.Collections.Generic;

namespace SysCommand.Mapping
{
    /// <summary>
    /// Map of command
    /// </summary>
    public class CommandMap
    {
        /// <summary>
        /// Command reference
        /// </summary>
        public CommandBase Command { get; private set; }

        /// <summary>
        /// List of methods
        /// </summary>
        public List<ActionMap> Methods { get; private set; }

        /// <summary>
        /// List of properties
        /// </summary>
        public List<ArgumentMap> Properties { get; private set; }

        internal CommandMap(CommandBase command)
        {
            this.Command = command;
            this.Methods = new List<ActionMap>();
            this.Properties = new List<ArgumentMap>();
        }
    }
}

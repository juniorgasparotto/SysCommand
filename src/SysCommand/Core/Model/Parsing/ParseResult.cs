using SysCommand.Mapping;
using System.Collections.Generic;
using System.Linq;

namespace SysCommand.Parsing
{
    /// <summary>
    /// Resume of parse
    /// </summary>
    public class ParseResult
    {
        private List<Level> levels;

        /// <summary>
        /// List of levels
        /// </summary>
        public IEnumerable<Level> Levels { get { return levels; } }

        /// <summary>
        /// Original args
        /// </summary>
        public string[] Args { get; internal set; }

        /// <summary>
        /// All maps
        /// </summary>
        public IEnumerable<CommandMap> Maps { get; internal set; }

        /// <summary>
        /// Determines whether multiple actions can be taken.
        /// </summary>
        public bool EnableMultiAction { get; internal set; }

        /// <summary>
        /// Initialize
        /// </summary>
        public ParseResult()
        {
            this.levels = new List<Level>();
        }

        /// <summary>
        /// Add a new level
        /// </summary>
        /// <param name="level"></param>
        public void Add(Level level)
        {
            this.levels.Add(level);
        }

        /// <summary>
        /// Insert a new level
        /// </summary>
        /// <param name="index">Position to be inserted</param>
        /// <param name="level">Level instance</param>
        public void Insert(int index , Level level)
        {
            this.levels.Insert(index, level);
        }

        /// <summary>
        /// Remove a level
        /// </summary>
        /// <param name="level">Level to be removed</param>
        public void Remove(Level level)
        {
            this.levels.Remove(level);
        }

        #region sub-classes

        /// <summary>
        /// Represent a level in command line
        /// </summary>
        public class Level
        {
            private List<CommandParse> commands;

            /// <summary>
            /// List of CommandParse
            /// </summary>
            public IEnumerable<CommandParse> Commands { get { return commands; } }
            public int LevelNumber { get; set; }

            /// <summary>
            /// Initialize
            /// </summary>
            public Level()
            {
                this.commands = new List<CommandParse>();
            }

            /// <summary>
            /// Add a new CommandParse
            /// </summary>
            /// <param name="command">CommandParse instance</param>
            public void Add(CommandParse command)
            {
                this.commands.Add(command);
            }

            /// <summary>
            /// Add a list of CommandParse
            /// </summary>
            /// <param name="commands">List of CommandParse</param>
            public void Add(IEnumerable<CommandParse> commands)
            {
                this.commands.AddRange(commands);
            }
        }

        /// <summary>
        /// Command resume in parse
        /// </summary>
        public class CommandParse
        {
            private List<ActionParsed> methods;
            private List<ArgumentParsed> properties;
            private List<ActionParsed> methodsInvalid;
            private List<ArgumentParsed> propertiesInvalid;

            /// <summary>
            /// Command reference
            /// </summary>
            public CommandBase Command { get; set; }

            /// <summary>
            /// List of valid methods
            /// </summary>
            public IEnumerable<ActionParsed> Methods { get { return methods; } }

            /// <summary>
            /// List of valid properties
            /// </summary>
            public IEnumerable<ArgumentParsed> Properties { get { return properties; } }

            /// <summary>
            /// List of invalid methods
            /// </summary>
            public IEnumerable<ActionParsed> MethodsInvalid { get { return methodsInvalid; } }

            /// <summary>
            /// List of invalid properties
            /// </summary>
            public IEnumerable<ArgumentParsed> PropertiesInvalid { get { return propertiesInvalid; } }

            /// <summary>
            /// Verify if exists errors
            /// </summary>
            public bool HasError
            {
                get
                {
                    return this.methodsInvalid.Any() || this.propertiesInvalid.Any();
                }
            }

            /// <summary>
            /// Verify if all is valid
            /// </summary>
            public bool IsValid
            {
                get
                {
                    return !HasError && (this.methods.Any() || this.properties.Any());
                }
            }

            /// <summary>
            /// Check if exists required arguments
            /// </summary>
            public bool HasAnyArgumentRequired
            {
                get
                {
                    return this.propertiesInvalid.Any(f => f.ParsingStates.HasFlag(ArgumentParsedState.ArgumentIsRequired));
                }
            }

            /// <summary>
            /// Initialize
            /// </summary>
            public CommandParse()
            {
                this.methods = new List<ActionParsed>();
                this.methodsInvalid = new List<ActionParsed>();
                this.propertiesInvalid = new List<ArgumentParsed>();
                this.properties = new List<ArgumentParsed>();
            }

            /// <summary>
            /// Add a valid method
            /// </summary>
            /// <param name="method">Valid method</param>
            public void AddMethod(ActionParsed method)
            {
                this.methods.Add(method);
            }

            /// <summary>
            /// Add a list of valid method
            /// </summary>
            /// <param name="methods">List of valid method</param>
            public void AddMethods(IEnumerable<ActionParsed> methods)
            {
                this.methods.AddRange(methods);
            }

            /// <summary>
            /// Remove a valid method
            /// </summary>
            /// <param name="method">Valid method</param>
            public void RemoveMethod(ActionParsed method)
            {
                this.methods.Remove(method);
            }

            /// <summary>
            /// Add a invalid method
            /// </summary>
            /// <param name="method">Invalid method</param>
            public void AddMethodInvalid(ActionParsed method)
            {
                this.methodsInvalid.Add(method);
            }

            /// <summary>
            /// Add a list of invalid methods
            /// </summary>
            /// <param name="method">Invalid methods</param>
            public void AddMethodsInvalid(IEnumerable<ActionParsed> method)
            {
                this.methodsInvalid.AddRange(method);
            }

            /// <summary>
            /// Remove a invalid method
            /// </summary>
            /// <param name="method"></param>
            public void RemoveInvalidMethod(ActionParsed method)
            {
                this.methodsInvalid.Remove(method);
            }

            /// <summary>
            /// Add a valid property
            /// </summary>
            /// <param name="property">Valid property</param>
            public void AddProperty(ArgumentParsed property)
            {
                this.properties.Add(property);
            }

            /// <summary>
            /// Add a list of valid properties
            /// </summary>
            /// <param name="properties">List of valid properties</param>
            public void AddProperties(IEnumerable<ArgumentParsed> properties)
            {
                this.properties.AddRange(properties);
            }

            /// <summary>
            /// Remove a valid property
            /// </summary>
            /// <param name="property">Valid property</param>
            public void RemoveProperty(ArgumentParsed property)
            {
                this.properties.Remove(property);
            }

            /// <summary>
            /// Add a invalid property
            /// </summary>
            /// <param name="property">Invalid property</param>
            public void AddPropertyInvalid(ArgumentParsed property)
            {
                this.propertiesInvalid.Add(property);
            }

            /// <summary>
            /// Add a list of invalid properties
            /// </summary>
            /// <param name="properties">Invalid properties</param>
            public void AddPropertiesInvalid(IEnumerable<ArgumentParsed> properties)
            {
                this.propertiesInvalid.AddRange(properties);
            }

            /// <summary>
            /// Remove a invalid property
            /// </summary>
            /// <param name="property">Invalid property</param>
            public void RemovePropertyInvalid(ArgumentParsed property)
            {
                this.propertiesInvalid.Remove(property);
            }

        }

        #endregion
    }
}
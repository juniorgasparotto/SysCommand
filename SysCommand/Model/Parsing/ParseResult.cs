using SysCommand.Mapping;
using System.Collections.Generic;
using System.Linq;

namespace SysCommand.Parsing
{
    public class ParseResult
    {
        private List<Level> levels;
        public IEnumerable<Level> Levels { get { return levels; } }

        public string[] Args { get; internal set; }
        public IEnumerable<CommandMap> Maps { get; internal set; }
        public bool EnableMultiAction { get; internal set; }

        public ParseResult()
        {
            this.levels = new List<Level>();
        }

        public void Add(Level level)
        {
            this.levels.Add(level);
        }

        public void Insert(int index , Level level)
        {
            this.levels.Insert(index, level);
        }

        public void Remove(Level level)
        {
            this.levels.Remove(level);
        }

        #region sub-classes

        public class Level
        {
            private List<CommandParse> commands;
            public IEnumerable<CommandParse> Commands { get { return commands; } }
            public int LevelNumber { get; set; }

            public Level()
            {
                this.commands = new List<CommandParse>();
            }

            public void Add(CommandParse command)
            {
                this.commands.Add(command);
            }

            public void Add(IEnumerable<CommandParse> commands)
            {
                this.commands.AddRange(commands);
            }
        }

        public class CommandParse
        {
            private List<ActionParsed> methods;
            private List<ArgumentParsed> properties;
            private List<ActionParsed> methodsInvalid;
            private List<ArgumentParsed> propertiesInvalid;

            public CommandBase Command { get; set; }
            public IEnumerable<ActionParsed> Methods { get { return methods; } }
            public IEnumerable<ArgumentParsed> Properties { get { return properties; } }
            public IEnumerable<ActionParsed> MethodsInvalid { get { return methodsInvalid; } }
            public IEnumerable<ArgumentParsed> PropertiesInvalid { get { return propertiesInvalid; } }

            public bool HasError
            {
                get
                {
                    return this.methodsInvalid.Any() || this.propertiesInvalid.Any();
                }
            }

            public bool IsValid
            {
                get
                {
                    return !HasError && (this.methods.Any() || this.properties.Any());
                }
            }

            public bool HasAnyArgumentRequired
            {
                get
                {
                    return this.propertiesInvalid.Any(f => f.ParsingStates.HasFlag(ArgumentParsedState.ArgumentIsRequired));
                }
            }

            public CommandParse()
            {
                this.methods = new List<ActionParsed>();
                this.methodsInvalid = new List<ActionParsed>();
                this.propertiesInvalid = new List<ArgumentParsed>();
                this.properties = new List<ArgumentParsed>();
            }

            public void AddMethod(ActionParsed method)
            {
                this.methods.Add(method);
            }

            public void AddMethods(IEnumerable<ActionParsed> methods)
            {
                this.methods.AddRange(methods);
            }


            public void RemoveMethod(ActionParsed method)
            {
                this.methods.Remove(method);
            }
            
            public void AddMethodInvalid(ActionParsed method)
            {
                this.methodsInvalid.Add(method);
            }

            public void AddMethodsInvalid(IEnumerable<ActionParsed> method)
            {
                this.methodsInvalid.AddRange(method);
            }

            public void RemoveInvalidMethod(ActionParsed method)
            {
                this.methodsInvalid.Remove(method);
            }

            public void AddProperty(ArgumentParsed property)
            {
                this.properties.Add(property);
            }

            public void AddProperties(IEnumerable<ArgumentParsed> properties)
            {
                this.properties.AddRange(properties);
            }

            public void RemoveProperty(ArgumentParsed property)
            {
                this.properties.Remove(property);
            }

            public void AddPropertyInvalid(ArgumentParsed property)
            {
                this.propertiesInvalid.Add(property);
            }

            public void AddPropertiesInvalid(IEnumerable<ArgumentParsed> properties)
            {
                this.propertiesInvalid.AddRange(properties);
            }

            public void RemovePropertyInvalid(ArgumentParsed property)
            {
                this.propertiesInvalid.Remove(property);
            }

        }

        #endregion
    }
}
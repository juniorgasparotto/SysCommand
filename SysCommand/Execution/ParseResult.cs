using SysCommand.Parser;
using System.Collections.Generic;
using System.Linq;
using System;

namespace SysCommand
{
    public class ParseResult
    {
        private List<Level> levels;
        public IEnumerable<Level> Levels { get { return levels; } }

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
            private List<ActionMapped> methods;
            private List<ArgumentMapped> properties;
            private List<ActionMapped> methodsInvalid;
            private List<ArgumentMapped> propertiesInvalid;

            public CommandBase Command { get; set; }
            public IEnumerable<ActionMapped> Methods { get { return methods; } }
            public IEnumerable<ArgumentMapped> Properties { get { return properties; } }
            public IEnumerable<ActionMapped> MethodsInvalid { get { return methodsInvalid; } }
            public IEnumerable<ArgumentMapped> PropertiesInvalid { get { return propertiesInvalid; } }

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
                    return this.propertiesInvalid.Any(f => f.MappingStates.HasFlag(ArgumentMappingState.ArgumentIsRequired));
                }
            }

            public CommandParse()
            {
                this.methods = new List<ActionMapped>();
                this.methodsInvalid = new List<ActionMapped>();
                this.propertiesInvalid = new List<ArgumentMapped>();
                this.properties = new List<ArgumentMapped>();
            }

            public void AddMethod(ActionMapped method)
            {
                this.methods.Add(method);
            }

            public void AddMethods(IEnumerable<ActionMapped> methods)
            {
                this.methods.AddRange(methods);
            }

            public void AddMethodInvalid(ActionMapped method)
            {
                this.methodsInvalid.Add(method);
            }

            public void AddMethodsInvalid(IEnumerable<ActionMapped> method)
            {
                this.methodsInvalid.AddRange(method);
            }

            public void AddProperty(ArgumentMapped property)
            {
                this.properties.Add(property);
            }

            public void AddProperties(IEnumerable<ArgumentMapped> properties)
            {
                this.properties.AddRange(properties);
            }

            public void AddPropertyInvalid(ArgumentMapped property)
            {
                this.propertiesInvalid.Add(property);
            }

            public void AddPropertiesInvalid(IEnumerable<ArgumentMapped> properties)
            {
                this.propertiesInvalid.AddRange(properties);
            }
        }

        #endregion
    }
}
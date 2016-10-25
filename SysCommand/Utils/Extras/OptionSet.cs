using System.Collections.Generic;
using System;
using System.Linq;
using SysCommand.Parsing;
using SysCommand.Mapping;
using System.Reflection;
using SysCommand.DefaultExecutor;

namespace SysCommand.Utils.Extras
{
    public sealed class OptionSet
    {
        private List<ArgumentMap> argumentsMap = new List<ArgumentMap>();
        private ArgumentParser argumentParser;
        private ArgumentRawParser argumentRawParser;

        public IEnumerable<ArgumentParsed> ArgumentsValid { get; private set; }
        public IEnumerable<ArgumentParsed> ArgumentsInvalid { get; private set; }
        public bool HasError { get { return ArgumentsInvalid.Any(); } }

        public OptionSet()
        {
            this.argumentRawParser = new ArgumentRawParser();
            this.argumentParser = new ArgumentParser();
        }

        public void Add<T>(string longName, string helpText, Action<T> action)
        {
            this.Add(longName, null, helpText, action);
        }

        public void Add<T>(char shortName, string helpText, Action<T> action)
        {
            this.Add(null, shortName, helpText, action);
        }

        public void Add<T>(string longName, char? shortName, string helpText, Action<T> action)
        {
            var argument = new Argument<T>(longName, shortName, helpText, action);
            this.Add(argument);
        }

        public void Add<T>(Argument<T> argument)
        {
            var argumentMap = new ArgumentMap(
                target: argument.Action.Target,
                targetMember: argument.Action.Method,
                mapName: argument.LongName ?? argument.ShortName.ToString(),
                longName: argument.LongName,
                shortName: argument.ShortName,
                type: argument.Type,
                isOptional: argument.IsOptional,
                hasDefaultValue: argument.HasDefaultValue,
                defaultValue: argument.DefaultValue,
                helpText: argument.HelpText,
                showHelpComplement: argument.ShowHelpComplement,
                position: argument.Position
            );

            argumentsMap.Add(argumentMap);
        }

        public void Parse(string[] args, bool enablePositionalArgs = false)
        {
            var argumentsRaw = this.argumentRawParser.Parse(args, null);
            var parseds = this.argumentParser.Parse(argumentsRaw, enablePositionalArgs, this.argumentsMap);
            this.ArgumentsValid = parseds.Where(f => f.ParsingStates.HasFlag(ArgumentParsedState.Valid));
            this.ArgumentsInvalid = parseds.Where(f => f.ParsingStates.HasFlag(ArgumentParsedState.IsInvalid) && !f.ParsingStates.HasFlag(ArgumentParsedState.ArgumentIsNotRequired));
            this.Execute();
        }

        private void Execute()
        {
            foreach(var arg in this.ArgumentsValid)
            {
                var method = arg.Map.TargetMember as MethodInfo;
                method.Invoke(arg.Map.Target, new[] { arg.Value });
            }
        }

        public class Argument<T>
        {
            private object defaultValue;

            internal Type Type { get; private set; }
            internal bool HasDefaultValue { get; private set; }
            public string LongName { get;set; }
            public char? ShortName { get; set; }
            public bool IsOptional { get; set; }
            public string HelpText { get; set; }
            public bool ShowHelpComplement { get; set; }
            public int? Position { get; set; }
            public Action<T> Action { get; set; }

            public object DefaultValue
            {
                get
                {
                    return defaultValue;
                }
                private set
                {
                    defaultValue = value;
                    HasDefaultValue = true;
                }
            }

            public Argument()
            {
                this.IsOptional = true;
            }

            public Argument(string longName, string helpText = null, Action<T> action = null)
                : this(longName, null, helpText, action)
            {
            }

            public Argument(char shortName, string helpText = null, Action<T> action = null)
                : this(null, shortName, helpText, action)
            {
            }

            public Argument(string longName, char? shortName, string helpText = null, Action<T> action = null)
            {
                this.LongName = longName;
                this.ShortName = shortName;
                this.Type = typeof(T);
                this.IsOptional = true;
                this.HelpText = helpText;
            }
        }
    }
}

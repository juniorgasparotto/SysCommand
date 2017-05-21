using System.Collections.Generic;
using System;
using System.Linq;
using SysCommand.Parsing;
using SysCommand.Mapping;
using System.Reflection;
using SysCommand.DefaultExecutor;
using SysCommand.Compatibility;

namespace SysCommand.Extras
{
    /// <summary>
    /// Represent a collection of argument
    /// </summary>
    public sealed class OptionSet
    {
        private List<ArgumentMap> argumentsMap = new List<ArgumentMap>();
        private ArgumentParser argumentParser;
        private ArgumentRawParser argumentRawParser;

        /// <summary>
        /// After execution, contains a list of valid arguments
        /// </summary>
        public IEnumerable<ArgumentParsed> ArgumentsValid { get; private set; }

        /// <summary>
        /// After execution, contains a list of invalid arguments
        /// </summary>
        public IEnumerable<ArgumentParsed> ArgumentsInvalid { get; private set; }

        /// <summary>
        /// After execution, verify if exists errors
        /// </summary>
        public bool HasError { get { return ArgumentsInvalid.Any(); } }

        /// <summary>
        /// Initiliaze OptionSet
        /// </summary>
        public OptionSet()
        {
            this.argumentRawParser = new ArgumentRawParser();
            this.argumentParser = new ArgumentParser();
        }

        /// <summary>
        /// Add a new argument
        /// </summary>
        /// <typeparam name="T">Type of argument</typeparam>
        /// <param name="longName">Long name of argument</param>
        /// <param name="helpText">Help text of argument</param>
        /// <param name="action">Callback when executed with success</param>
        public void Add<T>(string longName, string helpText, Action<T> action)
        {
            this.Add(longName, null, helpText, action);
        }

        /// <summary>
        /// Add a new argument 
        /// </summary>
        /// <typeparam name="T">Type of argument</typeparam>
        /// <param name="shortName">Short name of argument</param>
        /// <param name="helpText">Help text of argument</param>
        /// <param name="action">Callback when executed with success</param>
        public void Add<T>(char shortName, string helpText, Action<T> action)
        {
            this.Add(null, shortName, helpText, action);
        }

        /// <summary>
        /// Add a new argument
        /// </summary>
        /// <typeparam name="T">Type of argument</typeparam>
        /// <param name="longName">Long name of argument</param>
        /// <param name="shortName">Short name of argument</param>
        /// <param name="helpText">Help text of argument</param>
        /// <param name="action">Callback when executed with success</param>
        public void Add<T>(string longName, char? shortName, string helpText, Action<T> action)
        {
            var argument = new Argument<T>(longName, shortName, helpText, action);
            this.Add(argument);
        }

        /// <summary>
        /// Add a new argument
        /// </summary>
        /// <typeparam name="T">Type of argument</typeparam>
        /// <param name="argument">Instance of argument with all settings</param>
        public void Add<T>(Argument<T> argument)
        {
            var argumentMap = new ArgumentMap(
                target: argument.Action.Target,
                targetMember: argument.Action.Method(),
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

        /// <summary>
        /// Parse all arguments configured
        /// </summary>
        /// <param name="args">List of string that represent command line splited</param>
        /// <param name="enablePositionalArgs">Determine whether will consider positional arguments</param>
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

        /// <summary>
        /// Represent a argument in command line
        /// </summary>
        /// <typeparam name="T">Type of argument</typeparam>
        public class Argument<T>
        {
            private object defaultValue;

            internal Type Type { get; private set; }
            internal bool HasDefaultValue { get; private set; }

            /// <summary>
            /// Long name of argument
            /// </summary>
            public string LongName { get;set; }

            /// <summary>
            /// Short name of argument
            /// </summary>
            public char? ShortName { get; set; }

            /// <summary>
            /// Determine whether the argument is optional
            /// </summary>
            public bool IsOptional { get; set; }

            /// <summary>
            /// Argument help text
            /// </summary>
            public string HelpText { get; set; }

            /// <summary>
            /// Determine whether exist a complement text in help
            /// </summary>
            public bool ShowHelpComplement { get; set; }

            /// <summary>
            /// Position of this argument in command line
            /// </summary>
            public int? Position { get; set; }

            /// <summary>
            /// Callback when executed
            /// </summary>
            public Action<T> Action { get; set; }

            /// <summary>
            /// Default value if exists
            /// </summary>
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

            /// <summary>
            /// Initialize a argument
            /// </summary>
            public Argument()
            {
                this.IsOptional = true;
            }

            /// <summary>
            /// Initialize a argument
            /// </summary>
            /// <param name="longName">Argument long name</param>
            /// <param name="helpText">Argument help text</param>
            /// <param name="action">Callback when executed</param>
            public Argument(string longName, string helpText = null, Action<T> action = null)
                : this(longName, null, helpText, action)
            {
            }

            /// <summary>
            /// Initialize a argument
            /// </summary>
            /// <param name="shortName">Argument short name</param>
            /// <param name="helpText">Argument help text</param>
            /// <param name="action">Callback when executed</param>
            public Argument(char shortName, string helpText = null, Action<T> action = null)
                : this(null, shortName, helpText, action)
            {
            }

            /// <summary>
            /// Initialize a argument
            /// </summary>
            /// <param name="longName">Argument long name</param>
            /// <param name="shortName">Argument short name</param>
            /// <param name="helpText">Argument help text</param>
            /// <param name="action">Callback when executed</param>
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

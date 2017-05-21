using System.Collections.Generic;
using SysCommand.Parsing;
using SysCommand.Execution;
using System;
using SysCommand.Mapping;

namespace SysCommand.DefaultExecutor
{
    /// <summary>
    /// Contains all steps to execute the command line
    /// </summary>
    public class Executor : IExecutor
    {
        /// <summary>
        /// Name of method that represent the default method when is omited is command line
        /// </summary>
        public const string MAIN_METHOD_NAME = "main";
        private CommandMapper commandMapper;
        private ArgumentRawParser argumentRawParser;
        private ArgumentParser argumentParser;
        private ActionParser actionParser;

        /// <summary>
        /// Instance of CommandMapper
        /// </summary>
        public CommandMapper CommandMapper
        {
            get
            {
                if (commandMapper == null)
                {
                    var argMapper = new ArgumentMapper();
                    commandMapper = new CommandMapper(argMapper, new ActionMapper(argMapper));
                }
                return commandMapper;
            }
            set
            {
                commandMapper = value;
            }
        }

        /// <summary>
        /// Instance of ArgumentRawParser
        /// </summary>
        public ArgumentRawParser ArgumentRawParser
        {
            get
            {
                if (argumentRawParser == null)
                    argumentRawParser = new ArgumentRawParser();
                return argumentRawParser;
            }
            set
            {
                argumentRawParser = value;
            }
        }

        /// <summary>
        /// Instance of ArgumentParser
        /// </summary>
        public ArgumentParser ArgumentParser
        {
            get
            {
                if (argumentParser == null)
                    argumentParser = new ArgumentParser();
                return argumentParser;
            }
            set
            {
                argumentParser = value;
            }
        }

        /// <summary>
        /// Instance of ActionParser
        /// </summary>
        public ActionParser ActionParser
        {
            get
            {
                if (actionParser == null)
                    actionParser = new ActionParser(this.ArgumentParser);
                return actionParser;
            }
            set
            {
                actionParser = value;
            }
        }

        /// <summary>
        /// Get maps from all commands
        /// </summary>
        /// <param name="commands">List of commands</param>
        /// <returns>List of CommandMap</returns>
        public IEnumerable<CommandMap> GetMaps(IEnumerable<CommandBase> commands)
        {
            return this.CommandMapper.Map(commands);
        }

        /// <summary>
        /// Parse a list of string to arguments raw
        /// </summary>
        /// <param name="args">List of string that represent a command line splited</param>
        /// <param name="commandsMap">List of commands</param>
        /// <returns>List of ArgumentRaw</returns>
        public IEnumerable<ArgumentRaw> ParseRaw(string[] args, IEnumerable<CommandMap> commandsMap)
        {
            var allMethodsMaps = commandsMap.GetMethods();
            return this.ArgumentRawParser.Parse(args, allMethodsMaps);
        }

        /// <summary>
        /// Parse all elements
        /// </summary>
        /// <param name="args">List of string that represent a command line splited</param>
        /// <param name="argumentsRaw">List of ArgumentRaw</param>
        /// <param name="commandsMap">List of CommandMap</param>
        /// <param name="enableMultiAction">Determine whether can have more than on action per command line</param>
        /// <returns>Return the ParseResult instance that represent a parsed model of all elements</returns>
        public ParseResult Parse(string[] args, IEnumerable<ArgumentRaw> argumentsRaw, IEnumerable<CommandMap> commandsMap, bool enableMultiAction)
        {
            var parser = new InternalParser(this.ArgumentParser, this.ActionParser);
            return parser.Parse(args, argumentsRaw, commandsMap, enableMultiAction);
        }

        /// <summary>
        /// Execute a parse instance
        /// </summary>
        /// <param name="parseResult">Instance of parse that represent all parsed elements</param>
        /// <param name="onInvoke">Callback of each invoke (properties or methods)</param>
        /// <returns>Return the resume object of the execution</returns>
        public ExecutionResult Execute(ParseResult parseResult, Action<IMemberResult, ExecutionScope> onInvoke)
        {
            var executor = new InternalExecutor();
            return executor.Execute(parseResult, onInvoke);
        }
    }
}

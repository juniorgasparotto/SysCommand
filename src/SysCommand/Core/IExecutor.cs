using SysCommand.Execution;
using SysCommand.Mapping;
using SysCommand.Parsing;
using System;
using System.Collections.Generic;

namespace SysCommand
{
    /// <summary>
    /// Contains all steps to execute the command line
    /// </summary>
    public interface IExecutor
    {
        /// <summary>
        /// Get maps from all commands
        /// </summary>
        /// <param name="commands">List of commands</param>
        /// <returns>List of CommandMap</returns>
        IEnumerable<CommandMap> GetMaps(IEnumerable<CommandBase> commands);

        /// <summary>
        /// Parse a list of string to arguments raw
        /// </summary>
        /// <param name="args">List of string that represent a command line splited</param>
        /// <param name="commandsMap">List of commands</param>
        /// <returns>List of ArgumentRaw</returns>
        IEnumerable<ArgumentRaw> ParseRaw(string[] args, IEnumerable<CommandMap> commandsMap);

        /// <summary>
        /// Parse all elements
        /// </summary>
        /// <param name="args">List of string that represent a command line splited</param>
        /// <param name="argumentsRaw">List of ArgumentRaw</param>
        /// <param name="commandsMap">List of CommandMap</param>
        /// <param name="enableMultiAction">Determine whether can have more than on action per command line</param>
        /// <returns>Return the ParseResult instance that represent a parsed model of all elements</returns>
        ParseResult Parse(string[] args, IEnumerable<ArgumentRaw> argumentsRaw, IEnumerable<CommandMap> commandsMap, bool enableMultiAction);

        /// <summary>
        /// Execute a parse instance
        /// </summary>
        /// <param name="parseResult">Instance of parse that represent all parsed elements</param>
        /// <param name="onInvoke">Callback of each invoke (properties or methods)</param>
        /// <returns>Return the resume object of the execution</returns>
        ExecutionResult Execute(ParseResult parseResult, Action<IMemberResult, ExecutionScope> onInvoke);
    }
}
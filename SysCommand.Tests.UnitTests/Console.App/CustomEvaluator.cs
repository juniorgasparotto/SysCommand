using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using System.Linq;
using SysCommand.Tests.ConsoleApp.Commands;
using SysCommand.ConsoleApp;
using System.IO;
using SysCommand.Test;
using SysCommand.Parser;
using Mono.Options;
using System.Reflection;
using System.Collections;

namespace SysCommand.Tests.UnitTests
{
    public class CustomEvaluator : IEvaluationStrategy, IEnumerable<ArgumentMap>
    {
        private List<ArgumentMap> argumentsMap = new List<ArgumentMap>();
        public Action<IMember> OnInvoke { get; set; }

        public void Add<T>(string longName, string helpText, Action<T> action)
        {
            this.Add(longName, null, helpText, action);
        }

        public void Add<T>(char? shortName, string helpText, Action<T> action)
        {
            this.Add(null, shortName, helpText, action);
        }

        public void Add<T>(string longName, char? shortName, string helpText, Action<T> action)
        {
            var argumentMap = new ArgumentMap(
                source: action.Target,
                propertyOrParameter: action.Method,
                mapName: longName ?? shortName.ToString(),
                longName: longName,
                shortName: shortName,
                type: typeof(T),
                isOptional: true,
                hasDefaultValue: false,
                defaultValue: null,
                helpText: helpText,
                showHelpComplement: false,
                position: null
            );

            argumentsMap.Add(argumentMap);
        }

        public virtual ParseResult Parse(string[] args, IEnumerable<CommandMap> commandsMap, bool enableMultiAction)
        {
            var parseResult = new ParseResult();
            var argumentsRaw = CommandParser.ParseArgumentsRaw(args, null);
            var parseds = CommandParser.ParseArgumentMapped(argumentsRaw, false, this.argumentsMap);

            var level = new ParseResult.Level()
            {
                LevelNumber = 0
            };

            var commandParse = new ParseResult.CommandParse()
            {
                Command = null
            };

            parseResult.Add(level);
            level.Add(commandParse);

            commandParse.AddProperties(parseds.Where(f => f.MappingStates.HasFlag(ArgumentMappingState.Valid)));

            // Don't considere invalid args in this situation:
            // -> ArgumentMappingType.HasNoInput && ArgumentMappingState.ArgumentIsNotRequired
            // "ArgumentMappingType.HasNoInput": Means that args don't have input.
            // "ArgumentMappingState.ArgumentIsNotRequired": Means that args is optional.
            // in this situation the args is not consider invalid.
            commandParse.AddPropertiesInvalid(parseds.Where(f => f.MappingStates.HasFlag(ArgumentMappingState.IsInvalid) && !f.MappingStates.HasFlag(ArgumentMappingState.ArgumentIsNotRequired)));
            return parseResult;
        }

        public EvaluateResult Evaluate(string[] args, IEnumerable<CommandMap> maps, ParseResult parseResult)
        {
            var evaluateResult = new EvaluateResult();
            evaluateResult.Result = new Result<IMember>();

            var commandParse = parseResult.Levels.First().Commands.First();
            if (commandParse.IsValid)
            {
                var properties = commandParse.Properties;
                evaluateResult.Result.AddRange(properties.Select(f => new ArgumentAction(f)));
                evaluateResult.Result.Invoke(this.OnInvoke);
                evaluateResult.State = EvaluateState.Success;
            }
            else
            {
                var errors = this.CreateErrors(parseResult);
                evaluateResult.Errors = errors;

                bool allPropertiesNotExists = commandParse.PropertiesInvalid.All(f => f.MappingType == ArgumentMappingType.NotMapped);

                if (allPropertiesNotExists)
                    evaluateResult.State = EvaluateState.NotFound;
                else
                    evaluateResult.State = EvaluateState.HasError;
            }

            return evaluateResult;
        }

        private List<CommandError> CreateErrors(ParseResult parseResult)
        {
            var levelsInvalid =
                parseResult
               .Levels
               .Where(f => f.Commands.Empty(c => c.IsValid) || f.Commands.Any(c => c.HasAnyArgumentRequired));

            var commandsInvalids = levelsInvalid.SelectMany(f => f.Commands.Where(c => c.HasError));
            var groupsCommands = commandsInvalids.GroupBy(f => f.Command);

            var commandsErrors = new List<CommandError>();
            foreach (var groupCommand in groupsCommands)
            {
                var commandError = new CommandError();
                commandError.Command = groupCommand.Key;
                commandError.Methods.AddRange(groupCommand.SelectMany(f => f.MethodsInvalid));
                commandError.Properties.AddRange(groupCommand.SelectMany(f => f.PropertiesInvalid));

                commandsErrors.Add(commandError);
            }

            return commandsErrors;
        }

        public IEnumerator<ArgumentMap> GetEnumerator()
        {
            return (IEnumerator <ArgumentMap>)argumentsMap;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return argumentsMap.GetEnumerator();
        }
    }
}

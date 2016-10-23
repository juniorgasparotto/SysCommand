using System.Collections.Generic;
using System;
using System.Linq;
using SysCommand.Parsing;
using System.Collections;
using SysCommand.Evaluation;
using SysCommand.Mapping;
using SysCommand.Utils;

namespace SysCommand.Utils.Extras
{
    public class ArgumentSet : IEnumerable<ArgumentMap>
    {
        private List<ArgumentMap> argumentsMap = new List<ArgumentMap>();
        public Action<IMemberResult> OnInvoke { get; set; }

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

        public virtual ParseResult Parse(string[] args)
        {
            var parseResult = new ParseResult();
            var argumentsRaw = CommandParserUtils.ParseArgumentsRaw(args, null);
            var parseds = CommandParserUtils.GetArgumentsParsed(argumentsRaw, false, this.argumentsMap);

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

            commandParse.AddProperties(parseds.Where(f => f.ParsingStates.HasFlag(ArgumentParsedState.Valid)));

            // Don't considere invalid args in this situation:
            // -> ArgumentMappingType.HasNoInput && ArgumentMappingState.ArgumentIsNotRequired
            // "ArgumentMappingType.HasNoInput": Means that args don't have input.
            // "ArgumentMappingState.ArgumentIsNotRequired": Means that args is optional.
            // in this situation the args is not consider invalid.
            commandParse.AddPropertiesInvalid(parseds.Where(f => f.ParsingStates.HasFlag(ArgumentParsedState.IsInvalid) && !f.ParsingStates.HasFlag(ArgumentParsedState.ArgumentIsNotRequired)));
            return parseResult;
        }

        public EvaluateResult Evaluate(ParseResult parseResult)
        {
            var results = new List<IMemberResult>();
            var evaluateResult = new EvaluateResult();
            evaluateResult.Results = results;

            var commandParse = parseResult.Levels.First().Commands.First();
            if (commandParse.IsValid)
            {
                var properties = commandParse.Properties;
                results.AddRange(properties.Select(f => new ArgumentResult(f)));
                evaluateResult.Results.Invoke(this.OnInvoke);
                evaluateResult.State = EvaluateState.Success;
            }
            else
            {
                var errors = this.CreateErrors(parseResult);
                evaluateResult.Errors = errors;

                bool allPropertiesNotExists = commandParse.PropertiesInvalid.All(f => f.ParsingType == ArgumentParsedType.NotMapped);

                if (allPropertiesNotExists)
                    evaluateResult.State = EvaluateState.NotFound;
                else
                    evaluateResult.State = EvaluateState.HasError;
            }

            return evaluateResult;
        }

        private IEnumerable<EvaluateError> CreateErrors(ParseResult parseResult)
        {
            var levelsInvalid =
                parseResult
               .Levels
               .Where(f => f.Commands.Empty(c => c.IsValid) || f.Commands.Any(c => c.HasAnyArgumentRequired));

            var commandsInvalids = levelsInvalid.SelectMany(f => f.Commands.Where(c => c.HasError));
            var groupsCommands = commandsInvalids.GroupBy(f => f.Command);

            var commandsErrors = new List<EvaluateError>();
            foreach (var groupCommand in groupsCommands)
            {
                var propertiesInvalid = new List<ArgumentParsed>();
                propertiesInvalid.AddRange(groupCommand.SelectMany(f => f.PropertiesInvalid));
                var commandError = new EvaluateError();
                commandError.Command = groupCommand.Key;
                commandError.PropertiesInvalid = propertiesInvalid;
                commandsErrors.Add(commandError);
            }

            return commandsErrors;
        }

        public IEnumerator<ArgumentMap> GetEnumerator()
        {
            return (IEnumerator<ArgumentMap>)argumentsMap;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return argumentsMap.GetEnumerator();
        }
    }
}

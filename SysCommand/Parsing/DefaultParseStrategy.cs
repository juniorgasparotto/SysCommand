using System.Collections.Generic;
using System.Linq;
using SysCommand.Mapping;
using SysCommand.Utils;

namespace SysCommand.Parsing
{
    public class DefaultParseStrategy : IParseStrategy
    {
        public virtual ParseResult Parse(string[] args, IEnumerable<CommandMap> commandsMap, bool enableMultiAction)
        {
            var parseResult = new ParseResult();
            parseResult.Args = args;
            parseResult.Maps = commandsMap;
            parseResult.EnableMultiAction = enableMultiAction;

            var allMethodsMaps = commandsMap.GetMethods();
            var argumentsRaw = CommandParserUtils.ParseArgumentsRaw(args, allMethodsMaps);

            IEnumerable<ArgumentRaw> initialExtraArguments;
            var methodsParsed = CommandParserUtils.GetActionsParsed(argumentsRaw, enableMultiAction, allMethodsMaps, out initialExtraArguments).ToList();

            var hasMethodsParsed = methodsParsed.Count > 0;
            var hasExtras = initialExtraArguments.Any();
            if (hasExtras || (args.Length == 0 && !hasMethodsParsed))
            {
                var level = new ParseResult.Level();
                level.LevelNumber = 0;
                parseResult.Add(level);

                foreach (var commandMap in commandsMap)
                {
                    var commandParse = new ParseResult.CommandParse();
                    commandParse.Command = commandMap.Command;
                    level.Add(commandParse);

                    this.ParseProperties(commandMap, commandParse, initialExtraArguments);
                }
            }

            // step1: there are methods that are candidates to be execute
            if (hasMethodsParsed)
            {
                // step2: separate per level
                var levelsGroups = methodsParsed.GroupBy(f => f.Level).OrderBy(f => f.Key);
                foreach (var levelGroup in levelsGroups)
                {
                    var level = new ParseResult.Level();
                    parseResult.Add(level);

                    var commandsGroups = levelGroup.GroupBy(f => f.ActionMap.Target);
                    foreach (var commandGroup in commandsGroups)
                    {
                        var commandParse = new ParseResult.CommandParse();
                        commandParse.Command = (CommandBase)commandGroup.Key;
                        level.Add(commandParse);

                        bool isBestMethodButHasError;
                        var bestMethod = this.GetBestMethod(commandGroup, out isBestMethodButHasError);

                        if (isBestMethodButHasError)
                            commandParse.AddMethodInvalid(bestMethod);
                        else
                            commandParse.AddMethod(bestMethod);

                        // in this part of the code the method is 100% valid
                        // but can be exists extra arguments that are used 
                        // with properties inputs.
                        var argumentsExtras = bestMethod.ArgumentsExtras.SelectMany(f => f.AllRaw).ToList();
                        var commandMap = commandsMap.First(f => f.Command == commandParse.Command);
                        this.ParseProperties(commandMap, commandParse, argumentsExtras);
                    }
                }
            }

            // organize level number
            var i = 0;
            foreach(var level in parseResult.Levels)
                level.LevelNumber = i++;

            return parseResult;
        }

        private ActionParsed GetBestMethod(IEnumerable<ActionParsed> methods, out bool isBestMethodButHasError)
        {
            var candidates = methods
                    .Select(m => new
                    {
                        method = m,
                        countParameters = m.ActionMap.ArgumentsMaps.Count(),
                        countMappedParameters = m.Arguments.Count(a => a.IsMapped),
                        countValidParameters = m.Arguments.Count(a => a.ParsingStates.HasFlag(ArgumentParsedState.Valid))
                    })
                    .OrderByDescending(o => o.countMappedParameters)
                    .ThenBy(o => o.countParameters)
                    .ToList();

            var allValid = candidates.Where(
                    m => m.countParameters == m.countValidParameters
                );

            if (allValid.Any())
            {
                isBestMethodButHasError = false;
                return allValid.First().method;
            }
            
            isBestMethodButHasError = true;
            return candidates.First().method;
        }
        
        private void ParseProperties(CommandMap commandMap, ParseResult.CommandParse commandParse, IEnumerable<ArgumentRaw> argumentsRaw)
        {
            var parseds = CommandParserUtils.GetArgumentsParsed(argumentsRaw, commandMap.Command.EnablePositionalArgs, commandMap.Properties);
            commandParse.AddProperties(parseds.Where(f => f.ParsingStates.HasFlag(ArgumentParsedState.Valid)));

            // Don't considere invalid args in this situation:
            // -> ArgumentMappingType.HasNoInput && ArgumentMappingState.ArgumentIsNotRequired
            // "ArgumentMappingType.HasNoInput": Means that args don't have input.
            // "ArgumentMappingState.ArgumentIsNotRequired": Means that args is optional.
            // in this situation the args is not consider invalid.
            commandParse.AddPropertiesInvalid(parseds.Where(f => f.ParsingStates.HasFlag(ArgumentParsedState.IsInvalid) && !f.ParsingStates.HasFlag(ArgumentParsedState.ArgumentIsNotRequired)));
        }
    }
}
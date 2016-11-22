using System.Collections.Generic;
using System.Linq;
using SysCommand.Mapping;
using SysCommand.Parsing;
using SysCommand.Helpers;

namespace SysCommand.DefaultExecutor
{
    internal class InternalParser
    {
        private ActionParser actionParser;
        private ArgumentParser argumentParser;

        public InternalParser(ArgumentParser argumentParser, ActionParser actionParser)
        {
            this.argumentParser = argumentParser;
            this.actionParser = actionParser;
        }

        public ParseResult Parse(string[] args, IEnumerable<ArgumentRaw> argumentsRaw, IEnumerable<CommandMap> commandsMap, bool enableMultiAction)
        {
            var parseResult = new ParseResult();
            parseResult.Args = args;
            parseResult.Maps = commandsMap;
            parseResult.EnableMultiAction = enableMultiAction;
            //parseResult.ArgumentsRaw = argumentsRaw;

            IEnumerable<ArgumentRaw> initialExtraArguments;
            var allMethodsMaps = commandsMap.GetMethods();
            var methodsParsed = this.actionParser.Parse(argumentsRaw, enableMultiAction, allMethodsMaps, out initialExtraArguments).ToList();

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

                    this.ParseProperties(commandMap.Properties, commandMap.Command.EnablePositionalArgs, commandParse, initialExtraArguments);
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
                        var commandParse = new ParseResult.CommandParse()
                        {
                            Command = (CommandBase)commandGroup.Key
                        };

                        var commandMap = commandsMap.First(f => f.Command == commandParse.Command);
                        var argumentsMap = commandMap.Properties;

                        level.Add(commandParse);

                        bool isBestMethodButHasError;
                        var bestMethod = this.GetBestMethod(commandGroup, out isBestMethodButHasError);
                        var argumentsExtras = bestMethod.ArgumentsExtras;

                        // Dosen't add this method to be executed in this scenary:
                        // 1) is default 
                        // 2) and input is implicit (without action name)
                        var addMethod = true;
                        var isDefaultAndInputIsImplicit = bestMethod.ActionMap.IsDefault
                                                       && bestMethod.ArgumentRawOfAction == null;

                        if (isDefaultAndInputIsImplicit)
                        {
                            var countParameter = bestMethod.ActionMap.ArgumentsMaps.Count();

                            // 1) the default method dosen't have arguments
                            // 2) and have one or more input arguments
                            if (countParameter == 0 && args.Length > 0)
                            {
                                addMethod = false;
                            }
                            // 1) if default method has parameter, but exists error
                            //    else ignore this default method and use your parameters
                            //    to be used as input for the properties. This rule mantain
                            //    the fluent match.
                            // 2) Is obrigatory exists an less one property map to ignore 
                            //    this method.
                            // scenary:
                            // ----
                            //  default(int a)
                            //  string PropertyValue
                            // ----
                            //$ --property-value "invalid-value-for-default-method"
                            // expected: PropertyValue = invalid-value-for-default-method
                            else if (countParameter > 0 && isBestMethodButHasError && argumentsMap.Any())
                            { 
                                addMethod = false;
                                argumentsExtras = bestMethod.Arguments.Concat(bestMethod.ArgumentsExtras);
                            }
                        }
                        
                        // parse properties in same command with extras arguments the current method
                        var argumentsRawProperties = argumentsExtras.SelectMany(f => f.AllRaw).ToList();
                        this.ParseProperties(commandMap.Properties, commandMap.Command.EnablePositionalArgs, commandParse, argumentsRawProperties);

                        if (addMethod)
                        {
                            if (isBestMethodButHasError)
                                commandParse.AddMethodInvalid(bestMethod);
                            else
                                commandParse.AddMethod(bestMethod);
                        }
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
                        countValidParameters = m.Arguments.Count(a => a.ParsingStates.HasFlag(ArgumentParsedState.Valid)),
                        countExtras = m.ArgumentsExtras.Count()
                    })
                    .OrderByDescending(o => o.countMappedParameters)
                    .ThenBy(o => o.countParameters)
                    .ToList();

            // if have more than one valid, select the method
            // that have less extras arguments.
            // Main(int a, int b)
            // Main(string[] lst)
            // input1: main 1 2   -> Main(int a, int b) = 0 extras - selected because have more mapped args
            // input1: main 1 2   -> Main(string[] lst) = 0 extras
            // input2: main 1 2 3 -> Main(int a, int b) = 1 extras
            // input2: main 1 2 3 -> Main(string[] lst) = 0 extras - selected because no have extras
            var allValid = candidates.Where(
                    m =>
                    {
                        return m.countParameters == m.countValidParameters;
                    }
                )
                .OrderBy(f => f.countExtras);

            if (allValid.Any())
            {
                isBestMethodButHasError = false;
                return allValid.First().method;
            }
            
            isBestMethodButHasError = true;
            return candidates.First().method;
        }
        
        private void ParseProperties(IEnumerable<ArgumentMap> argumentsMaps, bool enablePositionalArgs, ParseResult.CommandParse commandParse, IEnumerable<ArgumentRaw> argumentsRaw)
        {
            var parseds = this.argumentParser.Parse(argumentsRaw, enablePositionalArgs, argumentsMaps);
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
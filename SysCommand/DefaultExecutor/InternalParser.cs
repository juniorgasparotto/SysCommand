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

            IEnumerable<ArgumentRaw> initialExtraArguments;
            var allPropertiesMap = commandsMap.GetProperties();
            var allMethodsMaps = commandsMap.GetMethods();
            var hasPropertyMap = allPropertiesMap.Any();

            var methodsParsed = this.actionParser.Parse(argumentsRaw, enableMultiAction, allMethodsMaps, out initialExtraArguments).ToList();
            var hasMethodsParsed = methodsParsed.Count > 0;

            // *******
            // Step1: Parse properties if has initial extras or if doesn't have arguments and methods found.
            // *******
            // noArgsAndNotExistsDefaultMethods: Important to cenaries:
            //      1) To show errors (Argument obrigatory for example)
            //      2) Process properties with default value
            var noArgsAndNotExistsDefaultMethods = (args.Length == 0 && !hasMethodsParsed);
            if (initialExtraArguments.Any() || noArgsAndNotExistsDefaultMethods)
            {
                var levelInitial = this.GetLevelWithProperties(commandsMap, initialExtraArguments);
                if (!this.IsEmptyLevel(levelInitial))
                    parseResult.Add(levelInitial);
            }

            if (hasMethodsParsed)
            {
                // Separate per level
                var levelsGroups = methodsParsed.GroupBy(f => f.Level).OrderBy(f => f.Key);
                foreach (var levelGroup in levelsGroups)
                {
                    // Separate per Command
                    var commandsGroups = levelGroup.GroupBy(f => f.ActionMap.Target);

                    // Step2 & Step 3: Create level only with methods and separate valids and invalids methods
                    var levelOfMethods = this.GetLevelWithMethods(args, hasPropertyMap, commandsGroups);
                    if (levelOfMethods == null)
                        continue;

                    var bestValidMethodInLevel = this.GetBestValidMethodInLevel(levelOfMethods);

                    if (bestValidMethodInLevel != null)
                    {
                        // Step2: Remove imcompatible valid methods
                        this.RemoveIncompatibleValidMethods(levelOfMethods, bestValidMethodInLevel);
                        if (!this.IsEmptyLevel(levelOfMethods))
                            parseResult.Add(levelOfMethods);

                        // Step3: Get extras of the best method and use as properties input to create a new level.
                        var levelOfProperties = this.GetLevelWithPropertiesUsingMethodExtras(commandsMap, bestValidMethodInLevel);
                        if (levelOfProperties != null && !this.IsEmptyLevel(levelOfProperties))
                            parseResult.Add(levelOfProperties);
                    }
                    else
                    {
                        // Step4 & Step5: When not exists valid methods
                        var allRemoved = this.RemoveInvalidImplicitDefaultMethodsIfExists(args, levelOfMethods, hasPropertyMap);
                        if (allRemoved)
                        {
                            // Step4: All defaults and invalid methods have been ignored to maintain the fluency 
                            // and the process of following with properties.
                            var levelOfProperties = this.GetLevelWithProperties(commandsMap, argumentsRaw);
                            if (levelOfProperties != null && !this.IsEmptyLevel(levelOfProperties))
                                parseResult.Add(levelOfProperties);
                        }
                        else
                        {
                            // Step5: There are some default method that has a real problem.
                            parseResult.Add(levelOfMethods);
                        }
                    }
                }
            }

            // *******
            // Step5: Organize level number
            // *******
            this.OrganizeLevelsSequence(parseResult);

            return parseResult;
        }

        private bool RemoveInvalidImplicitDefaultMethodsIfExists(string[] args, ParseResult.Level levelOfMethods, bool hasPropertyMap)
        {
            var defaultsInvalidMethodsWithImplicitInput = levelOfMethods.Commands
                                        .SelectMany(f => f.MethodsInvalid)
                                        .Where(f => f.ActionMap.IsDefault && f.ArgumentRawOfAction == null);

            // Step4: Exists action specify with name in input: my-action --p1 2
            if (defaultsInvalidMethodsWithImplicitInput.Empty())
                return false;

            // Step5: Exists default action without name in input: --p1 2 (action omitted)
            var countRemove = 0;
            foreach (var defaultMethod in defaultsInvalidMethodsWithImplicitInput)
            {
                var countParameter = defaultMethod.ActionMap.ArgumentsMaps.Count();
                var removeInvalidMethod = false;

                // 1) the default method dosen't have arguments
                // 2) and have one or more input arguments
                if (countParameter == 0 && args.Length > 0)
                {
                    removeInvalidMethod = true;
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
                else if (countParameter > 0 && hasPropertyMap)
                {
                    removeInvalidMethod = true;
                }

                if (removeInvalidMethod)
                {
                    foreach (var cmd in levelOfMethods.Commands)
                        foreach (var method in cmd.Methods.ToList())
                            if (method == defaultMethod)
                                cmd.RemoveInvalidMethod(method);
                    countRemove++;
                }
            }

            var allWasRemoved = countRemove == defaultsInvalidMethodsWithImplicitInput.Count();
            return allWasRemoved;
        }

        private void OrganizeLevelsSequence(ParseResult parseResult)
        {
            var i = 0;
            foreach (var level in parseResult.Levels)
                level.LevelNumber = i++;
        }

        private bool IsEmptyLevel(ParseResult.Level level)
        {
            var isEmpty = true;
            foreach (var cmd in level.Commands)
            {
                if (   cmd.Methods.Any()
                    || cmd.MethodsInvalid.Any()
                    || cmd.Properties.Any()
                    || cmd.PropertiesInvalid.Any())
                {
                    isEmpty = false;
                    break;
                }
            }

            return isEmpty;
        }

        private void RemoveIncompatibleValidMethods(ParseResult.Level level, ActionParsed reference)
        {
            // Remove all valid methos there are incompatible with best valid method
            foreach (var cmd in level.Commands)
            {
                foreach (var validMethod in cmd.Methods.ToList())
                {
                    if (validMethod == reference)
                        continue;

                    var argsA = reference.Arguments;
                    var argsB = validMethod.Arguments;

                    if (!this.AllRawAreEqualsOfArgumentParsed(argsA, argsB))
                        cmd.RemoveMethod(validMethod);
                }
            }
        }

        private bool AllRawAreEqualsOfArgumentParsed(IEnumerable<ArgumentParsed> argsA, IEnumerable<ArgumentParsed> argsB)
        {
            var countA = argsA.Count();
            var countB = argsB.Count();

            if (countA + countB == 0)
                return true;

            if (countA > 0)
            {
                for (var i = 0; i < countA; i++)
                {
                    var argA = argsA.ElementAt(i);
                    var argB = argsB.ElementAtOrDefault(i);

                    if (!this.AllRawAreEqualsInArgumentParsed(argA, argB))
                        return false;
                }

                return true;
            }

            return false;
        }

        private bool AllRawAreEqualsInArgumentParsed(ArgumentParsed argA, ArgumentParsed argB) {
            if (argA == null || argB == null)
                return false;

            var countA = argA.AllRaw.Count();
            var countB = argA.AllRaw.Count();

            if (countA + countB == 0)
                return true;

            if (countA > 0)
            {
                for (var i = 0; i < countA; i++)
                {
                    var rawA = argA.AllRaw.ElementAt(i);
                    var rawB = argB.AllRaw.ElementAtOrDefault(i);
                    if (rawA != rawB)
                        return false;
                }

                return true;
            }

            return false;
        }

        private ParseResult.Level GetLevelWithMethods(string[] args, bool hasPropertyMap, IEnumerable<IGrouping<object, ActionParsed>> commandsGroups)
        {
            ParseResult.Level level = null;
            if (commandsGroups.Any())
            {
                level = new ParseResult.Level();

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
                }
            }

            return level;
        }

        private ParseResult.Level GetLevelWithPropertiesUsingMethodExtras(IEnumerable<CommandMap> commandsMap, ActionParsed bestValidMethodInLevel)
        {
            var argumentsRawProperties = bestValidMethodInLevel.ArgumentsExtras.SelectMany(f => f.AllRaw).ToList();
            if (argumentsRawProperties.Any())
                return this.GetLevelWithProperties(commandsMap, argumentsRawProperties);
            return null;
        }

        private ParseResult.Level GetLevelWithProperties(IEnumerable<CommandMap> commandsMap, IEnumerable<ArgumentRaw> arguments)
        {
            var level = new ParseResult.Level();

            foreach (var commandMap in commandsMap)
            {
                var commandParse = new ParseResult.CommandParse();
                commandParse.Command = commandMap.Command;
                level.Add(commandParse);

                this.ParseProperties(commandMap.Properties, commandMap.Command.EnablePositionalArgs, commandParse, arguments);
            }

            // Create arguments references
            var allArgsRawIsMapped = true;
            var argReferences = new List<ArgumentParsed>();
            foreach(var raw in arguments)
            {
                var argParsed = this.GetFirstValidArgumentParsedByRawInLevel(level, raw);
                if (argParsed != null)
                {
                    if (argReferences.Empty(arg => arg == argParsed))
                        argReferences.Add(argParsed);
                }
                else
                {
                    allArgsRawIsMapped = false;
                    break;
                }
            }

            if (allArgsRawIsMapped && argReferences.Count > 0)
            {
                foreach (var cmd in level.Commands)
                {
                    // Remove all invalid properties because all input was mapped 
                    // and all are valid
                    foreach (var invalid in cmd.PropertiesInvalid.ToList())
                        cmd.RemovePropertyInvalid(invalid);

                    // Remove all valid properties that are imcompatible with
                    // reference
                    foreach (var arg in cmd.Properties.ToList())
                    {
                        var isCompatibleWithSomeRefs = argReferences.Any(argRef => this.AllRawAreEqualsInArgumentParsed(argRef, arg));
                        if (!isCompatibleWithSomeRefs)
                            cmd.RemoveProperty(arg);
                    }
                }
            }

            return level;
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

        private ActionParsed GetBestValidMethodInLevel(ParseResult.Level level)
        {
            // Select the best valid method that has the major arguments inputed
            var methodsValids = level.Commands.SelectMany(c => c.Methods);

            // never has error because here only valid methods
            bool isBestMethodButHasError;
            var bestMethodInLevel = this.GetBestMethod(methodsValids, out isBestMethodButHasError);

            return bestMethodInLevel;
        }
        
        private ArgumentParsed GetFirstValidArgumentParsedByRawInLevel(ParseResult.Level level, ArgumentRaw raw)
        {
            foreach (var cmd in level.Commands)
            {
                var argParsed = cmd.Properties.FirstOrDefault(p => p.AllRaw.Any(r => r == raw));
                if (argParsed != null)
                    return argParsed;
            }

            return null;
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

    }
}
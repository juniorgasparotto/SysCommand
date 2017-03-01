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
            // Step1: Parse properties if has initial extras
            // *******
            if (initialExtraArguments.Any())
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
                    var levelOfMethods = this.GetLevelWithMethods(hasPropertyMap, commandsGroups);
                    if (levelOfMethods == null)
                        continue;

                    var bestValidMethodInLevel = this.GetBestValidMethodInLevel(levelOfMethods);

                    if (bestValidMethodInLevel != null)
                    {
                        // Step2: Add or not the method
                        var canAddMethodLevel = true;
                       
                        if (this.IsMethodImplicitAndNoArguments(bestValidMethodInLevel))
                        {
                            // Step2.1: If has args, this default implicit method can't be added.
                            //            because it is invoked only if no has input.
                            //          eg: 
                            //          method: void defaultMethodWithoutArgs()
                            //          input : $ --some-args value
                            //          result: can't executed because is invalid.
                            if (args.Length > 0)
                                canAddMethodLevel = false;
                        }

                        if (canAddMethodLevel)
                        { 
                            // Step2.2: Remove imcompatible valid methods
                            this.RemoveIncompatibleValidMethods(levelOfMethods, bestValidMethodInLevel);
                            if (!this.IsEmptyLevel(levelOfMethods))
                                parseResult.Add(levelOfMethods);
                        }

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

                            // select all raw before 'level 1'
                            IEnumerable<ArgumentRaw> raws;
                            var methodLevel1 = methodsParsed.FirstOrDefault(f => f.Level == 1);
                            if (methodLevel1 != null)
                            {
                                var rawsAux = new List<ArgumentRaw>();
                                foreach(var raw in argumentsRaw)
                                {
                                    if (raw != methodLevel1.ArgumentRawOfAction)
                                        rawsAux.Add(raw);
                                    else
                                        break;
                                }

                                raws = rawsAux;
                            }
                            else
                            {
                                raws = argumentsRaw;
                            }

                            var levelOfProperties = this.GetLevelWithProperties(commandsMap, raws);
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
            // Step5: Create properties with default values if not exists
            //        Create required properties if not exists (to show errors)
            // *******
            var levelDefaultOrRequired = this.GetLevelWithPropertiesDefaultOrRequired(parseResult.Levels, commandsMap);
            if (levelDefaultOrRequired != null && !this.IsEmptyLevel(levelDefaultOrRequired))
                parseResult.Add(levelDefaultOrRequired);

            // *******
            // Step6: Organize level number
            // *******
            this.OrganizeLevelsSequence(parseResult);

            return parseResult;
        }
        
        private bool RemoveInvalidImplicitDefaultMethodsIfExists(string[] args, ParseResult.Level levelOfMethods, bool hasPropertyMap)
        {
            var defaultsInvalidMethodsWithImplicitInput = levelOfMethods.Commands
                                        .SelectMany(f => f.MethodsInvalid)
                                        .Where(f => this.IsMethodImplicit(f))
                                        .ToList();

            // Step4: Exists action specify with name in input: my-action --p1 2
            if (defaultsInvalidMethodsWithImplicitInput.Count == 0)
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
                    {
                        foreach (var method in cmd.MethodsInvalid.ToList())
                        {
                            if (method == defaultMethod)
                            {
                                cmd.RemoveInvalidMethod(method);
                                countRemove++;
                            }
                        }
                    }
                }
            }

            var allWasRemoved = countRemove == defaultsInvalidMethodsWithImplicitInput.Count;
            return allWasRemoved;
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

        private bool IsMethodImplicit(ActionParsed action)
        {
            return action.ActionMap.IsDefault && action.ArgumentRawOfAction == null;
        }

        private bool IsMethodImplicitAndNoArguments(ActionParsed action)
        {
            return this.IsMethodImplicit(action) && action.ActionMap.ArgumentsMaps.Empty();
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

        private bool AllRawAreEqualsOfArgumentParsed(IEnumerable<ArgumentParsed> argsA, IEnumerable<ArgumentParsed> argsB)
        {
            argsA = argsA.Where(f => f.ParsingType != ArgumentParsedType.DefaultValue);
            argsB = argsB.Where(f => f.ParsingType != ArgumentParsedType.DefaultValue);

            var countA = argsA.Count();
            var countB = argsB.Count();

            if (countA + countB == 0)
                return true;

            if (countA < countB)
                return false;

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
            var countB = argB.AllRaw.Count();

            if (countA + countB == 0)
                return true;

            if (countA < countB)
                return false;

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

        private ParseResult.Level GetLevelWithMethods(bool hasPropertyMap, IEnumerable<IGrouping<object, ActionParsed>> commandsGroups)
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
            
            // Step1: Create commands
            foreach (var commandMap in commandsMap)
            {
                var commandParse = new ParseResult.CommandParse();
                commandParse.Command = commandMap.Command;
                level.Add(commandParse);

                this.ParseProperties(commandMap.Properties, commandMap.Command.EnablePositionalArgs, commandParse, arguments);
            }

            // Step2: Create arguments references by raw
            var allArgsRawIsMapped = true;
            var argReferences = new List<ArgumentParsed>();
            foreach(var raw in arguments)
            {
                if (argReferences.Empty(f => f.AllRaw.Any(r => r == raw)))
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
            }

            // Step3: Clean commands if all valid raw was found
            if (allArgsRawIsMapped && argReferences.Count > 0)
            {
                foreach (var cmd in level.Commands)
                {
                    // Remove all invalid properties because all input was mapped 
                    // and all are valid
                    var propsInvalidWithoutRequireds = cmd.PropertiesInvalid.Where(f => f.ParsingType != ArgumentParsedType.HasNoInput).ToList();
                    foreach (var invalid in propsInvalidWithoutRequireds)
                        cmd.RemovePropertyInvalid(invalid);

                    // Remove all valid properties that are imcompatible with
                    // reference
                    var props = cmd.Properties.Where(f=>f.ParsingType != ArgumentParsedType.DefaultValue).ToList();
                    foreach (var arg in props)
                    {
                        var isCompatibleWithSomeRefs = argReferences.Any(argRef => this.AllRawAreEqualsInArgumentParsed(argRef, arg));
                        if (!isCompatibleWithSomeRefs)
                            cmd.RemoveProperty(arg);
                    }
                }
            }
            else
            {
                // Step4: Remove all valid properties that was mapped if not exists invalid 
                //        properties. This is necessary to force "not found" error when
                //        some argument raw doesn't was found.
                foreach (var cmd in level.Commands)
                {
                    if (cmd.PropertiesInvalid.Empty())
                    {
                        var props = cmd.Properties.Where(f => f.ParsingType != ArgumentParsedType.DefaultValue).ToList();
                        foreach (var arg in props)
                            cmd.RemoveProperty(arg);
                    }
                }
            }

            return level;
        }

        private ParseResult.Level GetLevelWithPropertiesDefaultOrRequired(IEnumerable<ParseResult.Level> levels, IEnumerable<CommandMap> map)
        {
            ParseResult.Level level = null;

            var properties = levels.SelectMany(l => l.Commands.SelectMany(c => c.Properties));
            var propertiesInvalid = levels.SelectMany(l => l.Commands.SelectMany(c => c.PropertiesInvalid));
            var listMapsRequiredOrDefaultNotMapped = new Dictionary<CommandBase, List<ArgumentMap>>();
            foreach (var cmd in map)
            {
                foreach (var propMap in cmd.Properties)
                {
                    if (propMap.HasDefaultValue || !propMap.IsOptional)
                    {
                        var notExistsInInvalid = propertiesInvalid.Empty(f => f.Map == propMap);
                        var notExistsInValid = properties.Empty(f => f.Map == propMap);
                        if (notExistsInInvalid && notExistsInValid)
                        {
                            if (!listMapsRequiredOrDefaultNotMapped.ContainsKey(cmd.Command))
                                listMapsRequiredOrDefaultNotMapped[cmd.Command] = new List<ArgumentMap>();

                            listMapsRequiredOrDefaultNotMapped[cmd.Command].Add(propMap);
                        }
                    }
                }
            }

            if (listMapsRequiredOrDefaultNotMapped.Any())
            {
                level = new ParseResult.Level();

                foreach (var keyValue in listMapsRequiredOrDefaultNotMapped)
                {
                    var defaultsOrRequireds = argumentParser.CreateArgumentsDefaultValueOrRequired(keyValue.Value);
                    argumentParser.SetState(defaultsOrRequireds);

                    var commandParse = new ParseResult.CommandParse();
                    commandParse.Command = keyValue.Key;
                    commandParse.AddProperties(defaultsOrRequireds.Where(f=>f.ParsingType ==  ArgumentParsedType.DefaultValue));
                    commandParse.AddPropertiesInvalid(defaultsOrRequireds.Where(f=>f.ParsingType ==  ArgumentParsedType.HasNoInput));
                    level.Add(commandParse);
                }
            }

            return level;
        }

        private void OrganizeLevelsSequence(ParseResult parseResult)
        {
            var i = 0;
            foreach (var level in parseResult.Levels)
                level.LevelNumber = i++;
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

            if (methodsValids.Empty())
                return null;

            // never has error because here only valid methods
            bool isBestMethodButHasError;
            var bestMethodInLevel = this.GetBestMethod(methodsValids, out isBestMethodButHasError);

            return bestMethodInLevel;
        }

        /// <summary>
        /// Find the best valid property reference by raw. 
        /// OBS: This rule does not exist for the methods by which the parameters 
        /// of the best methods are already references.
        /// </summary>
        /// <param name="level">Level to search</param>
        /// <param name="raw">Raw reference</param>
        /// <returns>The best argument parsed</returns>
        private ArgumentParsed GetFirstValidArgumentParsedByRawInLevel(ParseResult.Level level, ArgumentRaw raw)
        {
            var list = new List<ArgumentParsed>();

            foreach (var cmd in level.Commands)
            {
                foreach (var prop in cmd.Properties)
                {
                    if (prop.AllRaw.Any() && prop.AllRaw.First() == raw)
                        list.Add(prop);
                }
            }

            return list.OrderByDescending(a => a.AllRaw.Count()).FirstOrDefault();
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
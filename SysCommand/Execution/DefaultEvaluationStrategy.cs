using SysCommand.Parser;
using System.Collections.Generic;
using System.Linq;
using System;

namespace SysCommand
{
    public class DefaultEvaluationStrategy : IEvaluationStrategy
    {
        public Action<IMember> OnInvoke { get; set; }

        public virtual ParseResult Parse(string[] args, IEnumerable<CommandMap> commandsMap, bool enableMultiAction)
        {
            var parseResult = new ParseResult();
            
            var allMethodsMaps = commandsMap.GetMethods();
            var argumentsRaw = CommandParser.ParseArgumentsRaw(args, allMethodsMaps);

            IEnumerable<ArgumentRaw> initialExtraArguments;
            var methodsParsed = CommandParser.ParseActionMapped(argumentsRaw, enableMultiAction, allMethodsMaps, out initialExtraArguments).ToList();
            //var extrasArguments = new List<ArgumentRaw>(initialExtraArguments);

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

                    var commandsGroups = levelGroup.GroupBy(f => f.ActionMap.Source);
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

                        // CREATE PROPERTIES
                        // in this part of the code the method is 100% valid
                        // but can be exists extra arguments that are used 
                        // with properties inputs.
                        var argumentsExtras = bestMethod.ArgumentsExtras.SelectMany(f => f.AllRaw).ToList();
                        var commandMap = commandsMap.First(f => f.Command == commandParse.Command);
                        this.ParseProperties(commandMap, commandParse, argumentsExtras);

                        // CREATE PROPERTIES
                        // in this part of the code the method is 100% valid
                        // but can be exists extra arguments that are used 
                        // with properties inputs.
                        //var extrasArgumentsMethod = bestMethod.ArgumentsExtras.SelectMany(f => f.AllRaw).ToList();
                        //extrasArguments.AddRange(extrasArgumentsMethod);
                        ////var commandMap = commandsMap.First(f => f.Command == commandParse.Command);
                        //this.ParseProperties(commandMap, commandParse, argumentsExtras);
                    }
                }
            }

            // (args.Length == 0 && !hasMethodsParsed): This code is to prevent properties
            // that are required but dosen't exists args
            // Test: Test17_RequiredNoArgsAnd1CommandWith1PropertyObrigatory
            //var hasExtras = extrasArguments.Any();
            //if (hasExtras || (args.Length == 0 && !hasMethodsParsed))
            //{
            //    var level = new ParseResult.Level();
            //    parseResult.Insert(0, level);

            //    foreach (var commandMap in commandsMap)
            //    {
            //        var commandParse = new ParseResult.CommandParse();
            //        commandParse.Command = commandMap.Command;
            //        level.Add(commandParse);

            //        this.ParseProperties(commandMap, commandParse, extrasArguments);
            //    }
            //}

            // organize level number
            var i = 0;
            foreach(var level in parseResult.Levels)
                level.LevelNumber = i++;

            return parseResult;
        }

        public EvaluateResult Evaluate(string[] args, IEnumerable<CommandMap> maps, ParseResult parseResult)
        {
            var evaluateResult = new EvaluateResult();
            evaluateResult.Result = new Result<IMember>();

            var countLevelsValid = parseResult.Levels.Count(l => l.Commands.Any(c => c.IsValid));
            var countArgumentRequired = parseResult.Levels.Count(l => l.Commands.Any(c => c.HasAnyArgumentRequired));

            var addMainMethod = new Dictionary<CommandBase, bool>();

            if (countLevelsValid > 0 && parseResult.Levels.Count() == countLevelsValid && countArgumentRequired == 0)
            {
                foreach(var level in parseResult.Levels)
                {
                    var commandsParseValid = level.Commands.Where(f => f.IsValid);
                    foreach (var commandParse in commandsParseValid)
                    {
                        if (commandParse.Properties.Any())
                        {
                            evaluateResult.Result.AddRange(commandParse.Properties.Select(f => new Property(f)));
                            addMainMethod[commandParse.Command] = true;
                        }

                        if (commandParse.Methods.Any())
                        {
                            evaluateResult.Result.AddRange(commandParse.Methods.Select(f => new Method(f)));
                        }
                    }
                }

                if (addMainMethod.Any())
                {
                    List<MethodMain> mains = new List<MethodMain>();
                    foreach (var command in addMainMethod)
                    {
                        var main = this.CreateMainMethod(command.Key);
                        if (main != null)
                            mains.Add(main);
                    }

                    evaluateResult.Result.InsertRange(0, mains);
                }

                var evaluateScope = new EvaluateScope()
                {
                    Args = args,
                    Maps = maps,
                    ParseResult = parseResult,
                    EvaluateResult = evaluateResult
                };

                foreach(var cmd in evaluateResult.Result.Select(f=> (CommandBase)f.Source))
                    cmd.EvaluateScope = evaluateScope;

                evaluateResult.Result.With<Property>().Invoke(this.OnInvoke);
                evaluateResult.Result.With<MethodMain>().Invoke(this.OnInvoke);
                evaluateResult.Result.With<Method>().Invoke(this.OnInvoke);

                evaluateResult.State = EvaluateState.Success;
            }
            else
            {
                var errors = this.CreateErrors(parseResult);
                evaluateResult.Errors = errors;

                var hasMethodsInvalid = false;
                var hasMethodsValid = false;
                var hasPropertyValid = false;
                bool? allPropertiesNotExists = null;

                foreach (var level in parseResult.Levels)
                {
                    foreach (var cmd in level.Commands)
                    {
                        if (cmd.Methods.Any())
                            hasMethodsValid = true;
                        if (cmd.MethodsInvalid.Any())
                            hasMethodsInvalid = true;
                        if (cmd.Properties.Any())
                            hasPropertyValid = true;

                        var continueTrue = allPropertiesNotExists == null || allPropertiesNotExists == true;
                        if (continueTrue && cmd.PropertiesInvalid.All(f => f.MappingType == ArgumentMappingType.NotMapped))
                            allPropertiesNotExists = true;
                        else
                            allPropertiesNotExists = false;
                    }
                }

                var hasMember = hasMethodsValid || hasMethodsInvalid || hasPropertyValid;

                if (!hasMember && (allPropertiesNotExists == null || allPropertiesNotExists == true))
                    evaluateResult.State = EvaluateState.NotFound;
                else
                    evaluateResult.State = EvaluateState.HasError;
            }

            return evaluateResult;
        }

        private bool HasAnyMember(ParseResult parseResult)
        {
            foreach (var level in parseResult.Levels)
            {
                foreach (var cmd in level.Commands)
                    if (cmd.Methods.Any() || cmd.MethodsInvalid.Any()
                        || cmd.Properties.Any() || cmd.PropertiesInvalid.Any())
                        return true;
            }

            return false;
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

        private ActionMapped GetBestMethod(IEnumerable<ActionMapped> methods, out bool isBestMethodButHasError)
        {
            var candidates = methods
                    .Select(m => new
                    {
                        method = m,
                        countParameters = m.ActionMap.ArgumentsMaps.Count(),
                        countMappedParameters = m.Arguments.Count(a => a.IsMapped),
                        countValidParameters = m.Arguments.Count(a => a.MappingStates.HasFlag(ArgumentMappingState.Valid))
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
            var parseds = CommandParser.ParseArgumentMapped(argumentsRaw, commandMap.Command.EnablePositionalArgs, commandMap.Properties);
            commandParse.AddProperties(parseds.Where(f => f.MappingStates.HasFlag(ArgumentMappingState.Valid)));

            // Don't considere invalid args in this situation:
            // -> ArgumentMappingType.HasNoInput && ArgumentMappingState.ArgumentIsNotRequired
            // "ArgumentMappingType.HasNoInput": Means that args don't have input.
            // "ArgumentMappingState.ArgumentIsNotRequired": Means that args is optional.
            // in this situation the args is not consider invalid.
            commandParse.AddPropertiesInvalid(parseds.Where(f => f.MappingStates.HasFlag(ArgumentMappingState.IsInvalid) && !f.MappingStates.HasFlag(ArgumentMappingState.ArgumentIsNotRequired)));
        }

        private MethodMain CreateMainMethod(object command)
        {
            var mainMethod = command.GetType().GetMethods().Where(f => f.Name.ToLower() == CommandParser.MAIN_METHOD_NAME && f.GetParameters().Length == 0).FirstOrDefault();
            if (mainMethod != null)
                return new MethodMain(mainMethod.Name, command, mainMethod);
            return null;
        }
    }
}
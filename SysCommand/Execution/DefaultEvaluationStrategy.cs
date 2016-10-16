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
            var initialExtraArguments = new List<ArgumentRaw>();
            var methodsParsed = CommandParser.ParseActionMapped(argumentsRaw, enableMultiAction, allMethodsMaps, initialExtraArguments).ToList();
            
            if (initialExtraArguments.Any())
            {
                var level = new ParseResult.Level();
                level.LevelNumber = 0;
                parseResult.Add(level);

                foreach (var commandMap in commandsMap)
                {
                    var commandParse = new ParseResult.CommandParse();
                    commandParse.Command = commandMap.Command;
                    level.Add(commandParse);

                    this.ParseProperties2(commandMap, commandParse, initialExtraArguments);
                }
            }

            // step1: there are methods that are candidates to be execute
            if (methodsParsed.Count > 0)
            {
                // step2: separate per level
                var levelsGroups = methodsParsed.GroupBy(f => f.Level).OrderBy(f => f.Key);
                foreach (var levelGroup in levelsGroups)
                {
                    var level = new ParseResult.Level();
                    level.LevelNumber = levelGroup.Key;
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
                        this.ParseProperties2(commandMap, commandParse, argumentsExtras);
                    }
                }
            }

            return parseResult;
        }

        public EvaluateResult Evaluate(string[] args, IEnumerable<CommandMap> maps, ParseResult parseResult)
        {
            var evaluateResult = new EvaluateResult();
            evaluateResult.Result = new Result<IMember>();

            var countLevelsValid = parseResult.Levels.Count(l => l.Commands.Any(c => c.IsValid));
            var addMainMethod = new Dictionary<CommandBase, bool>();

            if (parseResult.Levels.Count() == countLevelsValid)
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
                    foreach (var command in addMainMethod)
                    {
                        var main = this.CreateMainMethod(command.Key);
                        if (main != null)
                            evaluateResult.Result.Insert(0, main);
                    }
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
                evaluateResult.State = EvaluateState.HasError;
            }

            return evaluateResult;
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
        
        //private void ParseProperties(CommandMap commandMap, CommandParseLevelResult level, IEnumerable<ArgumentRaw> argumentsRaw)
        //{
        //    var argumentsRawProperties = new List<ArgumentRaw>();
        //    foreach (var extraRaw in argumentsRaw)
        //    {
        //        // clone ArgumentRaw
        //        var raw = new ArgumentRaw(extraRaw.Index, extraRaw.Name, extraRaw.ValueRaw, extraRaw.Value, extraRaw.Format, extraRaw.DelimiterArgument, extraRaw.DelimiterValueInName);
        //        argumentsRawProperties.Add(raw);
        //    }

        //    var propertiesParsed = CommandParser.ParseArgumentMapped(argumentsRawProperties, commandMap.Command.EnablePositionalArgs, commandMap.Properties);
        //    level.Properties.AddRange(propertiesParsed.Where(f => f.MappingStates.HasFlag(ArgumentMappingState.Valid)));

        //    // add all invalids but ignore arguments with the state 'not required"
        //    // because the arg kind can't be consider valid or invalid, 
        //    // is a neutro type.
        //    level.PropertiesInvalid.AddRange(propertiesParsed.Where(f => f.MappingStates.HasFlag(ArgumentMappingState.IsInvalid) && !f.MappingStates.HasFlag(ArgumentMappingState.ArgumentIsNotRequired)));
        //}

        private void ParseProperties2(CommandMap commandMap, ParseResult.CommandParse commandParse, IEnumerable<ArgumentRaw> argumentsRaw)
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
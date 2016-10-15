using SysCommand.Parser;
using System.Collections.Generic;
using System.Linq;
using System;

namespace SysCommand
{
    public class DefaultEvaluationStrategy : IEvaluationStrategy
    {
        public Action<IMember> OnInvoke { get; set; }

        public class ExecutionResult
        {
            public IEnumerable<ExecutionLevel> ExecutionLevels { get; internal set; }
            public Result<IMember> Result { get; internal set; }
            public EvaluateState State { get; internal set; }

            public ExecutionResult()
            {

            }
        }

        public virtual IEnumerable<ExecutionLevel> Parse(string[] args, IEnumerable<CommandMap> commandsMap, bool enableMultiAction)
        {
            var allMethodsMaps = commandsMap.GetMethods();
            var argumentsRaw = CommandParser.ParseArgumentsRaw(args, allMethodsMaps);
            var initialExtraArguments = new List<ArgumentRaw>();
            var methodsParsed = CommandParser.ParseActionMapped(argumentsRaw, enableMultiAction, allMethodsMaps, initialExtraArguments).ToList();

            var levels = new List<ExecutionLevel>();
            if (initialExtraArguments.Any())
            {
                var level = new ExecutionLevel();
                level.Level = 0;
                foreach(var commandMap in commandsMap)
                {
                    var commandParse = new CommandParse();
                    level.CommandsParse.Add(commandParse);

                    this.ParseProperties2(commandMap, commandParse, initialExtraArguments);
                }
            }

            // step1: there are methods that are candidates to be execute
            if (methodsParsed.Count > 0)
            {
                // step2: separate per level
                var groupsMethodsPerLevel = methodsParsed.GroupBy(f => f.Level).OrderBy(f => f.Key).ToList();
                foreach (var groupMethodsPerLevel in groupsMethodsPerLevel)
                {
                    var level = new ExecutionLevel();
                    level.Level = groupMethodsPerLevel.Key;
                    foreach(var methods in groupMethodsPerLevel)
                    {
                        var groupsMethodsPerCommand = methodsParsed.GroupBy(f => f.ActionMap.Source);
                        foreach (var groupMethodsPerCommand in groupsMethodsPerCommand)
                        {
                            var commandParse = new CommandParse();
                            commandParse.Command = (CommandBase)groupMethodsPerCommand.Key;
                            level.CommandsParse.Add(commandParse);

                            bool isBestMethodButHasError;
                            var bestMethod = this.GetBestMethod(groupMethodsPerLevel, out isBestMethodButHasError);

                            if (isBestMethodButHasError)
                                commandParse.MethodsInvalid.Add(bestMethod);
                            else
                                commandParse.Methods.Add(bestMethod);

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
            }

            return levels;
        }

        public ExecutionResult Evaluate(string[] args, IEnumerable<CommandMap> maps, IEnumerable<CommandParseResult> result)
        {
            var levels = new List<ExecutionLevel>();
            var countLevelsValid = levels.Count(l => l.CommandsParse.Any(c => c.IsValid));
            var addMainMethod = new Dictionary<CommandBase, bool>();

            if (levels.Count == countLevelsValid)
            {
                foreach(var level in levels)
                {
                    var commandsParseValid = level.CommandsParse.Where(f => f.IsValid);
                    foreach (var commandParse in commandsParseValid)
                    {
                        if (commandParse.Properties.Any())
                        {
                            commandParse.Result.AddRange(commandParse.Properties.Select(f => new Property(f)));
                            addMainMethod[commandParse.Command] = true;
                        }

                        if (commandParse.Methods.Any())
                        {
                            commandParse.Result.AddRange(commandParse.Methods.Select(f => new Method(f)));
                        }
                    }
                }
            }

            if (addMainMethod.Any())
            {
                foreach (var command in addMainMethod)
                {
                    var main = this.CreateMainMethod(command);
                    if (main != null)
                        command.MainResult = main;
                }
            }

            var resultInvoke = command.GetResult();
            resultInvoke.With<Property>().Invoke(this.OnInvoke);
            resultInvoke.With<MethodMain>().Invoke(this.OnInvoke);
            resultInvoke.With<Method>().Invoke(this.OnInvoke);

            var commandsValid = result.Where(f => f.IsValid);
            
            if (commandsValid.Any())
            { 
                foreach(var command in commandsValid)
                {
                    var hasValidProperty = false;
                    foreach (var level in command.Levels)
                    {
                        if (level.Properties.Any())
                        {
                            level.Result.AddRange(level.Properties.Select(f => new Property(f)));
                            hasValidProperty = true;
                        }

                        if (level.Methods.Any())
                        {
                            level.Result.AddRange(level.Methods.Select(f => new Method(f)));
                        }
                    }

                    if (hasValidProperty)
                    {
                        // if exists some method Main(...) with args then ignore the
                        // default method Main() without args;
                        //if (!level.Methods.Any(f => f.Name.ToLower() == CommandParser.MAIN_METHOD_NAME))
                        {
                            var main = this.CreateMainMethod(command.Command);
                            if (main != null)
                                command.MainResult = main;
                        }

                    }

                    var resultInvoke = command.GetResult();
                    resultInvoke.With<Property>().Invoke(this.OnInvoke);
                    resultInvoke.With<MethodMain>().Invoke(this.OnInvoke);
                    resultInvoke.With<Method>().Invoke(this.OnInvoke);
                }

                return EvaluateState.Success;
            }
            else
            {
                return EvaluateState.HasError;
            }

            // step1: get valid properties, methods and respective commands
            //var properties = result.With<Property>();
            //var propertiesValid = properties.WithValidProperties();
            
            //var methods = result.With<Method>();
            //var methodsValid = methods.WithValidMethods();

            //var commandsValid = new List<object>();
            //var commandsValidForProperties = propertiesValid.Select(f => f.ArgumentMapped.Map.Source);
            //var commandsValidForMethods = methodsValid.Select(f => f.ActionMapped.ActionMap.Source);

            //foreach(var cmd in commandsValidForProperties)
            //{ 
            //    if (!commandsValid.Contains(cmd))
            //        commandsValid.Add(cmd);
            //}

            //foreach (var cmd in commandsValidForMethods)
            //{
            //    if (!commandsValid.Contains(cmd))
            //        commandsValid.Add(cmd);
            //}

            //// step2: invoke valid properties
            //if (propertiesValid.Any())
            //    propertiesValid.Invoke(this.OnInvoke);

            //// step3: invoke main method if exists any valid member
            //var hasValid = propertiesValid.Any() || methodsValid.Any();
            //if (hasValid)
            //{
            //    var mainMethods = this.CreateMainMethods(commandsValid);
            //    result.AddRange(mainMethods);
            //    result.With<MethodMain>().Invoke(this.OnInvoke);
            //}

            //// step4: execute valid user methods and remove duplicites
            //if (methodsValid.Any())
            //    methodsValid.TrimDuplicate().Invoke(this.OnInvoke);

            //var notFoundActions = methods.Empty();
            //var notFoundArguments = properties.Empty() || properties.All(f => f.ArgumentMapped.IsMapped == false);
            //var existsActionsError = methodsValid.Empty() && methods.Any();

            //if (notFoundActions && notFoundArguments)
            //    return EvaluateState.NotFound;
            //else if (existsActionsError)
            //    return EvaluateState.HasInvalidMethod;

            //return EvaluateState.Success;
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
        
        private void ParseProperties(CommandMap commandMap, CommandParseLevelResult level, IEnumerable<ArgumentRaw> argumentsRaw)
        {
            var argumentsRawProperties = new List<ArgumentRaw>();
            foreach (var extraRaw in argumentsRaw)
            {
                // clone ArgumentRaw
                var raw = new ArgumentRaw(extraRaw.Index, extraRaw.Name, extraRaw.ValueRaw, extraRaw.Value, extraRaw.Format, extraRaw.DelimiterArgument, extraRaw.DelimiterValueInName);
                argumentsRawProperties.Add(raw);
            }

            var propertiesParsed = CommandParser.ParseArgumentMapped(argumentsRawProperties, commandMap.Command.EnablePositionalArgs, commandMap.Properties);
            level.Properties.AddRange(propertiesParsed.Where(f => f.MappingStates.HasFlag(ArgumentMappingState.Valid)));

            // add all invalids but ignore arguments with the state 'not required"
            // because the arg kind can't be consider valid or invalid, 
            // is a neutro type.
            level.PropertiesInvalid.AddRange(propertiesParsed.Where(f => f.MappingStates.HasFlag(ArgumentMappingState.IsInvalid) && !f.MappingStates.HasFlag(ArgumentMappingState.ArgumentIsNotRequired)));
        }

        private void ParseProperties2(CommandMap commandMap, CommandParse commandParse, IEnumerable<ArgumentRaw> argumentsRaw)
        {
            var parseds = CommandParser.ParseArgumentMapped(argumentsRaw, commandMap.Command.EnablePositionalArgs, commandMap.Properties);
            commandParse.Properties.AddRange(parseds.Where(f => f.MappingStates.HasFlag(ArgumentMappingState.Valid)));

            // Don't considere invalid args in this situation:
            // -> ArgumentMappingType.HasNoInput && ArgumentMappingState.ArgumentIsNotRequired
            // "ArgumentMappingType.HasNoInput": Means that args don't have input.
            // "ArgumentMappingState.ArgumentIsNotRequired": Means that args is optional.
            // in this situation the args is not consider invalid.
            commandParse.PropertiesInvalid.AddRange(parseds.Where(f => f.MappingStates.HasFlag(ArgumentMappingState.IsInvalid) && !f.MappingStates.HasFlag(ArgumentMappingState.ArgumentIsNotRequired)));
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
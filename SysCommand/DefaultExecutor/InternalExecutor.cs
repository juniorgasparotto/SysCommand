using System.Collections.Generic;
using System.Linq;
using System;
using SysCommand.Parsing;
using SysCommand.Helpers;
using SysCommand.Execution;

namespace SysCommand.DefaultExecutor
{
    internal class InternalExecutor
    {
        public ExecutionResult Execute(ParseResult parseResult, Action<IMemberResult, ExecutionScope> onInvoke)
        {
            // (1) prepare

            var results = new List<IMemberResult>();
            var executionResult = new ExecutionResult();
            executionResult.Results = results;

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
                            results.AddRange(commandParse.Properties.Select(f => new PropertyResult(f)));
                            addMainMethod[commandParse.Command] = true;
                        }

                        if (commandParse.Methods.Any())
                        {
                            results.AddRange(commandParse.Methods.Select(f => new MethodResult(f)));
                        }
                    }
                }

                if (addMainMethod.Any())
                {
                    var mainsList = new List<MethodMainResult>();
                    foreach (var command in addMainMethod)
                    {
                        var main = this.CreateMainMethod(command.Key);
                        if (main != null)
                            mainsList.Add(main);
                    }

                    results.InsertRange(0, mainsList);
                }

                var executionScope = new ExecutionScope(parseResult, executionResult);

                foreach(var cmd in executionResult.Results.Select(f=> (CommandBase)f.Target))
                    cmd.ExecutionScope = executionScope;

                // (2) execution
                var properties = executionResult.Results.With<PropertyResult>();
                var mains = executionResult.Results.With<MethodMainResult>();
                var methods = executionResult.Results.With<MethodResult>();
                
                foreach (var prop in properties)
                {
                    if (executionScope.IsStopped)
                        break;

                    if (onInvoke == null)
                        prop.Invoke();
                    else
                        onInvoke(prop, executionScope);
                }

                if (!executionScope.IsStopped)
                {
                    foreach (var main in mains)
                    {
                        if (executionScope.IsStopped)
                            break;
                        
                        if (onInvoke == null)
                            main.Invoke();
                        else
                            onInvoke(main, executionScope);
                    }
                }

                if (!executionScope.IsStopped)
                {
                    foreach (var method in methods)
                    {
                        if (executionScope.IsStopped)
                            break;

                        if (onInvoke == null)
                            method.Invoke();
                        else
                            onInvoke(method, executionScope);
                    }
                }

                executionResult.State = ExecutionState.Success;
            }
            else
            {
                var errors = this.CreateErrors(parseResult);
                executionResult.Errors = errors;

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
                        if (continueTrue && cmd.PropertiesInvalid.All(f => f.ParsingType == ArgumentParsedType.NotMapped))
                            allPropertiesNotExists = true;
                        else
                            allPropertiesNotExists = false;
                    }
                }

                var hasMember = hasMethodsValid || hasMethodsInvalid || hasPropertyValid;

                if (!hasMember && (allPropertiesNotExists == null || allPropertiesNotExists == true))
                    executionResult.State = ExecutionState.NotFound;
                else
                    executionResult.State = ExecutionState.HasError;
            }

            return executionResult;
        }

        private IEnumerable<ExecutionError> CreateErrors(ParseResult parseResult)
        {
            var levelsInvalid = 
                parseResult
               .Levels
               .Where(f => f.Commands.Empty(c => c.IsValid) || f.Commands.Any(c => c.HasAnyArgumentRequired));

            var commandsInvalids = levelsInvalid.SelectMany(f => f.Commands.Where(c => c.HasError));
            var groupsCommands = commandsInvalids.GroupBy(f => f.Command);

            var commandsErrors = new List<ExecutionError>();
            foreach (var groupCommand in groupsCommands)
            {
                var propertiesInvalid = new List<ArgumentParsed>();
                var methodsInvalid = new List<ActionParsed>();
                methodsInvalid.AddRange(groupCommand.SelectMany(f => f.MethodsInvalid));
                propertiesInvalid.AddRange(groupCommand.SelectMany(f => f.PropertiesInvalid));

                var commandError = new ExecutionError();
                commandError.Command = groupCommand.Key;
                commandError.MethodsInvalid = methodsInvalid;
                commandError.PropertiesInvalid = propertiesInvalid;
                commandsErrors.Add(commandError);
            }

            return commandsErrors;
        }

        private MethodMainResult CreateMainMethod(object command)
        {
            var mainMethod = command.GetType().GetMethods().Where(f => f.Name.ToLower() == Executor.MAIN_METHOD_NAME && f.GetParameters().Length == 0).FirstOrDefault();
            if (mainMethod != null)
                return new MethodMainResult(mainMethod.Name, command, mainMethod);
            return null;
        }
    }
}
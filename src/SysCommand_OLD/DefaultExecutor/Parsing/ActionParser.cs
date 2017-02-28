using System.Collections.Generic;
using System.Linq;
using SysCommand.Mapping;
using SysCommand.Parsing;

namespace SysCommand.DefaultExecutor
{
    public class ActionParser
    {
        private ArgumentParser argumentParser;

        public ActionParser(ArgumentParser argumentParser)
        {
            this.argumentParser = argumentParser;
        }

        public IEnumerable<ActionParsed> Parse(IEnumerable<ArgumentRaw> argumentsRaw, bool enableMultiAction, IEnumerable<ActionMap> maps, out IEnumerable<ArgumentRaw> initialExtraArguments)
        {
            var actionsMapped = new List<ActionParsed>();
            var mapsDefaults = maps.Where(map => map.IsDefault);

            var initialExtraArgumentsAux = new List<ArgumentRaw>();
            initialExtraArguments = initialExtraArgumentsAux;

            // map the actions that are default if has default arguments
            if (argumentsRaw.Count() == 0)
            {
                foreach (var map in mapsDefaults)
                {
                    var actionCallerDefault = new ActionParsed(map.MapName, map, null, 0);
                    actionsMapped.Add(actionCallerDefault);
                }
            }
            else
            {
                //var argumentsRawDefault = new List<ArgumentRaw>();
                List<ActionParsed> lastFounds = null;
                List<ActionParsed> defaultsCallers = null;
                bool continueSearchToNextAction = true;
                var index = 0;
                foreach (var argRaw in argumentsRaw)
                {
                    ArgumentRaw argRawAction = null;

                    // found all actions that has the same name (e.g: overrides methods).
                    // ** use ValueRaw because de Value property has the value scaped when is action
                    // ** and ValueRaw mantein the original value: 
                    // ** Value: \exists-method-scaped -> exists-method-scaped
                    // ** ValueRaw: \exists-method-scaped -> \exists-method-scaped
                    var founds = maps.Where(map => map.ActionName == argRaw.ValueRaw).ToList();

                    if (argRaw.Format == ArgumentFormat.Unnamed && founds.Count > 0 && continueSearchToNextAction)
                        argRawAction = argRaw;

                    // consider initial args as LEVEL 0
                    // $ --id 10 action 1
                    // 1) "--id 10"  : LEVEL 0
                    // 2) "action 1" : LEVEL 1
                    if (index == 0 && initialExtraArgumentsAux.Any())
                        index = 1;

                    if (argRawAction != null)
                    {
                        lastFounds = new List<ActionParsed>();
                        foreach (var actionMap in founds)
                        {
                            var actionCaller = new ActionParsed(actionMap.MapName, actionMap, argRaw, index);
                            lastFounds.Add(actionCaller);
                            actionsMapped.Add(actionCaller);
                        }
                        index++;
                    }
                    else if (lastFounds != null)
                    {
                        foreach (var actionMap in lastFounds)
                            actionMap.AddArgumentRaw(argRaw);
                    }
                    else if (defaultsCallers == null)
                    {
                        if (mapsDefaults.Any())
                        {
                            defaultsCallers = new List<ActionParsed>();
                            foreach (var map in mapsDefaults)
                            {
                                var actionCallerDefault = new ActionParsed(map.MapName, map, null, index);
                                actionCallerDefault.AddArgumentRaw(argRaw);
                                defaultsCallers.Add(actionCallerDefault);
                                actionsMapped.Add(actionCallerDefault);
                            }
                            index++;
                        }
                        else if(actionsMapped.Count == 0 && initialExtraArguments != null)
                        {
                            initialExtraArgumentsAux.Add(argRaw);
                        }
                    }
                    else if (defaultsCallers != null)
                    {
                        foreach (var caller in defaultsCallers)
                            caller.AddArgumentRaw(argRaw);
                    }

                    // disable the search to the next action
                    if (!enableMultiAction)
                        continueSearchToNextAction = false;
                }
            }

            foreach (var action in actionsMapped)
            {
                var argumentsMapped = this.argumentParser.Parse(action.GetArgumentsRaw(), action.ActionMap.EnablePositionalArgs, action.ActionMap.ArgumentsMaps);

                var argumentsExtras = argumentsMapped.Where(f => f.ParsingType == ArgumentParsedType.NotMapped);
                var arguments = argumentsMapped.Where(f => f.ParsingType != ArgumentParsedType.NotMapped);

                action.Arguments = arguments;
                action.ArgumentsExtras = argumentsExtras;
                action.ParsingStates = GetState(action);
            }

            return actionsMapped;
        }

        public ActionParsedState GetState(ActionParsed actionParsed)
        {
            ActionParsedState state = ActionParsedState.None;

            var countMap = actionParsed.ActionMap.ArgumentsMaps.Count();
            var countArgs = actionParsed.Arguments.Count();
            var countExtras = actionParsed.ArgumentsExtras.Count();
            var allValids = actionParsed.Arguments.All(f => f.ParsingStates.HasFlag(ArgumentParsedState.Valid));

            if (countMap == 0 && countArgs == 0 && countExtras == 0)
            {
                state |= ActionParsedState.Valid | ActionParsedState.NoArgumentsInMapAndInInput;
            }
            else
            {
                if (countExtras > 0)
                    state |= ActionParsedState.HasExtras;

                if (allValids)
                    state |= ActionParsedState.Valid;
                else
                    state |= ActionParsedState.IsInvalid;
            }

            return state;
        }

    }
}

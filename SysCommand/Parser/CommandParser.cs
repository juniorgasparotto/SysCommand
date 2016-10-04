using System.Collections.Generic;
using System.Linq;
using System;
using Fclp;
using System.Reflection;
using System.Text;
using System.Globalization;
using System.Collections;

namespace SysCommand.Parser
{
    public static class CommandParser
    {
        public const string MAIN_METHOD_NAME = "main";

        #region Mappings - Step1

        public static IEnumerable<ActionMap> GetActionsMapsFromSourceObject(object source, bool onlyWithAttribute = false, bool usePrefixInAllMethods = false, string prefix = null)
        {
            var maps = new List<ActionMap>();
            var methods = source.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(f => f.IsPublic && !f.IsSpecialName).ToArray();
            return GetActionsMapsFromSourceObject(source, methods, onlyWithAttribute, usePrefixInAllMethods, prefix);
        }

        public static IEnumerable<ActionMap> GetActionsMapsFromSourceObject(object source, MethodInfo[] methods, bool onlyWithAttribute = false, bool usePrefixInAllMethods = false, string prefix = null)
        {
            var maps = new List<ActionMap>();

            foreach (var method in methods)
            {
                var attribute = Attribute.GetCustomAttribute(method, typeof(ActionAttribute)) as ActionAttribute;

                var isMainMethod = method.Name.ToLower() == MAIN_METHOD_NAME;
                var countParameters = method.GetParameters().Length;

                // the main method, with zero arguments, can't be a action
                if (isMainMethod && countParameters == 0 && attribute == null)
                    continue;
                else if (isMainMethod && countParameters == 0 && attribute != null)
                    throw new Exception("The main method, with zero arguments, can be se a action. This method is reserved to be aways invoked in action.   it is defined.");

                var ignoredByWithoutAttr = onlyWithAttribute && attribute == null && !isMainMethod;
                var ignoredByMethod = attribute != null && attribute.Ignore;

                if (ignoredByWithoutAttr || ignoredByMethod)
                    continue;

                string actionNameRaw;
                string actionName;

                if (attribute != null && !string.IsNullOrWhiteSpace(attribute.Name))
                    actionNameRaw = attribute.Name;
                else
                    actionNameRaw = GetArgumentLongName(method.Name);

                var usePrefix = attribute == null ? true : attribute.UsePrefix;
                var usePrefixFinal = usePrefixInAllMethods && usePrefix;
                if (usePrefixFinal)
                {
                    if (string.IsNullOrWhiteSpace(prefix))
                        prefix = GetArgumentLongNameByType(source.GetType());

                    actionName = prefix + "-" + actionNameRaw;
                }
                else
                {
                    actionName = actionNameRaw;
                }

                var isDefaultAction = false;
                if (isMainMethod || (attribute != null && attribute.IsDefault))
                    isDefaultAction = true;

                var enablePositionalArgs = attribute == null ? true : attribute.EnablePositionalArgs;
                var argsMaps = GetArgumentsMapsFromMethod(source, method);
                maps.Add(new ActionMap(source, method, actionName, prefix, actionNameRaw, usePrefixFinal, isDefaultAction, enablePositionalArgs, argsMaps));
            }

            return maps;
        }

        public static IEnumerable<ArgumentMap> GetArgumentsMapsFromProperties(object source, bool onlyWithAttribute = false)
        {
            var properties = source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            return GetArgumentsMapsFromProperties(source, properties, onlyWithAttribute);
        }

        public static IEnumerable<ArgumentMap> GetArgumentsMapsFromProperties(object source, PropertyInfo[] properties, bool onlyWithAttribute = false)
        {
            var maps = new List<ArgumentMap>();
            foreach (PropertyInfo property in properties)
            {
                var attribute = Attribute.GetCustomAttribute(property, typeof(ArgumentAttribute)) as ArgumentAttribute;

                if (onlyWithAttribute && attribute == null)
                    continue;

                var map = GetArgumentMap(source, property);
                maps.Add(map);
            }

            ValidateArgumentsMaps(maps, source.GetType().Name);

            return GetArgumentsMapsOrderedByPosition(maps);
        }

        public static IEnumerable<ArgumentMap> GetArgumentsMapsFromMethod(object source, MethodInfo method)
        {
            var maps = new List<ArgumentMap>();
            var parameters = method.GetParameters();
            foreach (var parameter in parameters)
            {
                var map = GetArgumentMap(source, parameter);
                maps.Add(map);
            }

            ValidateArgumentsMaps(maps, method.Name);

            return GetArgumentsMapsOrderedByPosition(maps);
        }

        public static ArgumentMap GetArgumentMap(object source, ParameterInfo parameter)
        {
            var attribute = Attribute.GetCustomAttribute(parameter, typeof(ArgumentAttribute)) as ArgumentAttribute;

            string longName = null;
            char? shortName = null;
            string helpText = null;
            bool showHelpComplement = false;
            int? position = null;
            bool hasPosition = false;
            bool isOptional = parameter.IsOptional;
            bool hasDefaultValue = parameter.HasDefaultValue;
            object defaultValue = parameter.DefaultValue;

            if (attribute != null)
            {
                longName = attribute.LongName;
                shortName = attribute.ShortName;
                helpText = attribute.Help;
                showHelpComplement = attribute.ShowHelpComplement;
                hasPosition = attribute.HasPosition;
                position = (int?)attribute.Position;
            }

            return GetArgumentMap(source, parameter, parameter.Name, parameter.ParameterType, longName, shortName, hasPosition, position, helpText, showHelpComplement, isOptional, hasDefaultValue, defaultValue);
        }

        public static ArgumentMap GetArgumentMap(object source, PropertyInfo property)
        {
            var attribute = Attribute.GetCustomAttribute(property, typeof(ArgumentAttribute)) as ArgumentAttribute;
            string longName = null;
            char? shortName = null;
            string helpText = null;
            bool showHelpComplement = false;
            int? position = null;
            bool hasPosition = false;
            bool isOptional = true;
            bool hasDefaultValue = false;
            object defaultValue = null;

            if (attribute != null)
            {
                longName = attribute.LongName;
                shortName = attribute.ShortName;
                helpText = attribute.Help;
                showHelpComplement = attribute.ShowHelpComplement;
                hasPosition = attribute.HasPosition;
                position = (int?)attribute.Position;
                hasDefaultValue = attribute.HasDefaultValue;
                defaultValue = attribute.DefaultValue;
                isOptional = !attribute.IsRequired;
            }

            return GetArgumentMap(source, property, property.Name, property.PropertyType, longName, shortName, hasPosition, position, helpText, showHelpComplement, isOptional, hasDefaultValue, defaultValue);
        }

        public static ArgumentMap GetArgumentMap(object source, object propertyOrParameter, string mapName, Type mapType, string longName, char? shortName, bool hasPosition, int? position, string helpText, bool showHelpComplement, bool isOptional, bool hasDefaultValue, object defaultValue)
        {
            shortName = shortName == default(char) ? null : shortName;
            position = hasPosition ? position : null;

            if (longName == null || shortName == null)
            {
                if (mapName.Length == 1)
                {
                    shortName = mapName[0].ToString().ToLower()[0];
                }
                else
                {
                    longName = GetArgumentLongName(mapName);
                }
            }

            return new ArgumentMap(source, propertyOrParameter, mapName, longName, shortName, mapType, isOptional, hasDefaultValue, defaultValue, helpText, showHelpComplement, position);
        }

        public static IEnumerable<ArgumentMap> GetArgumentsMapsOrderedByPosition(IEnumerable<ArgumentMap> maps)
        {
            var mapsWithOrder = maps.Where(f => f.Position != null).OrderBy(f => f.Position).ToList();
            if (mapsWithOrder.Count > 0)
            {
                var mapsWithoutOrder = maps.Where(f => f.Position == null);
                var mapsReturn = new List<ArgumentMap>();
                mapsReturn.AddRange(mapsWithOrder);
                mapsReturn.AddRange(mapsWithoutOrder);
                return mapsReturn;
            }
            return maps;
        }

        public static void ValidateArgumentsMaps(IEnumerable<ArgumentMap> maps, string contextName)
        {
            foreach (var map in maps)
            {
                if (string.IsNullOrWhiteSpace(map.LongName) && string.IsNullOrWhiteSpace(map.ShortName.ToString()))
                    throw new Exception(string.Format("The map '{0}' there's no values in 'LongName' or 'ShortName' in method or class '{1}'", map.MapName, contextName));

                if (map.ShortName != null && (map.ShortName.Value.ToString().Trim().Length == 0 || !char.IsLetter(map.ShortName.Value)))
                    throw new Exception(string.Format("The map '{0}' has 'ShortName' invalid in method or class '{1}'", map.MapName, contextName));

                if (map.LongName != null && (map.LongName.Trim().Length == 0 || !char.IsLetter(map.LongName[0])))
                    throw new Exception(string.Format("The map '{0}' has 'LongName' invalid in method or class '{1}'", map.MapName, contextName));

                if (map.MapName != null)
                {
                    var duplicate = maps.FirstOrDefault(m => m.MapName == map.MapName && m != map);
                    if (duplicate != null)
                        throw new Exception(string.Format("The map '{0}' has the same 'MapName' on the map '{1}' in method or class '{2}'", map.MapName, duplicate.MapName, contextName));
                }
                else
                {
                    throw new Exception(string.Format("The method or object '{0}' has a map with no value in the property 'MapName'", contextName));
                }

                if (map.LongName != null)
                {
                    var duplicate = maps.FirstOrDefault(m => m.LongName == map.LongName && m != map);
                    if (duplicate != null)
                        throw new Exception(string.Format("The map '{0}' has the same 'LogName' on the map '{1}' in method or class '{2}'", map.MapName, duplicate.MapName, contextName));
                }

                if (map.ShortName != null)
                {
                    var duplicate = maps.FirstOrDefault(m => m.ShortName == map.ShortName && m != map);
                    if (duplicate != null)
                        throw new Exception(string.Format("The map '{0}' has the same 'ShortName' on the map '{1}' in method or class '{2}'", map.MapName, duplicate.MapName, contextName));
                }
            }
        }

        #endregion

        #region Converters - Step2

        public static IEnumerable<ArgumentRaw> ParseArgumentRaw(string[] args, IEnumerable<ActionMap> actionsMaps = null)
        {
            var argsItems = new List<ArgumentRaw>();
            var trueChar = '+';
            var falseChar = '-';
            var enumerator = args.GetEnumerator();

            var i = 0;
            while (enumerator.MoveNext())
            {
                string argDelimiter;

                // if is non parameter: [value] [123] [true] [\--scape-parameter] [--=] [--]
                var arg = (string)enumerator.Current;
                if (!IsArgument(arg, out argDelimiter))
                {
                    argsItems.Add(new ArgumentRaw(i, null, arg, GetValueScaped(arg, actionsMaps), ArgumentFormat.Unnamed, null, null));
                    i++;
                    continue;
                }

                string value;
                string valueRaw;
                string delimiterValueInName = null;
                bool hasValueInName = false;
                bool hasNoValue = false;

                // get left pos and rigth pos in the following situations:
                // -x=true     -> posLeft = "-x"; posRight = "true"
                // -x          -> posLeft = "-x"; posRight = null
                // --x:true    -> posLeft = "-x"; posRight = "true"
                // --x:=true   -> posLeft = "-x"; posRight = "=true"
                // --x=:true   -> posLeft = "-x"; posRight = ":true"                
                string posLeft = null;
                string posRight = null;
                foreach (var c in arg)
                {
                    if (delimiterValueInName == null)
                    {
                        if (c == '=')
                            delimiterValueInName = "=";
                        else if (c == ':')
                            delimiterValueInName = ":";
                        else
                            posLeft += c;
                    }
                    else
                    {
                        posRight += c;
                    }
                }

                var lastLeftChar = posLeft.Last();

                // check if exists "+" or "-": [-x+] or [-x-]
                if (lastLeftChar.In(trueChar, falseChar))
                {
                    posLeft = posLeft.Remove(posLeft.Length - 1);
                    value = lastLeftChar == trueChar ? trueChar.ToString() : falseChar.ToString();
                    hasValueInName = true;
                }
                else if (posRight == null)
                {
                    // get next arg
                    value = args.Length > (i + 1) ? args[i + 1] : null;

                    // ignore if next arg is parameter: [-xyz --next-parameter ...]
                    if (IsArgument(value))
                    {
                        value = null;
                        hasNoValue = true;
                    }
                    // jump the next arg if is value: [-xyz value]
                    else
                    {
                        enumerator.MoveNext();
                        i++;
                    }
                }
                else
                {
                    value = posRight;
                    hasValueInName = true;
                }

                // --name \--value -> scape value
                valueRaw = value;
                value = GetValueScaped(value, null);

                // remove "-":  -xyz  -> xyz
                // remove "--": --xyz -> xyz
                // remove "/":  /xyz  -> xyz
                string name = posLeft.Substring(argDelimiter.Length);

                // -x -> single parameter
                if (argDelimiter == "-")
                {
                    ArgumentFormat format;
                    if (hasNoValue)
                        format = ArgumentFormat.ShortNameAndNoValue;
                    else if (hasValueInName)
                        format = ArgumentFormat.ShortNameAndHasValueInName;
                    else
                        format = ArgumentFormat.ShortNameAndHasValue;

                    foreach (var n in name)
                        argsItems.Add(new ArgumentRaw(i, n.ToString(), valueRaw, value, format, argDelimiter, delimiterValueInName));
                }
                else
                {
                    ArgumentFormat format;

                    if (hasNoValue)
                        format = ArgumentFormat.LongNameAndNoValue;
                    else if (hasValueInName)
                        format = ArgumentFormat.LongNameAndHasValueInName;
                    else
                        format = ArgumentFormat.LongNameAndHasValue;

                    argsItems.Add(new ArgumentRaw(i, name, valueRaw, value, format, argDelimiter, delimiterValueInName));
                }

                i++;
            }

            return argsItems;
        }

        #endregion

        #region Converters - Step3

        public static IEnumerable<ArgumentMapped> ParseArgumentMapped(IEnumerable<ArgumentRaw> argumentsRaw, bool enablePositionalArgs, IEnumerable<ArgumentMap> maps)
        {
            if (argumentsRaw == null)
                throw new ArgumentNullException("argumentsRaw");

            if (maps == null)
                throw new ArgumentNullException("maps");

            var argumentsMappeds = new List<ArgumentMapped>();
            var mapsUseds = maps.ToList();

            var i = 0;
            using (IEnumerator<ArgumentRaw> enumerator = argumentsRaw.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var argRaw = enumerator.Current;
                    var map = mapsUseds.FirstOrDefault(m =>
                    {
                        if (argRaw.IsLongName)
                            return m.LongName == argRaw.Name;
                        else if (argRaw.IsShortName)
                            return m.ShortName == argRaw.Name[0];
                        else
                            return false;
                    });

                    if (enablePositionalArgs && map == null && argRaw.Format == ArgumentFormat.Unnamed)
                        map = mapsUseds.FirstOrDefault();
                    
                    if (map != null)
                    {
                        var argMapped = new ArgumentMapped(map.MapName, GetValueRaw(argRaw.Value), null, map.Type, map);
                        argMapped.AddRaw(argRaw);

                        if (argRaw.Format == ArgumentFormat.Unnamed)
                        {
                            argMapped.MappingType = ArgumentMappingType.Position;
                        }
                        else
                        {
                            argMapped.MappingType = ArgumentMappingType.Name;
                        }

                        argumentsMappeds.Add(argMapped);
                        ProcessArgumentMappedValue(enumerator, argumentsRaw, ref i, argRaw, map, argMapped);

                        mapsUseds.Remove(map);
                    }
                    else
                    {
                        var argMapped = new ArgumentMapped(argRaw.Name, GetValueRaw(argRaw.Value), argRaw.Value, typeof(string), null);
                        argMapped.AddRaw(argRaw);
                        argMapped.MappingType = ArgumentMappingType.NotMapped;
                        argumentsMappeds.Add(argMapped);
                    }

                    i++;
                }
            }

            foreach (var mapWithoutInput in mapsUseds)
            {
                var argMapped = new ArgumentMapped(mapWithoutInput.MapName, null, null, mapWithoutInput.Type, mapWithoutInput);
                argumentsMappeds.Add(argMapped);

                if (mapWithoutInput.HasDefaultValue)
                {
                    argMapped.MappingType = ArgumentMappingType.DefaultValue;
                    argMapped.Value = mapWithoutInput.DefaultValue;
                    argMapped.ValueParsed = mapWithoutInput.DefaultValue;
                }
                else
                {
                    argMapped.MappingType = ArgumentMappingType.HasNoInput;
                }
            }

            foreach (var arg in argumentsMappeds)
                arg.MappingStates = GetArgumentMappingState(arg, argumentsMappeds);

            return argumentsMappeds;
        }

        private static void ProcessArgumentMappedValue(IEnumerator<ArgumentRaw> enumerator, IEnumerable<ArgumentRaw> argumentsRaw, ref int i, ArgumentRaw argRaw, ArgumentMap map, ArgumentMapped argMapped)
        {
            if (argRaw.Value == null && map.Type != typeof(bool))
            {
                argMapped.Value = GetDefaultForType(map.Type);
            }
            else
            {
                var value = argRaw.Value;
                var hasInvalidInput = false;
                var hasUnsuporttedType = false;
                object valueConverted = null;
                var iDelegate = i;
                Action<int> actionConvertSuccess = (int position) =>
                {
                    if (position > 0)
                    {
                        enumerator.MoveNext();
                        iDelegate++;
                        argMapped.AddRaw(enumerator.Current);
                    }
                };

                if (IsEnum(map.Type))
                {
                    var values = new List<string>() { argRaw.Value };
                    values.AddRange(GetUnamedValues(argumentsRaw, i + 1));
                    var valueArray = values.ToArray();
                    argMapped.ValueParsed = GetValueRaw(valueArray);
                    valueConverted = TryConvertEnum(map.Type, valueArray, out hasInvalidInput, actionConvertSuccess);
                    i = iDelegate;
                }
                else if (map.Type != typeof(string) && typeof(IEnumerable).IsAssignableFrom(map.Type))
                {
                    var values = new List<string>() { argRaw.Value };
                    values.AddRange(GetUnamedValues(argumentsRaw, i + 1));
                    var valueArray = values.ToArray();
                    argMapped.ValueParsed = GetValueRaw(valueArray);

                    valueConverted = TryConvertCollection(map.Type, valueArray, out hasInvalidInput, out hasUnsuporttedType, actionConvertSuccess);
                    i = iDelegate;
                }
                else
                {
                    valueConverted = TryConvertPrimitives(map.Type, value, out hasInvalidInput, out hasUnsuporttedType);
                }

                argMapped.HasInvalidInput = hasInvalidInput;
                argMapped.HasUnsuporttedType = hasUnsuporttedType;

                if (!hasInvalidInput && !hasUnsuporttedType)
                    argMapped.Value = valueConverted;
            }
        }

        public static IEnumerable<ActionMapped> ParseActionMapped(IEnumerable<ArgumentRaw> argumentsRaw, bool enableMultiAction, IEnumerable<ActionMap> maps)
        {
            var actionsMapped = new List<ActionMapped>();
            var mapsDefaults = maps.Where(map => map.IsDefault);

            // map the actions that are default if has default arguments
            if (argumentsRaw.Count() == 0)
            {
                foreach (var map in mapsDefaults)
                {
                    var actionCallerDefault = new ActionMapped(map.MapName, map, null, 0);
                    actionsMapped.Add(actionCallerDefault);
                }
            }
            else
            {
                //var argumentsRawDefault = new List<ArgumentRaw>();
                List<ActionMapped> lastFounds = null;
                List<ActionMapped> defaultsCallers = null;
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

                    if (argRawAction != null)
                    {
                        lastFounds = new List<ActionMapped>();
                        foreach (var actionMap in founds)
                        {
                            var actionCaller = new ActionMapped(actionMap.MapName, actionMap, argRaw, index);
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
                        defaultsCallers = new List<ActionMapped>();
                        foreach (var map in mapsDefaults)
                        {
                            var actionCallerDefault = new ActionMapped(map.MapName, map, null, index);
                            actionCallerDefault.AddArgumentRaw(argRaw);
                            defaultsCallers.Add(actionCallerDefault);
                            actionsMapped.Add(actionCallerDefault);
                        }
                        index++;
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
                action.ArgumentsMapped = ParseArgumentMapped(action.GetArgumentsRaw(), action.ActionMap.EnablePositionalArgs, action.ActionMap.ArgumentsMaps);
                

                action.MappingStates = GetActionMappingState(action);
            }

            return actionsMapped;
        }

        public static ActionMappingState GetActionMappingState(ActionMapped actionMapped)
        {
            ActionMappingState state = ActionMappingState.None;

            var countMap = actionMapped.ActionMap.ArgumentsMaps.Count();
            var countMapped = actionMapped.ArgumentsMapped.Count();
            var allIsMapped = actionMapped.ArgumentsMapped.All(f => f.IsMappedOrHasDefaultValue);

            if (countMap == 0 && countMapped == 0)
            {
                state |= ActionMappingState.NoArgumentsInMapAndInInput;
            }
            else
            {
                //if (countMapped > countMap)
                //    state |= ActionMappingState.AmountOfMappedIsBiggerThenMaps;
                //else if (countMapped < countMap)
                //    state |= ActionMappingState.AmountOfMappedIsLessThanMaps;

                if (allIsMapped)
                    state |= ActionMappingState.AllArgumentsAreMapped;

                if (actionMapped.ArgumentsMapped.Any(arg => arg.MappingStates.HasFlag(ArgumentMappingState.IsInvalid)))
                    state |= ActionMappingState.IsInvalid;

                //if (actionMapped.ArgumentsMapped.Any(arg => arg.MappingType.In(ArgumentMappingType.NotMapped)))
                //    state |= ActionMappingState.IsInvalid;

                //if (actionMapped.ArgumentsMapped.Any(arg => arg.MappingType.In(ArgumentMappingType.HasNoInput)))
                //    state |= ActionMappingState.IsInvalid;

                //if (actionMapped.ArgumentsMapped.Any(arg => arg.HasUnsuporttedType))
                //    state |= ActionMappingState.IsInvalid;

                //if (actionMapped.ArgumentsMapped.Any(arg => arg.HasInvalidInput))
                //    state |= ActionMappingState.IsInvalid;
            }

            if (!state.HasFlag(ActionMappingState.IsInvalid))
                state |= ActionMappingState.Valid;

            return state;
        }

        //public static IEnumerable<ActionMapped> GetBestActionsMappedOrAll(IEnumerable<ActionMapped> actionsMapped)
        //{
        //    // get all actions that has all arguments inputed or with default value
        //    var candidates = actionsMapped.Where(f => f.MappingStates.HasFlag(ActionMappingState.AllArgumentsAreMapped | ActionMappingState.NoArgumentsInMapAndInInput)).ToList();

        //    /* In this cenaries all actions are valid if the input is valid foreach action
        //     * input: method 1 2 3
        //     * 1) Method(string a, string b, string c)
        //     * 2) Method(string a, string b, int c)
        //     * 4) Method(string a = null, float? b = null, decimal? c = null)
        //     * 5) Method(List<string> args)
        //     * 6) Method(List<int> args)
        //     * 7) Method(Enum1And2And3 enum)
        //     * << -- or -- >>
        //     * input: 1 2 3
        //     * 1) Main(string a, string b, string c)
        //     * 2) Main(string a, string b, int c)
        //     * 4) Default2(string a = null, float? b = null, decimal? c = null)
        //     * 5) Default3(List<string> args)
        //     * 6) Default4(List<int> args)
        //     * 7) Default5(Enum1And2And3 enum)
        //     */
        //    var valids = candidates.Where(f => f.MappingStates.HasFlag(ActionMappingState.Valid)).ToList();
        //    if (valids.Count > 0)
        //        return valids;

        //    return candidates;
        //}

        //public static object InvokeSourceMethodsFromActionsMappeds(ActionMapped actionMapped)
        //{
        //    var parameters = actionMapped.ArgumentsMapped.Where(f => f.IsMapped).ToDictionary(f => f.Name, f => f.Value);
        //    return actionMapped.ActionMap.Method.InvokeWithNamedParameters(actionMapped.ActionMap.Source, parameters);
        //}

        //public static void InvokeSourcePropertiesFromArgumentsMappeds(IEnumerable<ArgumentMapped> argumentsMapped)
        //{
        //    foreach(var arg in argumentsMapped)
        //    {
        //        if (arg.Map != null)
        //        {
        //            var property = (PropertyInfo)arg.Map.PropertyOrParameter;
        //            property.SetValue(arg.Map.Source, arg.Value);
        //        }
        //    }
        //}

        //public class ErrorArgumentMapped
        //{
        //    public ArgumentMapped ArgumentMapped { get; private set; }
        //    public ErrorCode Code { get; private set; }
        //    public string DefaultMessage { get; private set; }

        //    public ErrorArgumentMapped(ArgumentMapped argumentMapped, ErrorCode code, string defaultMessage)
        //    {
        //        this.ArgumentMapped = argumentMapped;
        //        this.Code = Code;
        //        this.DefaultMessage = DefaultMessage;
        //    }
        //}

        //public class ErrorActionMapped
        //{
        //    public IEnumerable<ErrorArgumentMapped> ArgumentsMapped { get; private set; }
        //    public ErrorActionMapped(IEnumerable<ErrorArgumentMapped> argumentsMapped)
        //    {
        //        this.ArgumentsMapped = argumentsMapped;
        //    }
        //}

        //public class Error
        //{
        //    public ErrorCode Code { get; set; }
        //    public string DefaultMessage { get; set; }
        //    public IEnumerable<ErrorArgumentMapped> GlobalArguments { get; set; }
        //    public IEnumerable<ErrorActionMapped> Actions { get; set; }
        //}

        public static ArgumentMappingState GetArgumentMappingState(ArgumentMapped arg, IEnumerable<ArgumentMapped> argumentsMapped)
        {
            //string userParameterName = arg.GetArgumentNameInputted();

            if (arg.MappingType == ArgumentMappingType.NotMapped)
            {
                if (arg.AllRaw.First().Format != ArgumentFormat.Unnamed)
                {
                    var hasMappedBefore = argumentsMapped.Any(f => f.IsMapped && f.Name == arg.Name && f != arg);
                    if (hasMappedBefore)
                        //errors.Add(new ErrorArgumentMapped(arg, ErrorCode.ArgumentAlreadyBeenSet,  string.Format("The argument '{0}' has already been set", userParameterName)));
                        return ArgumentMappingState.ArgumentAlreadyBeenSet | ArgumentMappingState.IsInvalid;
                    else
                        //errors.Add(new ErrorArgumentMapped(arg, ErrorCode.ArgumentNotExists, string.Format("The argument '{0}' does not exist", userParameterName)));
                        return ArgumentMappingState.ArgumentNotExistsByName | ArgumentMappingState.IsInvalid;
                }
                else
                {
                    //errors.Add(new ErrorArgumentMapped(arg, ErrorCode.ValueWithoutArgument, string.Format("Could not find an argument to the specified value: {0}", arg.Raw)));
                    return ArgumentMappingState.ArgumentNotExistsByValue | ArgumentMappingState.IsInvalid;
                }
            }
            else if (!arg.Map.IsOptional && arg.MappingType == ArgumentMappingType.HasNoInput)
            {
                //errors.Add(new ErrorArgumentMapped(arg, ErrorCode.ArgumentIsRequired, string.Format("The argument '{0}' is required", userParameterName)));
                return ArgumentMappingState.ArgumentIsRequired | ArgumentMappingState.IsInvalid;
            }
            else if (arg.IsMapped && arg.HasInvalidInput)
            {
                //errors.Add(new ErrorArgumentMapped(arg, ErrorCode.ArgumentIsInvalid, string.Format("The argument '{0}' is invalid", userParameterName)));
                return ArgumentMappingState.ArgumentHasInvalidInput | ArgumentMappingState.IsInvalid;
            }
            else if (arg.IsMapped && arg.HasUnsuporttedType)
            {
                //errors.Add(new ErrorArgumentMapped(arg, ErrorCode.ArgumentIsUnsupported, string.Format("The argument '{0}' is unsupported", userParameterName)));
                return ArgumentMappingState.ArgumentHasUnsupportedType | ArgumentMappingState.IsInvalid;
            }

            return ArgumentMappingState.Valid;
        }

        #endregion

        #region .NET types of converters - without exceptions

        public static object TryConvertEnum(Type type, string[] values, out bool hasInvalidInput, Action<int> successConvertCallback = null)
        {
            Type typeOriginal = GetTypeOrTypeOfNullable(type);
            int valueConverted = 0;
            var enumNames = Enum.GetNames(typeOriginal);
            var enumValues = Enum.GetValues(typeOriginal).Cast<int>().Select(f => f.ToString()).ToList();
            var hasFlags = typeOriginal.IsDefined(typeof(FlagsAttribute), false);
            hasInvalidInput = false;

            // get next enum value
            // --enum1 value1 value2 --enum2 value1
            // [current.....] [next] [next+1......]
            // [current]: Has arg "enum1"
            // [next]: Hasn't arg, then is possible value
            // [next+1]: Has arg, is not possible value
            for (var i = 0; values.Length > i; i++)
            {
                string enumValue = values[i];
                var currentIsIntegerValue = false;
                var currentIsNamedValue = enumNames.Any(f => f.ToLower() == enumValue.ToLower());

                // try in value
                if (!currentIsNamedValue)
                {
                    // if is char, try convert to int because can be a enum of char
                    if (enumValue.Length == 1 && !char.IsDigit(enumValue[0]))
                        enumValue = ((int)enumValue[0]).ToString();

                    currentIsIntegerValue = enumValues.Any(f => f == enumValue);
                }

                if (currentIsNamedValue || currentIsIntegerValue)
                {
                    // (1 + 2) + (1 + 2) = 6
                    // (1 | 2) | (1 | 2) = 3
                    var valueParsed = (int)Enum.Parse(typeOriginal, enumValue, true);
                    if (valueConverted == 0)
                        valueConverted = valueParsed;
                    else
                        valueConverted |= valueParsed;

                    if (successConvertCallback != null)
                        successConvertCallback(i);

                    if (!hasFlags)
                        break;
                }
                else 
                {
                    // only is invalid when the first value is invalid.
                    if (i == 0)
                       hasInvalidInput = true;
                    break;
                }
            }

            if (!hasInvalidInput)
                return Enum.Parse(typeOriginal, valueConverted.ToString());

            return valueConverted;
        }

        public static object TryConvertCollection(Type type, string[] values, out bool hasInvalidInput, out bool hasUnsuporttedType, Action<int> successConvertCallback = null)
        {
            object list = null;
            var hasInvalidInputAux = false;
            var hasUnsuporttedTypeAux = false;
            hasInvalidInput = false;
            hasUnsuporttedType = false;

            var isList = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>) && type.GetGenericArguments().Length == 1;
            var isArray = type.IsArray && type.GetElementType() != null;
            if (isList || isArray)
            {
                Type typeList;
                if (isArray)
                    typeList = type.GetElementType();
                else
                    typeList = type.GetGenericArguments().FirstOrDefault();

                var iListRef = typeof(List<>);
                Type[] IListParam = { typeList };
                list = Activator.CreateInstance(iListRef.MakeGenericType(IListParam));

                for (var i = 0; values.Length > i; i++)
                {
                    var value = values[i];
                    var convertedValue = TryConvertPrimitives(typeList, value, out hasInvalidInputAux, out hasUnsuporttedTypeAux);
                    if (!hasInvalidInputAux && !hasUnsuporttedTypeAux)
                    {
                        list.GetType().GetMethod("Add").Invoke(list, new[] { convertedValue });
                        if (successConvertCallback != null)
                            successConvertCallback(i);
                    }
                    else
                    {
                        // only is invalid when the first value is invalid.
                        if (i == 0)
                        {
                            hasInvalidInput = hasInvalidInputAux;
                            hasUnsuporttedType = hasUnsuporttedTypeAux;
                            list = null;
                        }

                        break;
                    }
                }

                if (list != null && isArray)
                    list = list.GetType().GetMethod("ToArray").Invoke(list, null);
            }
            else
            {
                hasUnsuporttedType = true;
            }

            return list;
        }

        public static object TryConvertPrimitives(Type type, string value, out bool hasInvalidInput, out bool hasUnsuporttedType)
        {
            object valueConverted = null;
            hasInvalidInput = false;
            hasUnsuporttedType = false;
            Type typeOriginal = GetTypeOrTypeOfNullable(type);

            if (value == null && typeOriginal != typeof(bool))
            {
                valueConverted = GetDefaultForType(type);
            }
            else
            {
                if (typeOriginal == typeof(decimal))
                {
                    decimal valueType;
                    if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out valueType))
                        valueConverted = valueType;
                    else
                        hasInvalidInput = true;
                }
                else if (typeOriginal == typeof(int))
                {
                    int valueType;
                    if (int.TryParse(value, out valueType))
                        valueConverted = valueType;
                    else
                        hasInvalidInput = true;
                }
                else if (typeOriginal == typeof(double))
                {
                    double valueType;
                    if (double.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out valueType))
                        valueConverted = valueType;
                    else
                        hasInvalidInput = true;
                }
                else if (typeOriginal == typeof(DateTime))
                {
                    DateTime valueType;
                    if (DateTime.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles.None, out valueType))
                        valueConverted = valueType;
                    else
                        hasInvalidInput = true;
                }
                else if (typeOriginal == typeof(bool))
                {
                    bool valueType;
                    if (string.IsNullOrWhiteSpace(value))
                        valueConverted = true;
                    else if (value == "0" || value == "-")
                        valueConverted = false;
                    else if (value == "1" || value == "+")
                        valueConverted = true;
                    else if (bool.TryParse(value, out valueType))
                        valueConverted = valueType;
                    else
                        hasInvalidInput = true;
                }
                else if (typeOriginal == typeof(byte))
                {
                    byte valueType;
                    if (byte.TryParse(value, out valueType))
                        valueConverted = valueType;
                    else
                        hasInvalidInput = true;
                }
                else if (typeOriginal == typeof(short))
                {
                    short valueType;
                    if (short.TryParse(value, out valueType))
                        valueConverted = valueType;
                    else
                        hasInvalidInput = true;
                }
                else if (typeOriginal == typeof(ushort))
                {
                    ushort valueType;
                    if (ushort.TryParse(value, out valueType))
                        valueConverted = valueType;
                    else
                        hasInvalidInput = true;
                }
                else if (typeOriginal == typeof(long))
                {
                    long valueType;
                    if (long.TryParse(value, out valueType))
                        valueConverted = valueType;
                    else
                        hasInvalidInput = true;
                }
                else if (typeOriginal == typeof(ulong))
                {
                    ulong valueType;
                    if (ulong.TryParse(value, out valueType))
                        valueConverted = valueType;
                    else
                        hasInvalidInput = true;
                }
                else if (typeOriginal == typeof(float))
                {
                    float valueType;
                    if (float.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out valueType))
                        valueConverted = valueType;
                    else
                        hasInvalidInput = true;
                }
                else if (typeOriginal == typeof(char))
                {
                    char valueType;
                    if (char.TryParse(value, out valueType))
                        valueConverted = valueType;
                    else
                        hasInvalidInput = true;
                }
                else if (typeOriginal == typeof(uint))
                {
                    uint valueType;
                    if (uint.TryParse(value, out valueType))
                        valueConverted = valueType;
                    else
                        hasInvalidInput = true;
                }
                else if (typeOriginal == typeof(string))
                {
                    valueConverted = value;
                }
                else
                {
                    hasUnsuporttedType = true;
                }
            }

            return valueConverted;
        }

        #endregion

        #region Parse support

        public static IEnumerable<string> GetUnamedValues(IEnumerable<ArgumentRaw> argumentsRaw, int iStart)
        {
            // get nexts orphans values
            var count = argumentsRaw.Count();
            for (var iArgRaw = iStart; count > iArgRaw; iArgRaw++)
            {
                var elm = argumentsRaw.ElementAt(iArgRaw);
                if (elm.Format == ArgumentFormat.Unnamed)
                    yield return elm.Value;
                else
                    break;
            }
        }

        public static bool IsArgument(string value)
        {
            return GetArgumentDelimiterIfValidArgument(value) != null;
        }

        public static bool IsArgument(string value, out string argDelimiter)
        {
            argDelimiter = GetArgumentDelimiterIfValidArgument(value);
            return argDelimiter != null;
        }

        public static string GetArgumentDelimiterIfValidArgument(string value)
        {
            string argDelimiter = null;
            if (value != null && !value.In("-", "--", "/"))
            {   
                //var invalidArgStartName = new char[] { '=', ':','+', '-', '/', '\\', '*', '&', '%' };
                var char0 = value[0];
                var char1 = (value.Length > 1) ? value[1] : default(char);
                var char2 = (value.Length > 2) ? value[2] : default(char);

                // the following values are considered invalid args formats
                // --=a     --:a        --:
                // -=a      -:a
                // /=a      /:a
                // -2000    --2000      /0
                if (char0 == '-' && char1 == '-')
                {
                    //if (!char.IsDigit(char2) && !char2.In(invalidArgStartName))
                    if (char.IsLetter(char2))
                        argDelimiter = "--";
                }
                else if (char0 == '-' || char0 == '/')
                {
                    //if (!char.IsDigit(char1) && !char1.In(invalidArgStartName))
                    if (char.IsLetter(char1))
                        argDelimiter = "-";
                }
            }

            return argDelimiter;
        }

        public static string GetValueScaped(string value, IEnumerable<ActionMap> actionMaps)
        {
            if (AppHelpers.IsScaped(value))
            {
                string[] escapableEquals = null;

                //fix the method scape. E.g: 'save' exists but is scaped "\save" then remove the "\"
                if (actionMaps != null)
                    escapableEquals = actionMaps.Select(f => f.ActionName).ToArray();

                return GetValueScaped(value, new string[] { "-", "/" }, escapableEquals);
            }

            return value;

            //// "-"       = "-"
            //// "/"       = "/"
            //if (string.IsNullOrWhiteSpace(value) || value.Length <= 1)
            //    return value;

            //var char0 = value[0];
            //var char1 = value[1];
            //var char3 = value.Length > 2 ? value[2] : default(char);

            //// "\-"      = "-"
            //// "\-test"  = "-test"
            //var existsScape = char0 == '\\' && char1.In('-', '/');

            //// "\\-"     = "\-"
            //// "\\-test" = "\-test"
            //var existsScapeAndBackslashInValue = char0 == '\\' && char1 == '\\' && char3.In('-', '/');

            //if (existsScape || existsScapeAndBackslashInValue)
            //    value = value.Substring(1);

            //return value;
        }

        public static string GetValueScaped(string value, string[] reservedWordsToStartValue, string[] reservedFullWords)
        {
            // "-"       = "-"
            // "/"       = "/"
            if (string.IsNullOrWhiteSpace(value) || value.Length <= 1)
                return value;

            if (reservedWordsToStartValue != null)
            {
                foreach (var reservedStart in reservedWordsToStartValue)
                {
                    if (string.IsNullOrWhiteSpace(reservedStart))
                        throw new ArgumentException("The argument is invalid", "reservedWordsToStartValue");

                    // "\\-"     = "\-"
                    // "\\-test" = "\-test"
                    if (value.StartsWith("\\\\") && value.Substring(2).StartsWith(reservedStart))
                        return value.Substring(1);

                    // "\-"      = "-"
                    // "\-test"  = "-test"
                    if (value.StartsWith("\\") && value.Substring(1).StartsWith(reservedStart))
                        return value.Substring(1);
                }
            }

            if (reservedFullWords != null)
            {
                foreach (var reservedWord in reservedFullWords)
                {
                    if (string.IsNullOrWhiteSpace(reservedWord))
                        throw new ArgumentException("The argument is invalid", "reservedFullWords");

                    // "\\-"     = "\-"
                    // "\\-test" = "\-test"
                    if (value.StartsWith("\\\\") && value.Substring(2) == reservedWord)
                        return value.Substring(1);

                    // "\-"      = "-"
                    // "\-test"  = "-test"
                    if (value.StartsWith("\\") && value.Substring(1) == reservedWord)
                        return value.Substring(1);
                }
            }

            return value;
        }

        public static string GetValueRaw(params string[] values)
        {
            var hasString = false;
            var strBuilder = new StringBuilder();
            foreach (var v in values)
            {
                if (!string.IsNullOrEmpty(v))
                {
                    var value = v.Replace("\"", "\\\"");

                    hasString = true;
                    if (strBuilder.Length > 0)
                        strBuilder.Append(" ");

                    if (value.Contains(" "))
                    {
                        strBuilder.Append("\"");
                        strBuilder.Append(value);
                        strBuilder.Append("\"");
                    }
                    else
                    {
                        strBuilder.Append(value);
                    }
                }
            }

            return hasString ? strBuilder.ToString() : null;
        }

        private static string GetArgumentLongName(string name)
        {
            return AppHelpers.ToLowerSeparate(name, '-');
        }

        private static string GetArgumentLongNameByType(Type type)
        {
            var prefix = AppHelpers.ToLowerSeparate(type.Name, '-');
            var split = prefix.Split('-').ToList();
            if (split.Last() == "command")
            {
                split.RemoveAt(split.Count - 1);
                prefix = string.Join("-", split.ToArray());
            }

            return prefix;
        }
        
        #endregion

        #region Reflection helper

        private static bool IsEnum(Type type)
        {   
            return GetTypeOrTypeOfNullable(type).IsEnum;
        }

        private static Type GetTypeOrTypeOfNullable(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                return type.GetGenericArguments()[0];
            return type;
        }

        private static object GetDefaultForType(Type targetType)
        {
            return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
        }

        #endregion
    }
}

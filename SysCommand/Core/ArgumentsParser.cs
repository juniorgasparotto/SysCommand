using System.Collections.Generic;
using System.Linq;
using System;
using Fclp;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Globalization;
using System.Collections;

namespace SysCommand
{
    public static class ArgumentsParser
    {
        /// <summary>
        /// Map: a, b, c, d, e, f = 1
        /// Input: 1 -b -c -d - j
        /// Result expected:
        ///  1: Position
        /// -b: Name
        /// -c: Name
        /// -d: Name
        /// -e: HasNoInput
        /// -j: NotMapped
        /// -f: DefaultValue
        /// </summary>
        public enum ArgumentMappingType
        {
            Name,
            Position,
            DefaultValue,
            HasNoInput,
            NotMapped,
        }

        /// <summary>
        /// Input: 1 -a -b value1 -c+ --long --long2 value2 /long3:+
        /// Result expected:
        ///  1: Unnamed
        /// -a: ShortNameAndNoValue
        /// -b: ShortNameAndHasValue
        /// -c: ShortNameAndHasValueInName
        /// --long: LongNameAndNoValue
        /// --long2: LongNameAndHasValue
        /// --long3: LongNameAndHasValueInName
        /// </summary>
        public enum ArgumentFormat
        {
            Unnamed,
            ShortNameAndNoValue,
            ShortNameAndHasValue,
            ShortNameAndHasValueInName,
            LongNameAndNoValue,
            LongNameAndHasValue,
            LongNameAndHasValueInName,
        }

        public class ArgumentMap
        {
            public string MapName { get; private set; }
            public string LongName { get; private set; }
            public char? ShortName { get; private set; }
            public Type Type { get; private set; }
            public bool HasDefaultValue { get; private set; }
            public object DefaultValue { get; private set; }
            public bool IsOptional { get; private set; }
            public string HelpText { get; private set; }
            public bool ShowHelpComplement { get; private set; }
            public int? Position { get; private set; }

            public ArgumentMap(string mapName, string longName, char? shortName, Type type, bool isOptional, bool hasDefaultValue, object defaultValue, string helpText, bool showHelpComplement, int? position)
            {
                this.MapName = mapName;
                this.LongName = longName;
                this.ShortName = shortName;
                this.Type = type;
                this.IsOptional = isOptional;
                this.HasDefaultValue = hasDefaultValue;
                this.DefaultValue = defaultValue;
                this.HelpText = helpText;
                this.ShowHelpComplement = showHelpComplement;
                this.Position = position;
            }

            public override string ToString()
            {
                return "[" + this.MapName + ", " + this.Type + "]";
            }
        }

        public class ArgumentRaw
        {
            public string Name { get; set; }
            public string Value { get; set; }
            public ArgumentFormat Format { get; set; }
            public string DelimiterArgument { get; set; }
            public string DelimiterValueInName { get; set; }
            public string ValueRaw { get; set; }
            
            public bool IsShortName
            {
                get 
                {
                    switch (this.Format)
                    {
                        case ArgumentFormat.ShortNameAndHasValue:
                        case ArgumentFormat.ShortNameAndHasValueInName:
                        case ArgumentFormat.ShortNameAndNoValue:
                            return true;
                    }

                    return false;
                }
            }

            public bool IsLongName
            {
                get
                {
                    switch (this.Format)
                    {
                        case ArgumentFormat.LongNameAndHasValue:
                        case ArgumentFormat.LongNameAndHasValueInName:
                        case ArgumentFormat.LongNameAndNoValue:
                            return true;
                    }

                    return false;
                }
            }

            public ArgumentRaw(string name, string valueRaw, string value, ArgumentFormat format, string delimiterArgument, string delimiterValueInName)
            {
                this.Name = name;
                this.Value = value;
                this.ValueRaw = valueRaw;
                this.Format = format;
                this.DelimiterArgument = delimiterArgument;
                this.DelimiterValueInName = delimiterValueInName;
            }

            public override string ToString()
            {
                return "[" + this.Name + ", " + this.Value + "]";
            }

        }

        public class ArgumentMapped
        {
            private List<ArgumentRaw> allRaw;

            public string Name { get; set; }
            public object ValueParsed { get; set; }
            public object Value { get; set; }
            public bool HasUnsuporttedType { get; set; }
            public bool HasInvalidInput { get; set; }
            public Type Type { get; set; }
            public ArgumentMappingType MappingType { get; set; }
            public ArgumentMap Map { get; set; }

            public string Raw
            {
                get
                {
                    return ArgumentsParser.GetValueRaw(this.allRaw.Select(f => f.ValueRaw).ToArray());
                }
            }

            public IEnumerable<ArgumentRaw> AllRaw
            { 
                get 
                { 
                    return allRaw; 
                }
            }

            public bool IsMapped
            {
                get
                {
                    if (this.MappingType.In(ArgumentMappingType.Name, ArgumentMappingType.Position))
                        return true;
                    return false;
                }
            }

            public bool IsMappedOrHasDefaultValue
            {
                get
                {
                    if (this.IsMapped || this.MappingType == ArgumentMappingType.DefaultValue)
                        return true;
                    return false;
                }
            }

            public ArgumentMapped(string name, string valueParsed, string value, Type type, ArgumentMap map)
            {
                this.Name = name;
                this.Value = value;
                this.ValueParsed = valueParsed;
                this.Type = type;
                this.Map = map;
                this.allRaw = new List<ArgumentRaw>();
            }

            public void AddRaw(ArgumentRaw raw)
            {
                this.allRaw.Add(raw);
            }

            public override string ToString()
            {
                return "[" + this.Name + ", " + this.Value + "]";
            }

        }

        public class ActionMap
        {
            public string Name { get; private set; }
            public Type TypeReturn { get; private set; }
            public Type ParentClassType { get; private set; }
            public bool IsDefault { get; set; }
            public IEnumerable<ArgumentMap> ArgumentsMaps { get; private set; }

            public ActionMap(string name, Type typeReturn, Type parentClassType, bool isDefault, IEnumerable<ArgumentMap> argumentsMaps)
            {
                this.Name = name;
                this.TypeReturn = typeReturn;
                this.ParentClassType = parentClassType;
                this.ArgumentsMaps = argumentsMaps;
                this.IsDefault = isDefault;
            }

            public override string ToString()
            {
                return "[" + this.Name + ", " + this.ParentClassType + "]";
            }
        }

        public static IEnumerable<ArgumentRaw> Parser(string[] args)
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
                    argsItems.Add(new ArgumentRaw(null, arg, GetValueScaped(arg), ArgumentFormat.Unnamed, null, null));
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
                value = GetValueScaped(value);

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
                        argsItems.Add(new ArgumentRaw(n.ToString(), valueRaw, value, format, argDelimiter, delimiterValueInName));
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

                    argsItems.Add(new ArgumentRaw(name, valueRaw, value, format, argDelimiter, delimiterValueInName));
                }

                i++;
            }

            return argsItems;
        }

        public static IEnumerable<ArgumentMapped> Parser(IEnumerable<ArgumentRaw> argumentsRaw, bool enablePositionedArgs = true, params ArgumentMap[] maps)
        {
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

                    if (enablePositionedArgs && map == null && argRaw.Format == ArgumentFormat.Unnamed)
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
                        ParseArgument(enumerator, argumentsRaw, ref i, argRaw, map, argMapped);

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

            //string.Format("The type '{0}' not supported", map.Type.ToString());
            //var msgInputInvalid = "The input value is invalid.";

            return argumentsMappeds;
        }

        private static void ParseArgument(IEnumerator<ArgumentRaw> enumerator, IEnumerable<ArgumentRaw> argumentsRaw, ref int i, ArgumentRaw argRaw, ArgumentMap map, ArgumentMapped argMapped)
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
                    values.AddRange(GetValuesRawPositioned(argumentsRaw, i + 1));
                    var valueArray = values.ToArray();
                    argMapped.ValueParsed = GetValueRaw(valueArray);
                    valueConverted = TryConvertEnum(map.Type, valueArray, out hasInvalidInput, actionConvertSuccess);
                    i = iDelegate;
                }
                else if (map.Type != typeof(string) && typeof(IEnumerable).IsAssignableFrom(map.Type))
                {
                    var values = new List<string>() { argRaw.Value };
                    values.AddRange(GetValuesRawPositioned(argumentsRaw, i + 1));
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

        public static IEnumerable<string> GetValuesRawPositioned(IEnumerable<ArgumentRaw> argumentsRaw, int iStart)
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

        public static IEnumerable<ActionMap> GetActionsMapsFromType(Type type, bool onlyWithAttribute = false, bool canAddPrefixInAllMethods = false, string prefix = null)
        {
            var maps = new List<ActionMap>();
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(f => f.IsPublic && !f.IsSpecialName).ToArray();
            return GetActionsMapsFromType(type, methods, onlyWithAttribute, canAddPrefixInAllMethods, prefix);
        }

        public static IEnumerable<ActionMap> GetActionsMapsFromType(Type type, MethodInfo[] methods, bool onlyWithAttribute = false, bool canAddPrefixInAllMethods = false, string prefix = null)
        {
            var maps = new List<ActionMap>();
            foreach (var method in methods)
            {
                var attribute = Attribute.GetCustomAttribute(method, typeof(ActionAttribute)) as ActionAttribute;

                if (onlyWithAttribute && attribute == null)
                    continue;

                var actionName = "";
                if (attribute != null && !string.IsNullOrWhiteSpace(attribute.Name))
                    actionName = method.Name;
                else
                    actionName = AppHelpers.ToLowerSeparate(method.Name, '-');

                var canAddPrefix = attribute == null ? true : attribute.CanAddPrefix;
                if (canAddPrefixInAllMethods && canAddPrefix)
                    actionName = GetNameWithPrefix(actionName, type, prefix);

                var isDefaultAction = false;
                if (method.Name.ToLower() == "main" || (attribute != null && attribute.IsDefault == true))
                    isDefaultAction = true;

                var argsMaps = GetArgumentsMapsFromMethod(method);
                maps.Add(new ActionMap(method.Name, method.ReturnType, type, isDefaultAction, argsMaps));
            }

            return maps;
        }

        public static IEnumerable<ArgumentMap> GetArgumentsMapsFromProperties(Type type, bool onlyWithAttribute = false)
        {
            var properties = type.GetProperties();
            return GetArgumentsMapsFromProperties(type, properties, onlyWithAttribute);
        }

        public static IEnumerable<ArgumentMap> GetArgumentsMapsFromProperties(Type type, PropertyInfo[] properties, bool onlyWithAttribute = false)
        {
            var maps = new List<ArgumentMap>();
            foreach (PropertyInfo property in properties)
            {
                var attribute = Attribute.GetCustomAttribute(property, typeof(ArgumentAttribute)) as ArgumentAttribute;

                if (onlyWithAttribute && attribute == null)
                    continue;

                var map = GetArgumentMap(property);
                maps.Add(map);
            }

            ValidateArgumentsMaps(maps, type.Name);

            return GetArgumentsMapsOrderedByPosition(maps);
        }

        public static IEnumerable<ArgumentMap> GetArgumentsMapsFromMethod(MethodInfo method)
        {
            var maps = new List<ArgumentMap>();
            var parameters = method.GetParameters();
            foreach (var parameter in parameters)
            {
                var map = GetArgumentMap(parameter);
                maps.Add(map);
            }

            ValidateArgumentsMaps(maps, method.Name);

            return GetArgumentsMapsOrderedByPosition(maps);
        }

        public static ArgumentMap GetArgumentMap(ParameterInfo parameter)
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

            return GetArgumentMap(parameter.Name, parameter.ParameterType, longName, shortName, hasPosition, position, helpText, showHelpComplement, isOptional, hasDefaultValue, defaultValue);
        }

        public static ArgumentMap GetArgumentMap(PropertyInfo property)
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

            return GetArgumentMap(property.Name, property.PropertyType, longName, shortName, hasPosition, position, helpText, showHelpComplement, isOptional, hasDefaultValue, defaultValue);
        }

        public static ArgumentMap GetArgumentMap(string mapName, Type mapType, string longName, char? shortName, bool hasPosition, int? position, string helpText, bool showHelpComplement, bool isOptional, bool hasDefaultValue, object defaultValue)
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
                    longName = AppHelpers.ToLowerSeparate(mapName, '-');
                }
            }

            return new ArgumentMap(mapName, longName, shortName, mapType, isOptional, hasDefaultValue, defaultValue, helpText, showHelpComplement, position);
        }

        public static IEnumerable<ArgumentMap> GetArgumentsMapsOrderedByPosition(IEnumerable<ArgumentMap> maps)
        {
            var mapsWithOrder = maps.Where(f => f.Position != null).OrderBy(f=>f.Position).ToList();
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
            foreach(var map in maps)
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

        public static string GetValueScaped(string value)
        {
            // "-"       = "-"
            // "/"       = "/"
            if (string.IsNullOrWhiteSpace(value) || value.Length <= 1)
                return value;

            var char0 = value[0];
            var char1 = value[1];
            var char3 = value.Length > 2 ? value[2] : default(char);

            // "\-"      = "-"
            // "\-test"  = "-test"
            var existsScape = char0 == '\\' && char1.In('-', '/');

            // "\\-"     = "\-"
            // "\\-test" = "\-test"
            var existsScapeAndBackslashInValue = char0 == '\\' && char1 == '\\' && char3.In('-', '/');

            if (existsScape || existsScapeAndBackslashInValue)
                value = value.Substring(1);

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

        private static string GetNameWithPrefix(string name, Type type, string prefix)
        {
            if (string.IsNullOrWhiteSpace(prefix))
            {
                prefix = AppHelpers.ToLowerSeparate(type.Name, '-');
                var split = prefix.Split('-').ToList();
                if (split.Last() == "command")
                {
                    split.RemoveAt(split.Count - 1);
                    prefix = string.Join("-", split.ToArray());
                }
            }

            return prefix + "-" + name;
        }

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
    }
}

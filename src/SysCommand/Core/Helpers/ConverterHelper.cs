using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;
using System.Globalization;
using SysCommand.Compatibility;
using System.Reflection;

namespace SysCommand.Helpers
{
    internal static class ConverterHelper
    {
        public static object TryConvertEnum(Type type, string[] values, out bool hasInvalidInput, Action<int> successConvertCallback = null)
        {
            Type typeOriginal = ReflectionHelper.GetTypeOrTypeOfNullable(type);
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
            hasInvalidInput = false;
            hasUnsuporttedType = false;

            var isList = type.IsGenericType() && typeof(IEnumerable).IsAssignableFrom(type.GetGenericTypeDefinition()) && type.GetGenericArguments().Length == 1;
            var isArray = type.IsArray && type.GetElementType() != null;
            if (isList || isArray)
            {
                var typeList = isArray ? type.GetElementType() : type.GetGenericArguments().FirstOrDefault();
                
                var iListRef = typeof(List<>);
                Type[] listParam = { typeList };
                list = Activator.CreateInstance(iListRef.MakeGenericType(listParam));

                var methodAdd = list.GetType().GetMethod("Add");
                if (methodAdd == null || methodAdd.GetParameters().Length != 1)
                    throw new Exception($"Type '{list.GetType().FullName}' does not have a method named 'Add(T item)'");

                for (var i = 0; values.Length > i; i++)
                {
                    var value = values[i];
                    bool hasInvalidInputAux;
                    bool hasUnsuporttedTypeAux;

                    var convertedValue = TryConvertPrimitives(typeList, value, out hasInvalidInputAux, out hasUnsuporttedTypeAux);
                    if (!hasInvalidInputAux && !hasUnsuporttedTypeAux)
                    {
                        methodAdd.Invoke(list, new[] { convertedValue });
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
                {
                    var methodToArray = list.GetType().GetMethod("ToArray");
                    list = methodToArray.Invoke(list, null);
                }
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
            Type typeOriginal = ReflectionHelper.GetTypeOrTypeOfNullable(type);

            if (value == null && typeOriginal != typeof(bool))
            {
                valueConverted = ReflectionHelper.GetDefaultForType(type);
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
    }
}

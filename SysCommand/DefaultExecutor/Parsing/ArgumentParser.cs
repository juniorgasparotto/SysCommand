using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;
using System.Collections;
using SysCommand.Mapping;
using SysCommand.Utils;
using SysCommand.Parsing;

namespace SysCommand.DefaultExecutor
{
    public class ArgumentParser
    {
        public IEnumerable<ArgumentParsed> Parse(IEnumerable<ArgumentRaw> argumentsRaw, bool enablePositionalArgs, IEnumerable<ArgumentMap> maps)
        {
            if (argumentsRaw == null)
                throw new ArgumentNullException("argumentsRaw");

            if (maps == null)
                throw new ArgumentNullException("maps");

            var argumentsMappeds = new List<ArgumentParsed>();
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
                        var argMapped = new ArgumentParsed(map.MapName, ArgumentParsed.GetValueRaw(argRaw.Value), null, map.Type, map);
                        argMapped.AddRaw(argRaw);

                        if (argRaw.Format == ArgumentFormat.Unnamed)
                        {
                            argMapped.ParsingType = ArgumentParsedType.Position;
                        }
                        else
                        {
                            argMapped.ParsingType = ArgumentParsedType.Name;
                        }

                        argumentsMappeds.Add(argMapped);
                        ProcessValue(enumerator, argumentsRaw, ref i, argRaw, map, argMapped);

                        mapsUseds.Remove(map);
                    }
                    else
                    {
                        var argMapped = new ArgumentParsed(argRaw.Name, ArgumentParsed.GetValueRaw(argRaw.Value), argRaw.Value, typeof(string), null);
                        argMapped.AddRaw(argRaw);
                        argMapped.ParsingType = ArgumentParsedType.NotMapped;
                        argumentsMappeds.Add(argMapped);
                    }

                    i++;
                }
            }

            foreach (var mapWithoutInput in mapsUseds)
            {
                var argMapped = new ArgumentParsed(mapWithoutInput.MapName, null, null, mapWithoutInput.Type, mapWithoutInput);
                argumentsMappeds.Add(argMapped);

                if (mapWithoutInput.HasDefaultValue)
                {
                    argMapped.ParsingType = ArgumentParsedType.DefaultValue;
                    argMapped.Value = mapWithoutInput.DefaultValue;
                    argMapped.ValueParsed = mapWithoutInput.DefaultValue;
                }
                else
                {
                    argMapped.ParsingType = ArgumentParsedType.HasNoInput;
                }
            }

            foreach (var arg in argumentsMappeds)
                arg.ParsingStates = GetState(arg, argumentsMappeds);

            return argumentsMappeds;
        }

        public ArgumentParsedState GetState(ArgumentParsed arg, IEnumerable<ArgumentParsed> argumentsMapped)
        {
            if (arg.ParsingType == ArgumentParsedType.NotMapped)
            {
                if (arg.AllRaw.First().Format != ArgumentFormat.Unnamed)
                {
                    var hasMappedBefore = argumentsMapped.Any(f => f.IsMapped && arg.Name.In(f.Map.LongName, f.Map.ShortName.ToString()) && f != arg);
                    if (hasMappedBefore)
                        return ArgumentParsedState.ArgumentAlreadyBeenSet | ArgumentParsedState.IsInvalid;
                    else
                        return ArgumentParsedState.ArgumentNotExistsByName | ArgumentParsedState.IsInvalid;
                }
                else
                {
                    return ArgumentParsedState.ArgumentNotExistsByValue | ArgumentParsedState.IsInvalid;
                }
            }
            else if (!arg.Map.HasDefaultValue && arg.ParsingType == ArgumentParsedType.HasNoInput)
            {
                if (arg.Map.IsOptional)
                    return ArgumentParsedState.ArgumentIsNotRequired | ArgumentParsedState.IsInvalid;
                else
                    return ArgumentParsedState.ArgumentIsRequired | ArgumentParsedState.IsInvalid;
            }
            //else if (!arg.Map.IsOptional && arg.MappingType == ArgumentMappingType.HasNoInput)
            //{
            //    //errors.Add(new ErrorArgumentMapped(arg, ErrorCode.ArgumentIsRequired, string.Format("The argument '{0}' is required", userParameterName)));
            //    return ArgumentMappingState.ArgumentIsRequired | ArgumentMappingState.IsInvalid;
            //}
            else if (arg.IsMapped && arg.HasInvalidInput)
            {
                return ArgumentParsedState.ArgumentHasInvalidInput | ArgumentParsedState.IsInvalid;
            }
            else if (arg.IsMapped && arg.HasUnsuporttedType)
            {
                return ArgumentParsedState.ArgumentHasUnsupportedType | ArgumentParsedState.IsInvalid;
            }

            return ArgumentParsedState.Valid;
        }

        private void ProcessValue(IEnumerator<ArgumentRaw> enumerator, IEnumerable<ArgumentRaw> argumentsRaw, ref int i, ArgumentRaw argRaw, ArgumentMap map, ArgumentParsed argMapped)
        {
            if (argRaw.Value == null && map.Type != typeof(bool))
            {
                argMapped.Value = AppHelpers.GetDefaultForType(map.Type);
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

                if (AppHelpers.IsEnum(map.Type))
                {
                    var values = new List<string>() { argRaw.Value };
                    values.AddRange(GetUnamedValues(argumentsRaw, i + 1));
                    var valueArray = values.ToArray();
                    argMapped.ValueParsed = ArgumentParsed.GetValueRaw(valueArray);
                    valueConverted = Converters.TryConvertEnum(map.Type, valueArray, out hasInvalidInput, actionConvertSuccess);
                    i = iDelegate;
                }
                else if (map.Type != typeof(string) && typeof(IEnumerable).IsAssignableFrom(map.Type))
                {
                    var values = new List<string>() { argRaw.Value };
                    values.AddRange(GetUnamedValues(argumentsRaw, i + 1));
                    var valueArray = values.ToArray();
                    argMapped.ValueParsed = ArgumentParsed.GetValueRaw(valueArray);

                    valueConverted = Converters.TryConvertCollection(map.Type, valueArray, out hasInvalidInput, out hasUnsuporttedType, actionConvertSuccess);
                    i = iDelegate;
                }
                else
                {
                    valueConverted = Converters.TryConvertPrimitives(map.Type, value, out hasInvalidInput, out hasUnsuporttedType);
                }

                argMapped.HasInvalidInput = hasInvalidInput;
                argMapped.HasUnsuporttedType = hasUnsuporttedType;

                if (!hasInvalidInput && !hasUnsuporttedType)
                    argMapped.Value = valueConverted;
            }
        }

        public IEnumerable<string> GetUnamedValues(IEnumerable<ArgumentRaw> argumentsRaw, int iStart)
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
    }
}

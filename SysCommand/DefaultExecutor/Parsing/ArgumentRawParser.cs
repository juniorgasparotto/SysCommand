using System.Collections.Generic;
using System.Linq;
using System;
using SysCommand.Mapping;
using SysCommand.Helpers;
using SysCommand.Parsing;

namespace SysCommand.DefaultExecutor
{
    public class ArgumentRawParser
    {
        public IEnumerable<ArgumentRaw> Parse(string[] args, IEnumerable<ActionMap> actionsMaps = null)
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
                    if (IsArgument(value) || (actionsMaps != null && actionsMaps.Any(f => f.ActionName == value)))
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
                value = GetValueScaped(value, actionsMaps);

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

        public bool IsArgument(string value)
        {
            return GetDelimiterIfValidArgument(value) != null;
        }

        public bool IsArgument(string value, out string argDelimiter)
        {
            argDelimiter = GetDelimiterIfValidArgument(value);
            return argDelimiter != null;
        }

        public string GetDelimiterIfValidArgument(string value)
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

        public string GetValueScaped(string value, IEnumerable<ActionMap> actionMaps)
        {
            if (StringHelper.IsScaped(value))
            {
                string[] escapableEquals = null;

                //fix the method scape. E.g: 'save' exists but is scaped "\save" then remove the "\"
                if (actionMaps != null)
                    escapableEquals = actionMaps.Select(f => f.ActionName).ToArray();

                return GetValueScaped(value, new string[] { "-", "/" }, escapableEquals);
            }

            return value;
        }

        public string GetValueScaped(string value, string[] reservedWordsToStartValue, string[] reservedFullWords)
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
    }
}

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

namespace SysCommand
{
    public static class AppHelpers
    {
        #region reflection

        public static object InvokeWithNamedParameters(this MethodBase self, object obj, IDictionary<string, object> namedParameters)
        {
            if (namedParameters == null)
                return self.Invoke(obj, null);

            return self.Invoke(obj, MapParameters(self, namedParameters));
        }

        public static object[] MapParameters(MethodBase method, IDictionary<string, object> namedParameters)
        {
            string[] paramNames = method.GetParameters().Select(p => p.Name).ToArray();
            object[] parameters = new object[paramNames.Length];
            for (int i = 0; i < parameters.Length; ++i)
            {
                parameters[i] = Type.Missing;
            }
            foreach (var item in namedParameters)
            {
                var paramName = item.Key;
                var paramIndex = Array.IndexOf(paramNames, paramName);
                parameters[paramIndex] = item.Value;
            }
            return parameters;
        }

        public static string CSharpName(Type type, bool showFullName = false)
        {
            var sb = new StringBuilder();
            var name = showFullName ? type.FullName : type.Name;
            //return name;
            if (!type.IsGenericType || name.IndexOf('`') == -1) return name;
            sb.Append(name.Substring(0, name.IndexOf('`')));
            sb.Append("<");
            sb.Append(string.Join(", ", type.GetGenericArguments()
                                            .Select(t => CSharpName(t, showFullName))));
            sb.Append(">");
            return sb.ToString();
        }

        public static PropertyInfo GetPropertyInfo<TSource>(Expression<Func<TSource, object>> propertyLambda)
        {
            MemberExpression Exp = null;

            //this line is necessary, because sometimes the expression comes in as Convert(originalexpression)
            if (propertyLambda.Body is UnaryExpression)
            {
                var UnExp = (UnaryExpression)propertyLambda.Body;
                if (UnExp.Operand is MemberExpression)
                {
                    Exp = (MemberExpression)UnExp.Operand;
                }
                else
                    throw new ArgumentException();
            }
            else if (propertyLambda.Body is MemberExpression)
            {
                Exp = (MemberExpression)propertyLambda.Body;
            }
            else
            {
                throw new ArgumentException();
            }

            return (PropertyInfo)Exp.Member;
        }

        public static T Construct<T>(Type[] paramTypes, object[] paramValues)
        {
            Type t = typeof(T);

            ConstructorInfo ci = t.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, paramTypes, null);

            return (T)ci.Invoke(paramValues);
        }

        #endregion

        #region string

        public static string ToLowerSeparate(string str, char separate)
        {
            if (!string.IsNullOrWhiteSpace(str))
            {
                var newStr = "";
                for (var i = 0; i < str.Length; i++)
                {
                    var c = str[i];
                    if (i > 0 && separate != str[i - 1] && char.IsLetterOrDigit(str[i - 1]) && char.IsUpper(c) && !char.IsUpper(str[i - 1]))
                        newStr += separate + c.ToString().ToLower();
                    else
                        newStr += c.ToString().ToLower();
                }

                return newStr;
            }

            return str;
        }

        public static IEnumerable<string> ChunksWords(string str, int chunkSize)
        {
            str = str ?? "";

            int partLength = chunkSize;
            string sentence = str;
            string[] words = sentence.Split(' ');
            var parts = new Dictionary<int, string>();
            string part = string.Empty;
            int partCounter = 0;
            foreach (var word in words)
            {
                if (part.Length + word.Length < partLength)
                {
                    part += string.IsNullOrEmpty(part) ? word : " " + word;
                }
                else
                {
                    parts.Add(partCounter, part);
                    part = word;
                    partCounter++;
                }
            }
            parts.Add(partCounter, part);
            foreach (var item in parts)
            {
                yield return item.Value;
            }
        }

        public static IEnumerable<string> ChunksLetters(string str, int chunkSize)
        {
            for (int i = 0; i < str.Length; i += chunkSize)
                yield return str.Substring(i, Math.Min(chunkSize, str.Length - i));
        }

        /// <summary>
        /// Converts a List of string arrays to a string where each element in each line is correctly padded.
        /// Make sure that each array contains the same amount of elements!
        /// - Example without:
        /// Title Name Street
        /// Mr. Roman Sesamstreet
        /// Mrs. Claudia Abbey Road
        /// - Example with:
        /// Title   Name      Street
        /// Mr.     Roman     Sesamstreet
        /// Mrs.    Claudia   Abbey Road
        /// <param name="lines">List lines, where each line is an array of elements for that line.</param>
        /// <param name="padding">Additional padding between each element (default = 1)</param>
        /// </summary>
        public static string PadElementsInLines(List<string[]> lines, int padding = 1)
        {
            // Calculate maximum numbers for each element accross all lines
            var numElements = lines[0].Length;
            var maxValues = new int[numElements];
            for (int i = 0; i < numElements; i++)
            {
                maxValues[i] = lines.Max(x => x[i].Length) + padding;
            }
            var sb = new StringBuilder();
            // Build the output
            bool isFirst = true;
            foreach (var line in lines)
            {
                if (!isFirst)
                {
                    sb.AppendLine();
                }
                isFirst = false;
                for (int i = 0; i < line.Length; i++)
                {
                    var value = line[i];
                    // Append the value with padding of the maximum length of any value for this element
                    sb.Append(value.PadRight(maxValues[i]));
                }
            }
            return sb.ToString();
        }

        public static string ConcatFinalPhase(string phase, string phaseAdd)
        {
            if (string.IsNullOrWhiteSpace(phase))
                phase = "";

            phase = phase.Trim();
            var defaultValueStr = phaseAdd;

            if (phase.LastOrDefault() == '.')
                return phase + " " + defaultValueStr;
            else if (phase.Length > 0)
                return phase + ". " + defaultValueStr;
            else
                return defaultValueStr;
        }

        #endregion

        #region console application

        //public static Dictionary<string, string> ArgsToDictionary(string[] args)
        //{
        //    var dictionary = new Dictionary<string, string>();
        //    var trueChar = '+';
        //    var falseChar = '-';
        //    var enumerator = args.GetEnumerator();
        //    string value;

        //    var i = 0;
        //    while (enumerator.MoveNext())
        //    {
        //        // if is non parameter: [value] [123] [true] [\--scape-parameter]
        //        var arg = (string)enumerator.Current;
        //        if (!AppHelpers.IsArgumentFormat(arg))
        //        {
        //            dictionary.Add(i.ToString(), arg);
        //            i++;
        //            continue;
        //        }

        //        // -x=true     -> posLeft = "-x"; posRight = "true"
        //        // -x          -> posLeft = "-x"; posRight = null
        //        // --x:true    -> posLeft = "-x"; posRight = "true"
        //        // --x:=true   -> posLeft = "-x"; posRight = "=true"
        //        // --x=:true   -> posLeft = "-x"; posRight = ":true"
                
        //        var split = arg.Split(new char[] { '=', ':' }, 2, StringSplitOptions.RemoveEmptyEntries);
        //        var posLeft = split.Length > 0 ? split[0] : null;
        //        var posRight = split.Length > 1 ? split[1] : null;

        //        var char0 = (posLeft.Length > 0) ? posLeft[0] : default(char);
        //        var char1 = (posLeft.Length > 1) ? posLeft[1] : default(char);                
        //        var lastLeftChar = posLeft.Last();

        //        // check if exists "+" or "-": [-x+] or [-x-]
        //        if (lastLeftChar.In(trueChar, falseChar))
        //        {
        //            posLeft = posLeft.Remove(posLeft.Length - 1);
        //            value = lastLeftChar == trueChar ? "true" : "false";
        //        }
        //        else if (posRight == null)
        //        {
        //            // get next arg
        //            value = args.Length >= (i + 1) ? args[i + 1] : null;

        //            // ignore if next arg is parameter: [-xyz --next-parameter ...]
        //            if (AppHelpers.IsArgumentFormat(value))
        //                value = null;
        //            // jump the next arg if is value: [-xyz value]
        //            else
        //                enumerator.MoveNext();
        //        }
        //        else
        //        {
        //            value = posRight;
        //        }

        //        //if (string.IsNullOrWhiteSpace(value))
        //        //    value = "true";

        //        // -x -> single parameter
        //        if (char0 == '-' && char1 != '-')
        //        {
        //            // remove "-": -xyz -> xyz
        //            var keys = posLeft.Substring(1);
        //            foreach (var key in keys)
        //                dictionary[key.ToString()] = value;
        //        }
        //        else
        //        {
        //            string key = posLeft;

        //            // remove "--": --xyz -> xyz
        //            if (char0 == '-' && char1 == '-')
        //                key = key.Substring(1).Substring(1);
        //            // remove "/": /xyz -> xyz
        //            else
        //                key = arg.Substring(1);

        //            dictionary[key] = value;
        //        }

        //        i++;
        //    }

        //    return dictionary;
        //}

        public static Dictionary<string, string> ArgsToDictionary(string[] Args)
        {
            var Parameters = new Dictionary<string, string>();
            Regex Spliter = new Regex(@"^-{1,2}|^/|=|:", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            // original
            // Regex Remover = new Regex(@"^['""]?(.*?)['""]?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            // fix to command: "test \"blabla\""
            // the result without the fix is: test "blabla
            Regex Remover = new Regex(@"^['""]?(.*?)['""]?", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            string Parameter = null;
            string[] Parts;

            // Valid parameters forms:
            // {-,/,--}param{ ,=,:}((",')value(",'))
            // Examples: 
            // -param1 value1 --param2 /param3:"Test-:-work" 
            //   /param4=happy -param5 '--=nice=--'
            foreach (string Txt in Args)
            {
                // Look for new parameters (-,/ or --) and a
                // possible enclosed value (=,:)
                Parts = Spliter.Split(Txt, 3);

                switch (Parts.Length)
                {
                    // Found a value (for the last parameter 
                    // found (space separator))
                    case 1:
                        if (Parameter != null)
                        {
                            if (!Parameters.ContainsKey(Parameter))
                            {
                                Parts[0] =
                                    Remover.Replace(Parts[0], "$1");

                                Parameters.Add(Parameter, Parts[0]);
                            }
                            Parameter = null;
                        }
                        // else Error: no parameter waiting for a value (skipped)
                        break;

                    // Found just a parameter
                    case 2:
                        // The last parameter is still waiting. 
                        // With no value, set it to true.
                        if (Parameter != null)
                        {
                            if (!Parameters.ContainsKey(Parameter))
                                Parameters.Add(Parameter, "true");
                        }
                        Parameter = Parts[1];
                        break;

                    // Parameter with enclosed value
                    case 3:
                        // The last parameter is still waiting. 
                        // With no value, set it to true.
                        if (Parameter != null)
                        {
                            if (!Parameters.ContainsKey(Parameter))
                                Parameters.Add(Parameter, "true");
                        }

                        Parameter = Parts[1];

                        // Remove possible enclosing characters (",')
                        if (!Parameters.ContainsKey(Parameter))
                        {
                            Parts[2] = Remover.Replace(Parts[2], "$1");
                            Parameters.Add(Parameter, Parts[2]);
                        }

                        Parameter = null;
                        break;
                }
            }
            // In case a parameter is still waiting
            if (Parameter != null)
            {
                if (!Parameters.ContainsKey(Parameter))
                    Parameters.Add(Parameter, "true");
            }
            return Parameters;
        }

        [DllImport("shell32.dll", SetLastError = true)]
        static extern IntPtr CommandLineToArgvW([MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine, out int pNumArgs);

        public static string[] CommandLineToArgs(string commandLine)
        {
            int argc;
            var argv = CommandLineToArgvW(commandLine, out argc);
            if (argv == IntPtr.Zero)
                throw new System.ComponentModel.Win32Exception();
            try
            {
                var args = new string[argc];
                for (var i = 0; i < args.Length; i++)
                {
                    var p = Marshal.ReadIntPtr(argv, i * IntPtr.Size);
                    args[i] = Marshal.PtrToStringUni(p);
                }

                return args;
            }
            finally
            {
                Marshal.FreeHGlobal(argv);
            }
        }

        public static string GetConsoleHelper(Dictionary<string, string> helps, int padding = 4, int chunkSize = 80)
        {
            var helpPrintList = new List<string[]>();
            var helpFormated = new List<Tuple<string, string>>();

            foreach (var help in helps)
            {
                var split = AppHelpers.ChunksWords(help.Value, chunkSize).ToList();
                for (var i = 0; i < split.Count; i++)
                    if (i == 0)
                        helpFormated.Add(new Tuple<string, string>(help.Key, split[i]));
                    else
                        helpFormated.Add(new Tuple<string, string>("", split[i]));
            }

            foreach (var help in helpFormated)
            {
                helpPrintList.Add(new string[] { "", help.Item1, help.Item2 });
            }

            return AppHelpers.PadElementsInLines(helpPrintList, padding);
        }

        #endregion

        #region comparations

        public static bool In<TItem>(this TItem source, Func<TItem, TItem, bool> comparer, IEnumerable<TItem> items)
        {
            return items.Any(item => comparer(source, item));
        }

        public static bool In<TItem, T>(this TItem source, Func<TItem, T> selector, IEnumerable<TItem> items)
        {
            return items.Select(selector).Contains(selector(source));
        }

        public static bool In<T>(this T source, IEnumerable<T> items)
        {
            return items.Contains(source);
        }

        public static bool In<TItem>(this TItem source, Func<TItem, TItem, bool> comparer, params TItem[] items)
        {
            return source.In(comparer, (IEnumerable<TItem>)items);
        }

        public static bool In<TItem, T>(this TItem source, Func<TItem, T> selector, params TItem[] items)
        {
            return source.In(selector, (IEnumerable<TItem>)items);
        }

        public static bool In<T>(this T source, params T[] items)
        {
            return source.In((IEnumerable<T>)items);
        }

        #endregion

        #region syscommand

        public static bool IsArgumentFormat(string value)
        {
            var argsDelimiter = new char[] { '-', '/' };

            if (value == null || value.Length == 0)
                return false;

            return value[0].In(argsDelimiter);
        }

        public static bool IsActionFormat(string value)
        {
            if (value == null || value.Length == 0)
                return false;
            
            if (AppHelpers.IsScaped(value))
                return false;

            var isArgumentFormat = AppHelpers.IsArgumentFormat(value);
            if (App.Current.ActionCharPrefix == null)
                return !isArgumentFormat;
            else
                return !isArgumentFormat && value[0].In(App.Current.ActionCharPrefix.Value);
        }

        public static bool HasCharAtFirst(string value, char firstChar)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            return (value[0] == firstChar);
        }

        public static string RemoveFirstCharIfFound(string value, char charFind)
        {
            if (AppHelpers.IsScaped(value, charFind))
                return value.Substring(1);
            return value;
        }

        public static bool IsScaped(string value, char scapeChar = '\\')
        {
            return AppHelpers.HasCharAtFirst(value, scapeChar);
        }

        public static string RemoveScape(string value, char scapeChar = '\\')
        {
            return AppHelpers.RemoveFirstCharIfFound(value, scapeChar);
        }

        #endregion

        #region collections

        public static bool Empty<TSource>(this IEnumerable<TSource> source)
        {
            return !source.Any();
        }

        public static bool Empty<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return !source.Any(predicate);
        }

        #endregion
    }
}

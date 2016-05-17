using System.Collections.Generic;
using System.Linq;
using System;
using Fclp;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace SysCommand
{
    public static class AppHelpers
    {
        public static string[] StringToArgs(string value)
        {
            var result = value.Split('"')
                     .Select((element, index) => index % 2 == 0  // If even index
                                           ? element.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)  // Split the item
                                           : new string[] { element })  // Keep the entire item
                     .SelectMany(element => element).ToArray();

            return result;
        }

        public static object GetDefaultValueForArgs<TArgs>(Expression<Func<TArgs, object>> expression) where TArgs : class
        {
            return GetDefaultArgsPropValue<TArgs, object>(expression);
        }

        public static TProp GetDefaultArgsPropValue<TArgs, TProp>(Expression<Func<TArgs, object>> expression) where TArgs : class
        {
            var args = App.Current.GetConfig<ArgumentsHistory>().GetCommandArguments(App.Current.CurrentCommandName, typeof(TArgs)) as TArgs;
            
            if (args != null)
            {
                var prop = GetPropertyInfo<TArgs>(expression);
                return (TProp)prop.GetValue(args);
            }

            return default(TProp);
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

        public static IEnumerable<string> ChunksWords(string str, int chunkSize)
        {
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

        public static string GetConsoleHelper(Dictionary<string, string> helps, int chunkSize = 80)
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

            return AppHelpers.PadElementsInLines(helpPrintList, 2);
        }
    }
}

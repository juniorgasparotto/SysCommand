using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;

namespace SysCommand.Helpers
{
    public static class StringHelper
    {
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

        //public static IEnumerable<string> ChunksLetters(string str, int chunkSize)
        //{
        //    for (int i = 0; i < str.Length; i += chunkSize)
        //        yield return str.Substring(i, Math.Min(chunkSize, str.Length - i));
        //}

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
                    if (i == line.Length - 1)
                        sb.Append(value);
                    else
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

        public static bool HasCharAtFirst(string value, char firstChar)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            return (value[0] == firstChar);
        }

        public static bool IsScaped(string value, char scapeChar = '\\')
        {
            return StringHelper.HasCharAtFirst(value, scapeChar);
        }

        public static string ConcatFinalPhase(string help, object helpArgDescDefaultValue)
        {
            throw new NotImplementedException();
        }
    }
}

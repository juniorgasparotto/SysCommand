using System.Collections.Generic;
using System.Linq;

namespace SysCommand
{
    /// <summary>
    /// Class that simule the console parser
    /// </summary>
    public static class ConsoleInputParser
    {
        /// <summary>
        /// Parse string line to array of argument
        /// </summary>
        /// <param name="commandLine"></param>
        /// <returns>Array from string that represent arguments</returns>
        public static string[] Parse(string commandLine)
        {
            var args = new Arguments();
            char lastChar = default(char);
            var enumerator = commandLine.ToArray().GetEnumerator();

            while (enumerator.MoveNext())
            {
                var curChar = (char)enumerator.Current;
                var isQuoteChar = IsQuoteChar(curChar, out var quoteType);
                var lastIsScapeChar = IsScapeChar(lastChar);

                // check if is a phase cenary; have quotes 
                // 1) Is quote char (" or ')
                // 2) AND the last is not a scape char (\)
                var beginQuote = isQuoteChar && !lastIsScapeChar;

                if (!beginQuote)
                {
                    if (curChar == ' ')
                    {
                        args.CloseArgument();
                    }
                    else
                    {
                        if (lastIsScapeChar && isQuoteChar)
                            args.RemoveLastChar();

                        args.AddInCurrent(curChar);
                    }
                }
                else
                {
                    // if here, the current char is a quote " or '
                    while (enumerator.MoveNext())
                    {
                        curChar = (char)enumerator.Current;
                        if (curChar == quoteType)
                        {
                            if (IsScapeChar(lastChar))
                            {
                                args.RemoveLastChar();
                                args.AddInCurrent(quoteType);
                            }
                            else
                            {
                                // prevent empty quote
                                // cmd> --arg1     -> 1 args
                                // cmd> --arg1 ""  -> 2 args
                                if (args.GetCurrentChars().Length == 0)
                                    args.AddInCurrent('\0');
                                break;
                            }
                        }
                        else
                        {
                            args.AddInCurrent(curChar);
                        }

                        lastChar = curChar;
                    }
                }
                
                lastChar = curChar;
            }

            args.CloseArgument();

            return args.GetArguments();
        }

        private static bool IsSpaceChar(char lastChar)
        {
            return lastChar == ' ';
        }

        private static bool IsScapeChar(char lastChar)
        {
            return lastChar == '\\';
        }

        private static bool IsQuoteChar(char c, out char quoteChar)
        {
            if (c == '"' || c == '\'')
            {
                quoteChar = c;
                return true;
            }

            quoteChar = default(char);
            return false;
        }

        private class Arguments
        {
            private List<string> arguments = new List<string>();
            private List<char> chars = new List<char>();

            public char[] GetCurrentChars()
            {
                return chars.ToArray();
            }

            public void AddInCurrent(char c)
            {
                chars.Add(c);
            }

            public void CloseArgument()
            {
                if (chars.Count > 0)
                {
                    arguments.Add(string.Join(string.Empty, chars.Select(c => c == default(char) ? "" : c.ToString())));
                    chars.Clear();
                }
            }

            public string[] GetArguments()
            {
                return arguments.ToArray();
            }

            public void RemoveLastChar()
            {
                if (chars.Count > 0)
                    chars.RemoveAt(chars.Count - 1);
            }
        } 
    }
}
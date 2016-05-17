using System;
namespace SysCommand
{
    public class ConsoleWriter
    {
        public static string Verbose { get; set; }
        public static bool Quiet { get; set; }

        public static bool CheckIfWrite(string verb, bool forceLog)
        {
            if (string.IsNullOrWhiteSpace(Verbose) || Verbose.Contains(verb) || forceLog)
                return true;
            return false;
        }

        public static void Info(string msg, bool forceLog = false, bool breakLine = true)
        {
            if (CheckIfWrite("info", forceLog))
                Write(msg, ConsoleColor.DarkGray, breakLine);
        }

        public static void Error(string msg, bool forceLog = false, bool breakLine = true)
        {
            if (CheckIfWrite("error", forceLog))
                Write(msg, ConsoleColor.Red, breakLine);
        }

        public static void Success(string msg, bool forceLog = false, bool breakLine = true)
        {
            if (CheckIfWrite("success", forceLog))
                Write(msg, ConsoleColor.Blue, breakLine);
        }

        public static void Warning(string msg, bool forceLog = false, bool breakLine = true)
        {
            if (CheckIfWrite("warning", forceLog))
                Write(msg, ConsoleColor.Yellow, breakLine);
        }

        public static void Question(string msg, bool forceLog = false, bool breakLine = true)
        {
            Write(msg, ConsoleColor.Gray, breakLine);
        }

        public static void Write(string msg, ConsoleColor fontColor, bool breakLine = true)
        {
            if (Quiet)
                return;

            var color = Console.ForegroundColor;
            Console.ForegroundColor = fontColor;
            if (breakLine)
                Console.WriteLine(msg);
            else
                Console.Write(msg);

            Console.ForegroundColor = color;
        }

    }
}

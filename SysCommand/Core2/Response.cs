using Newtonsoft.Json;
using System;
namespace SysCommand
{
    public class Response
    {
        private static TypeNameSerializationBinder binder = new TypeNameSerializationBinder();
        public int Code { get; set; }

        private bool CheckIfWrite(VerboseEnum verb, bool forceWrite)
        {
            if (App.Current.Verbose == VerboseEnum.All || App.Current.Verbose.HasFlag(verb) || forceWrite)
                return true;
            return false;
        }

        public void Info(string msg, bool forceWrite = false, bool breakLine = true)
        {
            if (CheckIfWrite(VerboseEnum.Info, forceWrite))
                Write(msg, ConsoleColor.DarkGray, breakLine);
        }

        public void Critical(string msg, bool forceWrite = false, bool breakLine = true)
        {
            if (CheckIfWrite(VerboseEnum.Critical, forceWrite))
                Write(msg, ConsoleColor.DarkGray, breakLine);
        }

        public void Error(string msg, bool forceWrite = false, bool breakLine = true)
        {
            if (CheckIfWrite(VerboseEnum.Error, forceWrite))
                Write(msg, ConsoleColor.Red, breakLine);
        }

        public void Success(string msg, bool forceWrite = false, bool breakLine = true)
        {
            if (CheckIfWrite(VerboseEnum.Success, forceWrite))
                Write(msg, ConsoleColor.Blue, breakLine);
        }

        public void Warning(string msg, bool forceWrite = false, bool breakLine = true)
        {
            if (CheckIfWrite(VerboseEnum.Warning, forceWrite))
                Write(msg, ConsoleColor.Yellow, breakLine);
        }

        public void Question(string msg, bool forceWrite = false, bool breakLine = true)
        {
            Write(msg, ConsoleColor.Gray, breakLine);
        }

        public void Write(string msg, ConsoleColor fontColor, bool breakLine = true)
        {
            if (App.Current.Quiet)
                return;

            var color = Console.ForegroundColor;
            Console.ForegroundColor = fontColor;
            if (breakLine)
                Console.WriteLine(msg);
            else
                Console.Write(msg);

            Console.ForegroundColor = color;
        }

        public void Write(string value, params object[] args)
        {
            Console.Write(value, args);
        }

        public void WriteLine(string value, params object[] args)
        {
            Console.WriteLine(value, args);
        }

        public void Write(object obj)
        {
            string json = JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Binder = binder
            });

            Console.Write(json);
        }
    }
}

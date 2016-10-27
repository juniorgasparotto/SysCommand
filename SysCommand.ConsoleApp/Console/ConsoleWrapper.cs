using SysCommand.Parsing;
using SysCommand.Helpers;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace SysCommand.ConsoleApp
{
    public class ConsoleWrapper
    {
        private static TypeNameSerializationBinder binder = new TypeNameSerializationBinder();
        public bool BreakLineInNextWrite { get; set; }

        private int exitCode;
        private string input;

        public Verbose Verbose { get; set; }
        public ConsoleColor ColorInfo { get; set; }
        public ConsoleColor ColorCritical { get; set; }
        public ConsoleColor ColorError { get; set; }
        public ConsoleColor ColorSuccess { get; set; }
        public ConsoleColor ColorWarning { get; set; }
        public ConsoleColor ColorRead { get; set; }
        public TextWriter Out { get; set; }
        public TextReader In { get; set; }

        public bool ExitCodeHasValue { get; private set; }
        public bool Quiet { get; set; }

        public int ExitCode
        {
            get
            {
                return exitCode;
            }
            set
            {
                exitCode = value;
                this.ExitCodeHasValue = true;
            }
        }

        public ConsoleWrapper()
        {
            this.ColorInfo = ConsoleColor.DarkGray;
            this.ColorCritical = ConsoleColor.DarkGray;
            this.ColorError = ConsoleColor.Red;
            this.ColorSuccess = ConsoleColor.Blue;
            this.ColorWarning = ConsoleColor.Yellow;
            this.ColorRead = ConsoleColor.Cyan;
            this.In = Console.In;
            this.Out = Console.Out;
        }

        private bool CheckIfWrite(Verbose verb, bool forceWrite)
        {
            if (this.Verbose.HasFlag(Verbose.All) || this.Verbose.HasFlag(verb) || forceWrite)
                return true;
            return false;
        }

        public string Read()
        {
            // https://referencesource.microsoft.com/#mscorlib/system/io/textreader.cs,91b2c7adbdc4b165

            StringBuilder sb = new StringBuilder();
            while (true)
            {
                int ch = Console.In.Read();
                if (ch == -1) break;
                if (ch == '\r' || ch == '\n')
                {
                    if (ch == '\r' && Console.In.Peek() == '\n')
                        Console.In.Read();
                    
                    BreakLineInNextWrite = false;
                    break;
                }
                sb.Append((char)ch);
            }

            if (sb.Length > 0)
                return sb.ToString();
            return null;
        }

        public string Read(string label, string defaultValueIfQuiet = null, bool breakLine = false)
        {
            if (this.Quiet)
                return defaultValueIfQuiet;

            Write(label, breakLine, this.ColorRead);
            return Read();
        }

        public string Input()
        {
            if (Console.IsInputRedirected && Console.In != null)
                this.input = Console.In.ReadToEnd();

            return this.input;
        }
        
        public void Info(object msg, bool breakLine = false, bool forceWrite = false)
        {
            if (CheckIfWrite(Verbose.Info, forceWrite))
                Write(msg, breakLine, this.ColorInfo);
        }

        public void Critical(object msg, bool breakLine = false, bool forceWrite = false)
        {
            if (CheckIfWrite(Verbose.Critical, forceWrite))
                Write(msg, breakLine, this.ColorCritical);
        }

        public void Error(object msg, bool breakLine = false, bool forceWrite = false)
        {
            if (CheckIfWrite(Verbose.Error, forceWrite))
                Write(msg, breakLine, this.ColorError);
        }

        public void Success(object msg, bool breakLine = false, bool forceWrite = false)
        {
            if (CheckIfWrite(Verbose.Success, forceWrite))
                Write(msg, breakLine, this.ColorSuccess);
        }

        public void Warning(object msg, bool breakLine = false, bool forceWrite = false)
        {
            if (CheckIfWrite(Verbose.Warning, forceWrite))
                Write(msg, breakLine, this.ColorWarning);
        }

        public void Write(object msg, bool breakLine = false)
        {
            Write(msg, breakLine, Console.ForegroundColor);
        }

        public void Write(object obj, bool breakLine, ConsoleColor fontColor)
        {
            if (this.Quiet)
                return;

            var str = obj != null ? obj.ToString() : null;

            if (BreakLineInNextWrite)
                Out.WriteLine();

            BreakLineInNextWrite = !string.IsNullOrEmpty(str) && !str.LastOrDefault().In('\n', '\r');

            var color = Console.ForegroundColor;
            Console.ForegroundColor = fontColor;
            if (breakLine)
            { 
                Out.WriteLine(str);
                BreakLineInNextWrite = false;
            }
            else
            { 
                Out.Write(str);
            }
            Console.ForegroundColor = color;
        }

        internal object Read(object debugGetArguments)
        {
            throw new NotImplementedException();
        }

        //public string ToJson(object obj)
        //{
        //    string json = JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings
        //    {
        //        TypeNameHandling = TypeNameHandling.Auto,
        //        Binder = binder
        //    });

        //    return json;
        //}

        //public T Input<T>()
        //{
        //    var objFile = default(T);

        //    if (string.IsNullOrWhiteSpace(Input()))
        //    {
        //        objFile = JsonConvert.DeserializeObject<T>(Input(), new JsonSerializerSettings
        //        {
        //            TypeNameHandling = TypeNameHandling.Auto,
        //            Binder = binder
        //        });
        //    }

        //    return objFile;
        //}
    }
}

using SysCommand.Helpers;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace SysCommand.ConsoleApp
{
    public class ConsoleWrapper
    {
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
        //public bool Quiet { get; set; }

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
            this.Verbose = Verbose.Info;
        }

        public virtual bool CheckIfWrite(Verbose verb, bool forceWrite)
        {
            if (this.Verbose.HasFlag(Verbose.All) || this.Verbose.HasFlag(verb) || forceWrite)
                return true;
            return false;
        }

        public virtual string Read()
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

        public virtual string Read(string label, /*string defaultValueIfQuiet = null,*/ bool breakLine = false)
        {
            //if (this.Quiet)
            //    return defaultValueIfQuiet;

            WriteInternal(label, breakLine, this.ColorRead);
            return Read();
        }

        public virtual string Input()
        {
            if (Console.IsInputRedirected && Console.In != null)
                this.input = Console.In.ReadToEnd();

            return this.input;
        }
        
        public virtual void Write(object msg, bool breakLine = false, bool forceWrite = false)
        {
            if (CheckIfWrite(Verbose.Info, forceWrite))
                WriteInternal(msg, breakLine, this.ColorInfo);
        }

        public virtual void Critical(object msg, bool breakLine = false, bool forceWrite = false)
        {
            if (CheckIfWrite(Verbose.Critical, forceWrite))
                WriteInternal(msg, breakLine, this.ColorCritical);
        }

        public virtual void Error(object msg, bool breakLine = false, bool forceWrite = false)
        {
            if (CheckIfWrite(Verbose.Error, forceWrite))
                WriteInternal(msg, breakLine, this.ColorError);
        }

        public virtual void Success(object msg, bool breakLine = false, bool forceWrite = false)
        {
            if (CheckIfWrite(Verbose.Success, forceWrite))
                WriteInternal(msg, breakLine, this.ColorSuccess);
        }

        public void Warning(object msg, bool breakLine = false, bool forceWrite = false)
        {
            if (CheckIfWrite(Verbose.Warning, forceWrite))
                WriteInternal(msg, breakLine, this.ColorWarning);
        }

        protected virtual void WriteInternal(object msg, bool breakLine = false)
        {
            WriteInternal(msg, breakLine, Console.ForegroundColor);
        }

        protected virtual void WriteInternal(object obj, bool breakLine, ConsoleColor fontColor)
        {
            //if (this.Quiet)
            //    return;

            var str = obj != null ? obj.ToString() : null;

            if (BreakLineInNextWrite)
                Out.WriteLine();

            BreakLineInNextWrite = !string.IsNullOrEmpty(str) && !(str.Last() == '\n' || str.Last() == '\r');

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
    }
}

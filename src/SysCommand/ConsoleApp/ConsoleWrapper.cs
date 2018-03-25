using SysCommand.Helpers;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace SysCommand.ConsoleApp
{
    /// <summary>
    /// Small wrapper of System.Console
    /// </summary>
    public class ConsoleWrapper
    {
        /// <summary>
        /// Smart line break while using dós write and read methods. The variable App.Console.BreakLineInNextWrite controls the breaks and helps you not leave empty lines.
        /// </summary>
        public bool BreakLineInNextWrite { get; set; }

        private int exitCode;
        private string input;

        /// <summary>
        /// Current verbose level
        /// </summary>
        public Verbose Verbose { get; set; }

        /// <summary>
        /// Color for verbose info
        /// </summary>
        public ConsoleColor ColorInfo { get; set; }

        /// <summary>
        /// Color for verbose critical
        /// </summary>
        public ConsoleColor ColorCritical { get; set; }

        /// <summary>
        /// Color for verbose error
        /// </summary>
        public ConsoleColor ColorError { get; set; }

        /// <summary>
        /// Color for verbose success
        /// </summary>
        public ConsoleColor ColorSuccess { get; set; }

        /// <summary>
        /// Color for verbose warning
        /// </summary>
        public ConsoleColor ColorWarning { get; set; }

        /// <summary>
        /// Color for verbose read
        /// </summary>
        public ConsoleColor ColorRead { get; set; }

        /// <summary>
        /// Wrapper to Console.Out
        /// </summary>
        public TextWriter Out { get; set; }

        /// <summary>
        /// Wrapper to Console.In
        /// </summary>
        public TextReader In { get; set; }

        /// <summary>
        /// Check if exit code was filled
        /// </summary>
        public bool ExitCodeHasValue { get; private set; }

        /// <summary>
        /// Exit code value
        /// </summary>
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

        /// <summary>
        /// Initialize
        /// </summary>
        public ConsoleWrapper(TextWriter output = null)
        {
            this.ColorInfo = Console.ForegroundColor;
            this.ColorCritical = ConsoleColor.Red;
            this.ColorError = ConsoleColor.Red;
            this.ColorSuccess = ConsoleColor.Blue;
            this.ColorWarning = ConsoleColor.Yellow;
            this.ColorRead = Console.ForegroundColor;
            this.In = Console.In;
            this.Out = output ?? Console.Out;
            this.Verbose = Verbose.Info;
        }

        /// <summary>
        /// Check if can write the text
        /// </summary>
        /// <param name="verb">Verbose test</param>
        /// <param name="forceWrite">Flag to force write</param>
        /// <returns>Return if can write</returns>
        public virtual bool CheckIfWrite(Verbose verb, bool forceWrite)
        {
            if (this.Verbose.HasFlag(Verbose.All) || this.Verbose.HasFlag(verb) || forceWrite)
                return true;
            return false;
        }

        /// <summary>
        /// Read information from user
        /// </summary>
        /// <returns>The user input</returns>
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

        /// <summary>
        /// Read information from user with a label
        /// </summary>
        /// <param name="label">Label text</param>
        /// <param name="breakLine">Break line if true</param>
        /// <returns>The user input</returns>
        public virtual string Read(string label, bool breakLine = false)
        {
            WriteWithColor(label, breakLine, this.ColorRead);
            return Read();
        }

        /// <summary>
        /// Get the redirect input value
        /// </summary>
        /// <returns>Redirect input value</returns>
        public virtual string Input()
        {
            if (Console.IsInputRedirected && Console.In != null)
                this.input = Console.In.ReadToEnd();

            return this.input;
        }

        /// <summary>
        /// Write in console
        /// </summary>
        /// <param name="msg">Message</param>
        /// <param name="breakLine">Break line if true</param>
        /// <param name="forceWrite">Flag to force write</param>
        public virtual void Write(object msg, bool breakLine = false, bool forceWrite = false)
        {
            if (CheckIfWrite(Verbose.Info, forceWrite))
                WriteWithColor(msg, breakLine, this.ColorInfo);
        }

        /// <summary>
        /// Write in console as a critical
        /// </summary>
        /// <param name="msg">Message</param>
        /// <param name="breakLine">Break line if true</param>
        /// <param name="forceWrite">Flag to force write</param>
        public virtual void Critical(object msg, bool breakLine = false, bool forceWrite = false)
        {
            if (CheckIfWrite(Verbose.Critical, forceWrite))
                WriteWithColor(msg, breakLine, this.ColorCritical);
        }

        /// <summary>
        /// Write in console as a error
        /// </summary>
        /// <param name="msg">Message</param>
        /// <param name="breakLine">Break line if true</param>
        /// <param name="forceWrite">Flag to force write</param>
        public virtual void Error(object msg, bool breakLine = false, bool forceWrite = false)
        {
            if (CheckIfWrite(Verbose.Error, forceWrite))
                WriteWithColor(msg, breakLine, this.ColorError);
        }

        /// <summary>
        /// Write in console as a success
        /// </summary>
        /// <param name="msg">Message</param>
        /// <param name="breakLine">Break line if true</param>
        /// <param name="forceWrite">Flag to force write</param>
        public virtual void Success(object msg, bool breakLine = false, bool forceWrite = false)
        {
            if (CheckIfWrite(Verbose.Success, forceWrite))
                WriteWithColor(msg, breakLine, this.ColorSuccess);
        }

        /// <summary>
        /// Write in console as a warning
        /// </summary>
        /// <param name="msg">Message</param>
        /// <param name="breakLine">Break line if true</param>
        /// <param name="forceWrite">Flag to force write</param>
        public void Warning(object msg, bool breakLine = false, bool forceWrite = false)
        {
            if (CheckIfWrite(Verbose.Warning, forceWrite))
                WriteWithColor(msg, breakLine, this.ColorWarning);
        }

        public virtual void WriteWithColor(object obj, bool breakLine, ConsoleColor fontColor)
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

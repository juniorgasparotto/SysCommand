using System.Collections.Generic;
using System.Linq;
using System;
using System.Runtime.InteropServices;
using SysCommand.Helpers;
using SysCommand;

namespace SysCommand.ConsoleApp.Helpers
{
    /// <summary>
    /// Helper to simulate the prompt parser
    /// </summary>
    public static class ConsoleAppHelper
    {
        [DllImport("shell32.dll", SetLastError = true)]
        private static extern IntPtr CommandLineToArgvW([MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine, out int pNumArgs);

        /// <summary>
        /// Parse using win32 dll
        /// </summary>
        /// <param name="commandLine">Command line text</param>
        /// <returns>Arguments parseds</returns>
        public static string[] CommandLineToArgsFromShell32(string commandLine)
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

        /// <summary>
        /// Parse using a internal class that simule the a simple prompt parse
        /// </summary>
        /// <param name="commandLine">Command line text</param>
        /// <returns>Arguments parseds</returns>
        public static string[] CommandLineToArgs(string commandLine)
        {
            return ConsoleInputParser.Parse(commandLine);
        }

        /// <summary>
        /// Parse using a internal class that simule the a simple prompt parse
        /// </summary>
        /// <param name="args">Command line text</param>
        /// <returns>Arguments parseds</returns>
        public static string[] StringToArgs(string args)
        {
            if (!string.IsNullOrWhiteSpace(args))
                return ConsoleAppHelper.CommandLineToArgs(args);
            else
                return new string[0];
        }
    }
}

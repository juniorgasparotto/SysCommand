using System.Collections.Generic;
using System.Linq;
using System;
using System.Runtime.InteropServices;
using SysCommand.Helpers;
using SysCommand;

namespace SysCommand.ConsoleApp.Helpers
{
    public static class ConsoleAppHelper
    {
        [DllImport("shell32.dll", SetLastError = true)]
        private static extern IntPtr CommandLineToArgvW([MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine, out int pNumArgs);

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

        public static string[] CommandLineToArgs(string commandLine)
        {
            return ConsoleInputParser.Parse(commandLine);
        }

        public static string[] StringToArgs(string args)
        {
            if (!string.IsNullOrWhiteSpace(args))
                return ConsoleAppHelper.CommandLineToArgs(args);
            else
                return new string[0];
        }
    }
}

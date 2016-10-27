using System.Collections.Generic;
using System.Linq;
using System;
using System.Runtime.InteropServices;
using SysCommand.Helpers;

namespace SysCommand.ConsoleApp
{
    public static class ConsoleAppHelper
    {
        [DllImport("shell32.dll", SetLastError = true)]
        static extern IntPtr CommandLineToArgvW([MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine, out int pNumArgs);

        public static string[] CommandLineToArgs(string commandLine)
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

        public static string GetConsoleHelper(Dictionary<string, string> helps, int padding = 4, int chunkSize = 80)
        {
            var helpPrintList = new List<string[]>();
            var helpFormated = new List<Tuple<string, string>>();

            foreach (var help in helps)
            {
                var split = StringHelper.ChunksWords(help.Value, chunkSize).ToList();
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

            return StringHelper.PadElementsInLines(helpPrintList, padding);
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

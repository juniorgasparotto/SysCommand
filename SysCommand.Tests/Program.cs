using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysCommand.Tests
{
    public class Program : App333
    {
        public static void LoadAssembly()
        {

        }

        public Program()
        {

        }

        static int Main(string[] args)
        {
            //foreach (var v in args)
            //    Console.WriteLine(v);
            //return 0;

            while (true)
            {
                App333.Initialize<Program>();
                App333.Current.DebugShowExitConfirm = false;
                App333.Current.ActionCharPrefix = null;
                App333.Current.SetArgs("position --teste -xyz+ --bla -u false -i+ teste");
                App333.Current.Run();
                if (!App333.Current.InDebug)
                    return App333.Current.Response.Code;
            }
        }
    }
}

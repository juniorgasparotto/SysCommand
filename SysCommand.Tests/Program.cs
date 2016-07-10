using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysCommand.Tests
{
    public class Program : App
    {
        public static void LoadAssembly()
        {

        }

        public Program()
        {

        }

        static int Main(string[] args)
        {
            while (true)
            {
                App.Initialize<Program>();
                App.Current.DebugShowExitConfirm = false;
                App.Current.ActionCharPrefix = null;
                App.Current.SetArgs("position --teste -xyz+ --bla -u false -i+ teste");
                App.Current.Run();
                if (!App.Current.InDebug)
                    return App.Current.Response.Code;
            }
        }
    }
}

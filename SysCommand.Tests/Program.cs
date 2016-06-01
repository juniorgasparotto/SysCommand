using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysCommand.Tests
{
    class Program : App
    {
        static int Main(string[] args)
        {   
            while (true)
            {
                App.Initialize<Program>();
                App.Current.DebugShowExitConfirm = false;
                App.Current.ActionCharPrefix = '$';
                App.Current.Run();
                if (!App.Current.InDebug)
                    return App.Current.Response.Code;
            }
        }
    }
}

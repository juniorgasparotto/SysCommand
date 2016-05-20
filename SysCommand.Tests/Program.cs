using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysCommand.Tests
{
    class Program : App
    {
        static void Main(string[] args)
        {   
            while (true)
            {
                App.Initialize<Program>();
                App.Current.DebugShowExitConfirm = false;
                //App.Current.IgnoreCommmand<ClearCommand>();
                App.Current.Run();
                if (!App.Current.InDebug)
                    break;
            }
        }
    }
}

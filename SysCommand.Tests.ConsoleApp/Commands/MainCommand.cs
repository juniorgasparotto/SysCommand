using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysCommand.Tests.ConsoleApp.Commands
{
    public class MainCommand : Command
    {
        public string Main()
        {
            return "Main";
        }

        public string main()
        {
            return "main";
        }
    }
}

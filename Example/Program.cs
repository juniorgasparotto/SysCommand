using System;
using System.Collections.Generic;
using System.Linq;

namespace Example
{
    using SysCommand.ConsoleApp;

    public class Program
    {
        public static int Main()
        {
            return App.RunApplication();
        }
    }

    public class HelloWorld1 : Command
    {
       
        public string HelloWorld(string myArg0, int? myArg1 = null)
        {
            return string.Format("My HelloWorld1 (Arg0: {0}; Arg1: {1})", myArg0, myArg1);
        }

        public string HelloWorld(string myArg0, DateTime myArg1)
        {
            return string.Format("My HelloWorld1 (Arg0: {0}; Arg1: {1})", myArg0, myArg1);
        }

        public string HelloWorld2(IEnumerable<string> myArg0)
        {
            return string.Format("My HelloWorld1 (Arg0: {0})", myArg0.Count());
        }
    }
    
    public class HelloWorld2 : Command
    {
        public string MyArg0 { get; set; }
        public string MyArg1 { get; set; }

        public string Main()
        {
            return string.Format("My HelloWorld2 (Arg0: {0}; Arg1: {1})", MyArg0, MyArg1);
        }
    }

    public class HelloWorld3 : Command
    {
        public decimal Test()
        {
            var result = this.App.Console.Read("My question: ");

            if (result != "S")
            {
                // option1: use write method
                this.App.Console.Write(99.99m);
                // option2: or use return, its the same.
                return 99.99m;
            }

            return 0;
        }
    }
}

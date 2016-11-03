using SysCommand.ConsoleApp;
using System;

namespace SysCommand.Tests.ConsoleApp
{
    class Program
    {
        static int Main(string[] args)
        {
            return App.RunInfiniteIfDebug();
        }
    }
}
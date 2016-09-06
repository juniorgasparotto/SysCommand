using System.Linq;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SysCommand.UnitTests
{
    [TestClass]
    public class TestApp
    {
        [TestMethod]
        public void RunApp()
        {
            var app = new App2(Commands.LoadAllCommandsInAppDomain(App2.InDebug, null));
        }
    }

    internal class App2
    {
        private string[] args;

        public static bool InDebug { get { return System.Diagnostics.Debugger.IsAttached; } }

        public Commands MapContext { get; private set; }
        public bool WhenIsInDebugReadArgs { get; set; }

        public App2(Commands mapContext)
        {
            this.MapContext = mapContext;
            ArgumentRaw
        }

        public void Run()
        {

        }

        private string[] GetOrRead()
        {
            if (this.args != null)
                return this.args;

            if (InDebug && this.WhenIsInDebugReadArgs)
            {
                Console.WriteLine("Enter with args:");
                this.SetArgs(Console.ReadLine());
            }
            else
            {
                this.args = Environment.GetCommandLineArgs();
                var listArgs = this.args.ToList();
                // remove the app path that added auto by .net
                listArgs.RemoveAt(0);
                this.SetArgs(listArgs.ToArray());
            }

            return this.args;
        }

        private void SetArgs(string[] args)
        {
            this.args = args;
        }

        public void SetArgs(string args)
        {
            if (!string.IsNullOrWhiteSpace(args))
                this.args = AppHelpers.CommandLineToArgs(args);
            else
                this.args = new string[0];
        }
    }
}

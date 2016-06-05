using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using SysCommand.Tests;

namespace SysCommand.UnitTests
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void TestRun()
        {
            var output = new StringWriter();
            this.ConfigApp(output, "save");
            output.ToString();
        }

        [TestMethod]
        public void TestRequest()
        {
            var args = AppHelpers.CommandLineToArgs("action-name --p1 \"a\" --p2 \"b\"");
            var request = new Request(args);
            var requestAction = request.RequestActions.FirstOrDefault(f => f.Name == null);
            Assert.IsTrue(requestAction == null);            
            Assert.IsTrue(requestAction.Arguments[0] == "--p1");
            Assert.IsTrue(requestAction.Arguments[1] == "a");
            Assert.IsTrue(requestAction.Arguments[2] == "--p2");
            Assert.IsTrue(requestAction.Arguments[3] == "b");
            
        }

        private void ConfigApp(StringWriter output, string args)
        {
            // Only to load all actions in 'console application test project'.
            Program.LoadAssembly();

            Console.SetOut(output);
            App.Initialize();
            App.Current.DebugShowExitConfirm = false;
            App.Current.ActionCharPrefix = null;
            App.Current.SetArgs(args);
            App.Current.Run();
        }
    }
}

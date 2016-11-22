using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using System.Linq;
using SysCommand.ConsoleApp;
using System.IO;
using SysCommand.Test;

namespace SysCommand.Tests.UnitTests
{
    [TestClass]
    public class TestAppHistory
    {
        public TestAppHistory()
        {
            TestHelper.SetCultureInfoToInvariant();
        }

        [TestMethod]
        public void Test_01_AppHistory()
        {
            var app = new App(
                   commandsTypes: new AppDomainCommandLoader().GetFromAppDomain(false)
               );

            var argsInput = "main 1 1 1";
            var appResult = app.Run(argsInput);
            Assert.IsTrue(string.Join(" ", appResult.Args) == argsInput);
        }
    }
}

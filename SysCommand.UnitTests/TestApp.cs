using System.Linq;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SysCommand;
using SysCommand.ConsoleApp;

namespace SysCommand.UnitTests
{
    [TestClass]
    public class TestApp
    {
        [TestMethod]
        public void RunApp()
        {
            var commandLoader = new AppDomainCommandLoader();
            //var app = new App2(commandLoader.GetFromAppDomain());
        }
    }

    
}

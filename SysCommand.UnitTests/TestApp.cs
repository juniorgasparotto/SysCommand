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
            var commandLoader = new CommandAppDomainLoader();
            var app = new App2(commandLoader.GetFromAppDomain());
        }
    }

    
}

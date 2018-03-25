using SysCommand.ConsoleApp.Helpers;
using SysCommand.Tests.UnitTests.Common;
using System.IO;

namespace SysCommand.Tests.UnitTests.DotNetCore
{
    public sealed class Startup : IStartup
    {
        static Startup()
        {
            Directory.SetCurrentDirectory(Development.GetProjectDirectory());
        }

        public void Start()
        {
            TestHelper.FolderTests = null;
            //TestHelper.FolderTests = "../SysCommand.Tests.UnitTests.DotNetCore/.tests";
        }
    }
}

using SysCommand.Tests.UnitTests.Common;
using System.IO;

namespace SysCommand.Tests.UnitTests.DotNetFull
{
    public sealed class Startup : IStartup
    {
        static Startup()
        {
            Directory.SetCurrentDirectory("../../../SysCommand.Tests.UnitTests.DotNetCore");
        }

        public void Start()
        {
            TestHelper.FolderTests = null;
        }
    }
}

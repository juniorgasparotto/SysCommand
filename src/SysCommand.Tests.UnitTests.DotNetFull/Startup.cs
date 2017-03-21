using SysCommand.Tests.UnitTests.Common;

namespace SysCommand.Tests.UnitTests.DotNetFull
{
    public sealed class Startup : IStartup
    {
        public void Start()
        {
            TestHelper.FolderTests = "../SysCommand.Tests.UnitTests.DotNetCore/.tests";
        }
    }
}

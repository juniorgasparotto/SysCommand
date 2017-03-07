using SysCommand.ConsoleApp;
using Xunit;

namespace MyFirstUnitTests
{
    public class Class1
    {
        [Fact]
        public void PassingTest()
        {
            App.RunApplication();
        }
    }
}
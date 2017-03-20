using SysCommand.TestUtils;
/// <summary>
/// Used by the ModuleInit. All code inside the Initialize method is ran as soon as the assembly is loaded.
/// </summary>
public static class ModuleInitializer
{
    /// <summary>
    /// Initializes the module.
    /// </summary>
    public static void Initialize()
    {
        TestHelper.FolderTests = "../SysCommand.Tests.UnitTests.DotNetCore/.tests";
    }
}
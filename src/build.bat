dotnet msbuild "D:\Junior\Projetos\GITHUB.COM\juniorgasparotto\SysCommand\src\SysCommand.Tests.UnitTests\SysCommand.Tests.UnitTests.csproj"
dotnet test "D:\Junior\Projetos\GITHUB.COM\juniorgasparotto\SysCommand\src\SysCommand.Tests.UnitTests\SysCommand.Tests.UnitTests.csproj"

dotnet msbuild "D:\Junior\Projetos\GITHUB.COM\juniorgasparotto\SysCommand\src\SysCommand.Tests.UnitTests\SysCommand.Tests.UnitTests.csproj" /p:DefineConstants="NETSTANDARD1_6"

dotnet msbuild "D:\Junior\Projetos\GITHUB.COM\juniorgasparotto\SysCommand\src\SysCommand\SysCommand.csproj" /p:OutPutPath="D:/OutPut"


dotnet test "D:\Junior\Projetos\GITHUB.COM\juniorgasparotto\SysCommand\src\SysCommand.Tests.XUnit\SysCommand.Tests.XUnit.csproj"

dotnet test "D:\Junior\Projetos\GITHUB.COM\juniorgasparotto\SysCommand\src\SysCommand.Tests.XUnit\SysCommand.Tests.XUnit.csproj"
dotnet test "D:\Junior\Projetos\GITHUB.COM\juniorgasparotto\SysCommand\src\SysCommand.Tests.UnitTests.DotNetCore\SysCommand.Tests.UnitTests.DotNetCore.csproj"
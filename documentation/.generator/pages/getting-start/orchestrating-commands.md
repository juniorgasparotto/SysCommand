## Orquestrando comandos <header-set anchor-name="orchestrating-commands" />

Uma forma interessante de usar o SysCommand é fazendo o uso de diversos comandos em uma ação orquestradora. É importante lembrar que os comandos devem ser criados para funcionarem de forma independente, se isso não for possível, não torne-o um comando, crie uma classe que não herde de `Command` e utilize em sua ação.

O exemplo abaixo mostra um cenário onde seria interessante o uso de diversos comandos em uma ação. A ideia é criar uma aplicação que possa fazer a montagem de um `csproj` e também o ZIP de uma pasta qualquer. Porém, teremos uma ação `Publish` que fará a publicação da aplicação usando os dois comandos.

```csharp
using SysCommand.ConsoleApp;
using SysCommand.Mapping;
using System;
using System.Diagnostics;
using System.IO;

namespace Publisher
{
    public class OrchestratorCommand : Command
    {
        public void Publish(string csproj, string dirOutput)
        {
            var build = App.Commands.Get<MSBuildCommand>();
            var zip = App.Commands.Get<ZipCommand>();
            
            build.Build(csproj, dirOutput);
            pack.Zip(dirOutput);
        }
    }

    public class ZipCommand : Command
    {
        private void Zip(string dirToZip)
        {
            System.IO.Compression.ZipFile.CreateFromDirectory(dirToZip, $"{dirToZip}/package.zip"});
        }
    }

    public class MSBuildCommand : Command
    {
        public BuildCommand()
        {
            this.UsePrefixInAllMethods = true;
        }

        public void Clear()
        {
            // Clear
        }

        [Action(UsePrefix = false)]
        public void Build(string csproj, string dirOutput)
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    CreateNoWindow = false,
                    UseShellExecute = false,
                    FileName = "C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe",
                    WindowStyle = ProcessWindowStyle.Normal,
                    Arguments = string.Format("{0} /t:Build /m /property:Configuration={1} /p:OutDir={2}", csproj, "Debug", dirOutput)
                };

                using (Process exeProcess = Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                App.Console.Error(ex.Message);
            }
        }
    }
}
```
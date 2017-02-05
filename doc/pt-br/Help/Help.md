# Help automatico <header-set anchor-name="help" />

O `help` é gerado de forma automatica pelo sistema e para exibi-lo basta seguir os exemplos abaixo:

**Exibe o help completo:**

```MyApp.exe help```

**Exibe o help para uma ação especifica:**

```MyApp.exe help my-action```

O texto do `help` é gerado por um comando interno do sistema, mas é possível customizar esse texto, basta criar um novo `Command` que herda da interface `SysCommand.ConsoleApp.Commands.IHelpCommand` e o help padrão será ignorado.

```csharp
using SysCommand.ConsoleApp;
public class Program
{
    public static int Main()
    {
        return App.RunApplication();
    }

    public class CustomHelp : Command, SysCommand.ConsoleApp.Commands.IHelpCommand
    {
        public string MyCustomHelp(string action = null)
        {
            foreach(var map in this.App.Maps)
            {
                
            }
            return "Custom help";
        }
    }
}
```

```
MyApp.exe my-custom-help
Custom help

MyApp.exe help
Could not find any action.
```

Uma outra opção é criar um `Descriptor` que herda da interface `SysCommand.ConsoleApp.Descriptor.IDescriptor` e defini-lo na sua propriedade `App.Descriptor`. Isso é possível, pois o `help` padrão utiliza os métodos de help contidos dentro dessa instancia. Essa opção não é recomendada se você deseja apenas customizar o `help`.

Uma opção mais segura seria criar um `Descriptor` herdando da classe `SysCommand.ConsoleApp.Descriptor.DefaultDescriptor` e sobrescrer apenas os métodos de help.

```csharp
using SysCommand.ConsoleApp;
public class Program
{
    public static int Main()
    {
        return App.RunApplication(
            () => {
                var app = new App();
                app.Descriptor = new CustomDescriptor();
                // OR
                app.Descriptor = new CustomDescriptor2();
                return app;
            }
        );
    }

    public class CustomDescriptor : IDescriptor { ... }
    public class CustomDescriptor2 : DefaultDescriptor
    { 
        public override string GetHelpText(IEnumerable<CommandMap> commandMaps) { ... }
        public override string GetHelpText(IEnumerable<CommandMap> commandMaps, string actionName) { ... }
    }
}
```

* O comando de help é o único que não pode ser ignorado pela inicialização, caso ele não exista na lista de tipos, ele será adicionado internamente.


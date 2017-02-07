## Customizando <header-set anchor-name="help-default" />

A funcionalidade de `help` nada mais é que um comando interno `SysCommand.ConsoleApp.Commands.HelpCommand.cs` que define as duas `actions` de help que foram apresentadas no tópico anterior. Por definição, todo comando de help precisa herdar da interface `SysCommand.ConsoleApp.Commands.IHelpCommand`, assim o sistema entende que esse comando fará esse papel. Obrigatóriamente, sempre haverá um comando de help, caso o usuário não customize, o comando padrão `HelpCommand` será utilizado.

Abaixo, segue um exemplo de um help completamente customizado:

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

Uma outra opção é criar um `Descriptor` que herda da interface `SysCommand.ConsoleApp.Descriptor.IDescriptor` e defini-lo na sua propriedade `App.Descriptor`. Isso é possível, pois o help padrão utiliza os métodos de help contidos dentro dessa instancia. Essa opção não é recomendada se você deseja apenas customizar o `help`.

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

**Observações:**

* O comando de help é o único que não pode ser ignorado pela inicialização, caso ele não exista na lista de tipos, ele será adicionado internamente.
* Para mais informações sobre customizações do help em propriedades veja o tópido de <anchor-get name="properties-customizing-help" />.
* Para mais informações sobre customizações do help em ações veja o tópido de <anchor-get name="methods-customizing-help" />.

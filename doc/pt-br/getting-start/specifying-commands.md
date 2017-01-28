## Especificando os tipos de comandos !heading

Ao especificar cada `Command` que será utilizado, você perde o recurso de busca automatica, mas ganha a flexibidade de controlar quais `Commands` devem ou não fazer parte do seu sistema. Para isso você pode trabalhar de duas formas, a `inclusiva` ou a `exclusiva`. A forma inclusiva é basicamente a especificação de cada `Command` e a forma exclusiva é o oposto, primeiro se carrega tudo e depois elimina-se o que não deseja.

A classe `SysCommand.ConsoleApp.Loader.AppDomainCommandLoader` é a responsável por buscar os commands de forma automatica e você pode usa-la na forma exclusiva. Internamente o sistema faz uso dela caso o parametro `commandsTypes` esteja `null`.

**Exemplo de forma inclusiva:**

```csharp
public class Program
{
    public static void Main(string[] args)
    {
        var commandsTypes = new[]
        {
            typeof(FirstCommand)
        };
        
        // Specify what you want.
        new App(commandsTypes).Run(args);

        // Search for any class that extends from Command.
        /*
        new App().Run(args);
        */
    }

    public class FirstCommand : Command
    {
        public string FirstProperty
        {
            set
            {
                App.Console.Write("FirstProperty");
            }
        }
    }

    public class SecondCommand : Command
    {
        public string SecondProperty
        {
            set
            {
                App.Console.Write("SecondProperty");
            }
        }
    }
}
```

``` 
MyApp.exe help
usage:    [--first-property=<phrase>] <actions[args]>

FirstCommand

   --first-property    Is optional.

Displays help information

   help
      --action         Is optional.

Use 'help --action=<name>' to view the details of
any action. Every action with the symbol "*" can
have his name omitted.
```

Perceba que no help não existe nenhuma ocorrencia da class `SecondCommand`.

Perceba também que existe um help para o próprio mecanismo de help, esse `Command` sempre deverá existir, caso não seja especificado na sua lista de tipos o proprio sistema se encarregará de cria-lo utilizando o help padrão `SysCommand.ConsoleApp.Commands.HelpCommand`. Para mais informações sobre customização de help consulte `help automatico`.

**Exemplo de forma exclusiva:**

```csharp
public class Program
{
    public static void Main(string[] args)
    {
        // Create loader instance
        var loader = new AppDomainCommandLoader();

        // Remove unwanted command
        loader.IgnoreCommand<FirstCommand>();
        loader.IgnoreCommand<VerboseCommand>();
        loader.IgnoreCommand<ArgsHistoryCommand>();

        // Get all commands with 'ignored' filter
        var commandsTypes = loader.GetFromAppDomain();

        new App(commandsTypes).Run(args);
    }

    public class FirstCommand : Command
    {
        public string FirstProperty
        {
            set
            {
                App.Console.Write("FirstProperty");
            }
        }
    }

    public class SecondCommand : Command
    {
        public string SecondProperty
        {
            set
            {
                App.Console.Write("SecondProperty");
            }
        }
    }
}
```

```
MyApp.exe help
usage:    [--second-property=<phrase>] <actions[args]>

SecondCommand

   --second-property    Is optional.

Displays help information

   help
      --action          Is optional.

Use 'help --action=<name>' to view the details of
any action. Every action with the symbol "*" can
have his name omitted.
```

Perceba que no help não existe nenhuma ocorrencia da class `FirstCommand`.

Por enquanto, não se atente agora para as classes `VerboseCommand` e `ArgsHistoryCommand` elas são commands internos e serão explicados mais adiante na documentação.
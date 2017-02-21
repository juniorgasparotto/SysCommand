# Extras - OptionSet <header-set anchor-name="extras" />

Esse extra foi criado para uma ocasição especifica de parse onde o foco é ser simples. Com a classe `SysCommand.Extras.OptionSet` é possível fazer o parse de argumentos da forma tradicional.

_Métodos:_

* `void Add<T>(string longName, string helpText, Action<T> action)`: Adiciona uma configuração no formato `longo`
* `void Add<T>(char shortName, string helpText, Action<T> action)`: Adiciona uma configuração no formato `curto`
* `Add<T>(string longName, char? shortName, string helpText, Action<T> action)`: Adiciona uma configuração no formato `longo` e `curto`.
* `Add<T>(Argument<T> argument)`: Adiciona uma configuração completa
* `void Parse(string[] args, bool enablePositionalArgs = false)`: Executa o parse

_Propriedades:_

* `ArgumentsValid`: Depois do parse essa informação contém todos os argumentos válidos
* `ArgumentsInvalid`: Depois do parse essa informação contém todos os argumentos inválidos
* `HasError`: Indica se existe erros no parse

**Exemplo:**

```csharp
using SysCommand.ConsoleApp;
using SysCommand.ConsoleApp.Helpers;
using SysCommand.Extras;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Example.Extras
{
    public class Program
    {
        public static void Main(string[] args)
        {
            while (true)
            {
                Console.Write(Strings.CmdIndicator);
                args = ConsoleAppHelper.StringToArgs(Console.ReadLine());

                bool verbosity = false;
                var shouldShowHelp = false;
                var names = new List<string>();

                var options = new OptionSet();

                options.Add(new OptionSet.Argument<List<string>>("name", "the name of someone to greet.")
                {
                    Action = (n) =>
                    {
                        if (n != null)
                            names.AddRange(n);
                    }
                });

                options.Add(new OptionSet.Argument<bool>('v', "show verbose")
                {
                    Action = (v) =>
                    {
                        verbosity = v;
                    }
                });

                options.Add(new OptionSet.Argument<bool>("help", "show help")
                {
                    Action = (h) =>
                    {
                        shouldShowHelp = h;
                    }
                });

                options.Parse(args);

                if (!options.ArgumentsInvalid.Any())
                {
                    Console.WriteLine("verbosity: " + verbosity);
                    Console.WriteLine("shouldShowHelp: " + shouldShowHelp);
                    Console.WriteLine("names.Count: " + names.Count);
                }
                else
                {
                    Console.WriteLine("error");
                }
            }
        }
    }
}
```

```
cmd> --name a b c -v --help
verbosity: True
shouldShowHelp: True
names.Count: 3
```


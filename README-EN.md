# Table of Contents

* [Iniciando com o SysCommand](#iniciando-com-o-syscommand)


# Iniciando com o SysCommand

Esse tópico tem por objetivo resumir os principais recursos da ferramenta, não entraremos em detalhes agora, para isso veja a documentação completa. Vamos lá...

Para iniciar com o SysCommand você precisa de muitos poucos passos:

**Passos obrigatórios:**

* Criar seu projeto do tipo Console Application
* Instalar, via NuGet, o SysCommand em sua aplicação. Essa instalação consta com duas DLLs e duas dependencias que fazem todo o trabalho.
* Criar uma classe, em qualquer lugar, que herde de `SysCommand.ConsoleApp.Command`.
* Implemente sua classe levando em consideração que suas propriedades `publicas` serão convertidas em `arguments` e seus métodos `publicos` em `actions`.
* No seu método `Program.Main(string[] args)`, configure uma instancia da classe `SysCommand.ConsoleApp.App` que será o contexto da execução. Ou simplementes utilize o método estático `App.RunApplication()` que além de ser mais objetivo ainda dispõe do recurso de `simulação de console`.
* Execute o contexto usando o método `myApp.Run(args)` caso NÃO esteja usando o método estatico `App.RunApplication()`.

**Passos opcionais:**

* Se desejar, customize seus `arguments` ou `actions` usando os atributos `ArgumentAttribute` e `ActionAttribute`. Você pode customizar diversos atributos como nomes, help text, obrigatóriedade e dentro outros.
* Opte por usar o método `int Program.Main(string[] args)` com retorno, assim você pode retornar o status code para o console.
* Você pode utilizar o retorno dos métodos como `output`.
* Crie um método chamado `Main()` (sem parametros) dentro da sua classe para poder trabalhar com propriedades. Utilize tipos `Nullable` para ter condições de identificar que o usuário fez o input de um determinado argumento que corresponda a uma propriedade. O nome "Main" foi convensionado para esse tipo de uso, mas apenas quando esse método não tiver parametros.

**Exemplo:**

```csharp
namespace Example.Initialization.GettingStart
{
    using SysCommand.ConsoleApp;
    using SysCommand.Mapping;
    using System;

    public class Program
    {
        public static int Main(string[] args)
        {
            var myApp = new App();
            myApp.Run(args);
            return myApp.Console.ExitCode;

            // OR use static run, it's more simple and you can debug yours inputs directly in visual studio
            // return App.RunApplication();
        }
    }

    public class MyCommand : Command
    {
        public string MyPropertyString { get; set; }

        [Argument(LongName="property", ShortName='p', Help="My property help")]
        public int? MyPropertyInt { get; set; }

        public void Main()
        {
            if (MyPropertyString != null)
            {
                // using in your prompt to use this argument: 
                // MyApp.exe --my-property-string value
                Console.WriteLine("MyPropertyString");
            }

            if (MyPropertyInt != null)
            {
                // using in your prompt to use this argument: 
                // MyApp.exe --property 123
                // MyApp.exe -p 123
                Console.WriteLine("MyPropertyInt");
            }
        }

        public string MyAction(string myParameter)
        {
            // using in your prompt to use this action:
            // MyApp.exe my-action --my-parameter value
            return "MyAction";
        }

        [Action(Name="custom-action", Help = "Action help")]
        public string MyCustomAction
        (
            [Argument(ShortName = 'a')]
            decimal myParameter
        )
        {
            // using in your prompt to use this action:
            // MyApp.exe custom-action -a 9999.99
            return "MyCustomAction";
        }
    }
}
```

```
MyApp.exe --my-property-string value
MyPropertyString

MyApp.exe --property 123
MyPropertyInt

MyApp.exe -p 123
MyPropertyInt

MyApp.exe my-action --my-parameter value
MyAction

MyApp.exe custom-action -a 9999.99
MyCustomAction
```
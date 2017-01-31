## Como funciona? !heading

Ele funciona como um analisar de linhas de comando automático onde todas as tarefas de parse ficam por conta do framework, deixando o programador focado nas regras de negócios de sua aplicação. 

Tecnicamente, existem quatro entidades que são a base do framework:

* `App`: É o contexto da aplicação, onde uma `App` contém diversos `Commands`. É representada pela classe `SysCommand.ConsoleApp.App` e ela deve ser instanciada no método `Main` dando inicio a todo o processo. 

    Outro modo de iniciar sua aplicação é utilizando o método estático `App.RunApplication()` que além de ser mais objetivo ainda dispõe do recurso de `simulação de console`. Veja [Classe App](#classe-app) e [Inicializando por método estático com simulador de console](#inicializando-por-método-estático-com-simulador-de-console).
* `Command`: Os comandos representam um agrupamento de funcionalidades do mesmo contexto de negócio, similar aos `Controllers do MVC`. Programaticamente representa uma classe que herda de `SysCommand.ConsoleApp.Command`. Pode haver quantos comandos for necessário. Veja [Tipos de comandos](#tipos-de-comandos) e [Especificando os tipos de comandos](#especificando-os-tipos-de-comandos).
* `Argument`: Os argumentos representam o meio mais básico de uma aplicação console, são os conhecidos `--argument-name valor`, `-a` e etc. Programaticamente eles são representados pelas propriedades do `Command`. Do lado do usuário, nenhuma sintaxe especial foi criada, todo o padrão já conhecido foi respeitado, ou seja, os argumentos longos são acessados com o prefixo `--` ou pelo caracter `/` acompanhado do nome do argumento e os curtos com apenas um traço `-` acompanhado de apenas um caracter. Inputs posicionais também são suportados sendo possível omitir nomes de argumentos. Veja [Trabalhando com propriedades](#trabalhando-com-propriedades), [Tipos de inputs](#tipos-de-inputs) e [Tipos suportados](#tipos-suportados).
* `Action`: Representam ações iguais as `Actions dos Controllers do MVC`. Programaticamente representam os métodos do `Command` e seus parâmetros (se existir) serão convertidos em `arguments` que só serão acessados acompanhado do nome da `actions`, com exceção dos `Métodos padrão`. Seu uso é similar ao modo como usamos os recursos do `git` como: `git add -A`; `git commit -m "comments"`, onde `add` e `commit` seriam o nome das `actions` e `-A` e `-m` seus respectivos `arguments`. Veja [Trabalhando com métodos](#trabalhando-com-métodos) e [Métodos padrão](#métodos-padrão).

**Exemplo:**

```csharp
public class Program
{
    public static int Main(string[] args)
    {
        return App.RunApplication();
        
        // OR without "simulate console"
        // var myApp = new App();
        // myApp.Run(args);
        // return myApp.Console.ExitCode;
    }
}

public class GitCommand : Command
{
    public void Add(bool all)
    { 
        App.Console.Write("Add"); 
    }

    public void Commit(string message)
    { 
        App.Console.Write("Commit"); 
    }
}

public class MyCommand : Command
{
    // "Argument without customization"
    // usage:
    // MyApp.exe --my-property value
    public string MyProperty { get; set; }

    // "Argument customized"
    // usage:
    // MyApp.exe --custom-property 123
    // MyApp.exe -p 123
    [Argument(LongName="custom-property", ShortName='p', Help="My custom argument ")]
    public decimal? MyPropertyDecimal { get; set; }

    // Method to process arguments/properties, if any exist.
    // This signature "Main()" is reserved for this use only.
    public decimal Main()
    {
        
        if (MyProperty != null)
            App.Console.Write("MyProperty");

        if (MyPropertyDecimal != null)
            App.Console.Write("MyPropertyDecimal");

        return MyPropertyDecimal ?? 99.0;
    }

    // "Action without customization"
    // usage:
    // MyApp.exe my-action -p value
    public string MyAction(string p)
    {
        // Example showing that properties are executed before methods
        if (MyPropertyDecimal != null)
            App.Console.Write("Use property here if you want!");
        
        return "MyAction";
    }

    // "Action customized"
    // usage:
    // MyApp.exe custom-action
    // MyApp.exe custom-action -o
    [Action(Name="custom-action", Help = "My custom action")]
    public string MyAction
    (
        [Argument(ShortName = 'o')]
        bool? optionalParameter = null
    )
    {
        if (optionalParameter != null)
            App.Console.Error("optionalParameter");
        
        return "MyCustomAction";
    }
}
```

output

```
cmd> MyApp.exe --property 123
MyProperty

cmd> MyApp.exe --property2=123.99
MyProperty2

cmd> MyApp.exe my-action --my-parameter value
MyAction

cmd> MyApp.exe my-action --my-parameter value /property:123 --property2 0.1
MyAction
Use property here if you want!
MyProperty
MyProperty2
```

**Observações do exemplo acima:**

* Crie um método chamado `Main()` (sem parametros) dentro da sua classe para poder trabalhar com propriedades. Utilize tipos `Nullable` para ter condições de identificar que o usuário fez o input de um determinado argumento que corresponda a uma propriedade. O nome "Main" foi convensionado para esse tipo de uso, mas apenas quando esse método não tiver parametros. Veja [Trabalhando com propriedades](#trabalhando-com-propriedades).
* Todos os tipos primitivos do .NET, Enums, Enums Flags e Collections são suportados. Veja o tópico de [Tipos suportados](#tipos-suportados).
* Use `App.Console.Write()`, `App.Console.Error()` (entre outros) para imprimir seus outputs e usufruir de recursos como o `verbose`. Veja [Verbose](#verbose).
* Você pode utilizar o retorno dos métodos como `output`, inclusive o método reservado `Main()`. Ou use `void` se não quiser usar esse recurso. Veja [Output](#output).
* Se desejar, customize seus `arguments` ou `actions` usando os atributos `ArgumentAttribute` e `ActionAttribute`. Você pode customizar diversos atributos como nomes, texto de ajuda, obrigatóriedade e dentro outros. Veja [Customizando os nomes dos argumentos](#customizando-os-nomes-dos-argumentos) e [Customizando nomes de actions e arguments](#customizando-nomes-de-actions-e-arguments).
* Você pode usar métodos com o mesmo nome (sobrecargas) para definir diferentes `actions`. Elas podem ser chamadas no prompt de comando com o mesmo nome, mas os argumentos definirão qual o método a ser chamado, igual ocorre em c#. Veja [Sobrecargas](#sobrecargas)
* Opte por usar o método `int Program.Main(string[] args)` com retorno, assim você pode retornar o status code para o console. (ERROR=1 ou SUCCESS=0).
* Existe também o suporte nativo para gerar o texto de ajuda. Veja [Help automático](#help-automatico).

Esse inicio aborta apenas o básico do comportamento padrão, mas você pode customização do jeito que achar necessário. Para conhecer mais sobre esse projeto veja a nossa [Documentação completa](#documentação).
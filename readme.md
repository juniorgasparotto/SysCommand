# SysCommand

O `SysCommand` é um poderoso framework para o desenvolvimento de aplicações `Console Aplication` usando .NET. É simples, fortemente tipado e com grandes influências do padrão MVC. 

## Como funciona?

Ele funciona como um analisar de linhas de comando automático onde todas as tarefas de parse ficam por conta do framework, deixando o programador focado nas regras de negócios de sua aplicação. 

Tecnicamente, existem quatro entidades que são a base do framework:

* `App`: É o contexto da aplicação, onde uma `App` contém diversos `Commands`. Em C# representa a classe `SysCommand.ConsoleApp.App` e ela deve ser iniciada no método `Main`. Veja [Classe App](#classe-app).
* `Command`: Os comandos representam um agrupamento de funcionalidades do mesmo contexto de negócio, similar aos `Controllers` do MVC. Em c# representa uma classe que herda de `SysCommand.ConsoleApp.Command`. Pode haver quantos comandos for necessário. Veja [Tipos de comandos](#tipos-de-comandos) e [Especificando os tipos de comandos](#especificando-os-tipos-de-comandos).
* `Argument`: Os argumentos representam o meio mais básico de uma aplicação console, são os conhecidos `--argument-name valor`, `-a` e etc. Em c# eles são representados pelas propriedades do comando. Do lado do usuário, nenhuma sintaxe especial foi criada, todo o padrão já conhecido para uso de aplicativos de console foi respeitado, ou seja, os argumentos longos são acessados com o prefixo `--` ou pelo caracter `/` acompanhado do nome do argumento e os curtos com apenas um traço `-` acompanhado de apenas um caracter. Outra forma comum em diversos aplicativos é a possibilidade da omissão do nome do argumento usando `inputs posicionais` para inputar os valores. Veja [Trabalhando com propriedades](#trabalhando-com-propriedades), [Tipos de inputs](#tipos-de-inputs) e [Tipos suportados](#tipos-suportados).
* `Action`: As ações representam uma determinada ação dentro do seu negócio, igual as `Actions dos Controllers do MVC`. Em c# representam os métodos do comando e seus parâmetros serão convertidos em `arguments` que só serão acessados acompanhado do nome da `actions`, com exceção dos `Métodos padrão`. Seu uso é similar ao modo como usamos os recursos do `git` como: `git add -A`; `git commit -m "comments"`. Veja [Trabalhando com métodos](#trabalhando-com-métodos) e [Métodos padrão](#métodos-padrão).

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

* No seu método `Program.Main(string[] args)`, configure uma instância da classe `SysCommand.ConsoleApp.App` que será o contexto da execução. Ou simplementes utilize o método estático `App.RunApplication()` que além de ser mais objetivo ainda dispõe do recurso de `simulação de console`. Veja XXXXXXXXX
* Crie um método chamado `Main()` (sem parametros) dentro da sua classe para poder trabalhar com propriedades. Utilize tipos `Nullable` para ter condições de identificar que o usuário fez o input de um determinado argumento que corresponda a uma propriedade. O nome "Main" foi convensionado para esse tipo de uso, mas apenas quando esse método não tiver parametros. Veja [Trabalhando com propriedades](#trabalhando-com-propriedades).
* Todos os tipos primitivos do .NET, Enums e Collections são suportados. Veja o tópico de [Tipos suportados](#tipos-suportados).
* Use `App.Console.Write()`, `App.Console.Error()` entre outros para imprimir seus outputs e usufruir de recursos como o `verbose`, mas isso NÃO é obrigatório. Veja [Verbose](#verbose).
* Você pode utilizar o retorno dos métodos como `output`, inclusive o método reservado `Main()`. Ou use `void` se não quiser usar esse recurso. Veja [Output](#output).
* Se desejar, customize seus `arguments` ou `actions` usando os atributos `ArgumentAttribute` e `ActionAttribute`. Você pode customizar diversos atributos como nomes, help text, obrigatóriedade e dentro outros. Veja [Customizando os nomes dos argumentos](#customizando-os-nomes-dos-argumentos) e [Customizando nomes de actions e arguments](#customizando-nomes-de-actions-e-arguments).
* Você pode usar métodos com o mesmo nome (sobrecargas) para definir diferentes `actions`. Elas podem ser chamadas no prompt de comando com o mesmo nome, mas os argumentos definirão qual o método a ser chamado, igual em c#. Veja [Sobrecargas](#sobrecargas)
* Opte por usar o método `int Program.Main(string[] args)` com retorno, assim você pode retornar o status code para o console. (ERROR=1 ou SUCCESS=0).
* A sintaxe dos inputs se baseia no modelo: `[action-name ][-|--|/][argument-name][=|:| ][value]`. O caracter "|" significa "ou".
  * A forma curta é representada apenas por apenas um traço `-`
  * A forma longa pode ser representada por dois traços `--` ou pelo caracter `/`
  * O valor do argumento deve vir depois de um espaço ` ` ou unido com o nome do argumento por um `=` ou `:`
  * Todos os tipos primitivos do .NET, Enums e Collections são suportados.
  * Veja o tópico de [Tipos suportados](#tipos-suportados)
* Existe também o suporte nativo para gerar o texto de ajuda. Veja [Help automático](#help-automatico).


Esse inicio aborta apenas o básico do comportamento padrão, mas você pode customização do jeito que achar necessário. Para conhecer mais sobre esse projeto veja a nossa [Documentação completa](#documentação).

## Qual o objetivo deste projeto?

O objetivo é ajudar programadores de qualquer linguagem de programação que sofrem na hora de criar uma aplicação console. Muitas vezes desistimos de criar algo pela burocracia do parse e pela dificuldade de manutenção ao ver códigos onde sua lógica de parse está unida com sua lógica de negócios. Se você é como eu que adora criar mini-aplicações para resolver problemas do dia a dia usando consoles, então junte-se a nós!

Se você nunca trabalhou com .NET, talvez essa seja uma excelente oportunidade de conhece-lo. Com o novo .NET (Core Clr) você pode criar softwares em qualquer sistema operacional e somado aos beneficios do `SysCommand` você pode criar sua coleção de aplicativos de console da forma mais fácil possível.

# Instalação

* NuGet:
* NuGet Core CRL: 

## DLLs do pacote

  * `SysCommand.dll`: Contém toda a lógica de parse e execução de linhas de comandos. Pode ser utilizado em outros tipos de projetos como `Web Application` ou `Windows Forms`.
  * `SysCommand.ConsoleApp.dll`: Contém diversos recursos que uma aplicação do tipo `Console Application` necessita. Tudo foi pensado para que o padrão MVC fosse o mais natural possível.
  * Dependencias `NewtonSoft.Json` e `System.Web.Razor`: São dependencias necessárias para ajudar em alguns recursos que serão explicados mais adiante na documentação.

# Documentação

* [Começando](#começando)
  * [Classe App](#classe-app)
  * [Inicializando por método estático](#inicializando-por-método-estático)
  * [Especificando os tipos de comandos](#especificando-os-tipos-de-comandos)
  * [Utilizando o recurso de MultiAction](#utilizando-o-recurso-de-multiaction)
  * [Controle de eventos](#controle-de-eventos)
* [Tipos suportados](#tipos-suportados)
* [Help automatico](#help-automatico)
  * [Comportamento padrão](#comportamento-padrão)
* [Tipos de comandos](#tipos-de-comandos)
* [Verbose](#verbose)
* [Output](#output)
* [Output usando template Razor](#output-usando-template-razor)
* [Output usando template T4](#output-usando-template-t4)
* [Output tabelado](#output-tabelado)
* [Gerenciamento de históricos de argumentos](#gerenciamento-de-históricos-de-argumentos)
* [Redirecionamento de comandos](#redirecionamento-de-comandos)
* [Cancelamento da continuidade da execução](#cancelamento-da-continuidade-da-execução)
* [Tratamento de erros](#tratamento-de-erros)
* [Trabalhando com propriedades](#trabalhando-com-propriedades)
  * [Customizando os nomes dos argumentos](#customizando-os-nomes-dos-argumentos)
  * [Customizando as informações de help](#customizando-as-informações-de-help)
  * [Propriedades obrigatórias](#propriedades-obrigatórias)
  * [Habilitando o input posicional](#habilitando-o-input-posicional)
  * [Ignorar propriedades publicas por uma escolha manual usando atributo](#ignorar-propriedades-publicas-por-uma-escolha-manual-usando-atributo)
* [Trabalhando com métodos](#trabalhando-com-métodos)
  * [Métodos sem parametros](#métodos-sem-parametros)
  * [Parametros optionais](#parametros-optionais)
  * [Sobrecargas](#sobrecargas)
  * [Usando inputs posicionais](#usando-inputs-posicionais)
  * [Ignorar métodos publicos por uma escolha manual usando atributo](#ignorar-métodos-publicos-por-uma-escolha-manual-usando-atributo)
  * [Customizando nomes de actions e arguments](#customizando-nomes-de-actions-e-arguments)
  * [Customizando as informações de help de actions e seus parametros](#customizando-as-informações-de-help-de-actions-e-seus-parametros)
  * [Trocando a posição de parametros posicionais](#trocando-a-posição-de-parametros-posicionais)
  * [Propriedades do atributos ArgumentAttribute que não são utilizados](#propriedades-do-atributos-argumentattribute-que-não-são-utilizados)
  * [Métodos padrão](#métodos-padrão)
* [Tipos de inputs](#tipos-de-inputs)
* [Licença](#licença)


# Começando

Esse tópico tem por objetivo resumir os principais recursos da ferramenta, não entraremos em detalhes agora, para isso veja a documentação completa. Vamos lá...

Para iniciar com o SysCommand você precisa de muitos poucos passos:

**Passos obrigatórios:**

* Instalar o Visual Studio em sua máquina (Windows)
* Criar seu projeto do tipo `Console Application`
* Instalar o `SysCommand` em sua aplicação.
* Criar uma classe, em qualquer lugar, que herde de `SysCommand.ConsoleApp.Command`.
* Implemente sua classe levando em consideração que suas propriedades `publicas` serão convertidas em `arguments` e seus métodos `publicos` em `actions`.
* No seu método `Program.Main(string[] args)`, configure uma instância da classe `SysCommand.ConsoleApp.App` que será o contexto da execução. Ou simplementes utilize o método estático `App.RunApplication()` que além de ser mais objetivo ainda dispõe do recurso de `simulação de console`.
* Execute o contexto usando o método `myApp.Run(args)` caso NÃO esteja usando o método estatico `App.RunApplication()`.

**Passos opcionais:**

* Se desejar, customize seus `arguments` ou `actions` usando os atributos `ArgumentAttribute` e `ActionAttribute`. Você pode customizar diversos atributos como nomes, help text, obrigatóriedade e dentro outros.
* Opte por usar o método `int Program.Main(string[] args)` com retorno, assim você pode retornar o status code para o console. (ERROR=1 ou SUCCESS=0).
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

## Classe App

A classe `App` é a principal classe do sistema, ela é responsável por manter um contexto isolado por cada instancia `App`. Nenhum recurso estático é usado aqui, isso é importante para você ter a liberdade de criar quantas instancias quiser em qualquer escopo.

A inicialização do contexto da aplicação pode ser feita de duas formas, por uma instancia da class `App` com suas possíveis customizações ou atravez do método estático `App.RunApplication` que disponibiliza um recurso muito interressante de `simulação de console` ajudando você a testar seus inputs dentro do próprio Visual Studio, sem a necessidade de executar seu ".exe" em um console externo ou via "Command Line Arguments".

Em seu construtor estão as configurações mais importantes, são eles:

```csharp
public App(
           IEnumerable<Type> commandsTypes = null,
           bool enableMultiAction = true,
           bool addDefaultAppHandler = true
       )
```

* `commandsTypes`: Especifica os tipos dos `Command` que serão utilidados em todo o processo. Caso seja `null` então o sistema buscará automaticamente qualquer classe que extenda de `Command`. Entenda melhor em `Especificando os tipos de comandos`.
* `enableMultiAction`: Liga ou desliga o comportamento de `MultiAction`. Por padrão, esse comportamento estará ligado. Entenda melhor em `Utilizando o recurso de MultiAction`
* `addDefaultAppHandler`: Caso seja `false` então NÃO cria o handler de eventos que é responsável pelo mecanismo padrão de `outputs` e controles de `erros` e dentre outros. O padrão é `true`. Entenda melhor em `Controle de eventos`


## Inicializando por método estático

O método estático `App.RunApplication` disponibiliza um recurso muito interressante de `simulação de console` que ajuda você a testar seus inputs dentro do próprio Visual Studio sem a necessidade de executar seu ".exe" em um console externo ou via "Command Line Arguments". É importante ressaltar que esse simulador só será exibido dentro do Visual Studio, quando executar seu aplicativo final em outro console o comportamento será o mesmo do modo por instancia.

A chamada é bastante simples, basta uma linha para que tudo funcione usando as regras padrão. Caso você queira customizar a sua instancia de `App` então utilize o construtor `App.RunApplication(Func<App> appFactory)`.

```csharp
 public class Program
{
    public static void Main(string[] args)
    {
        // Default
        App.RunApplication();

        // Or use custom App
        
        /*
        App.RunApplication(() =>
        {
            var app = new App(enableMultiAction: false);
            return app;
        });
        */
    }

    public class MyCommand : Command
    {
        public string MyProperty
        {
            set
            {
                App.Console.Write(value);
            }
        }
    }
}
```

Ao iniciar esse código no Visual Studio um prompt padrão com um label "Enter with args:" será exibido. Isso indica que você pode iniciar seus testes quantas vezes for necessário. Para sair você pode usar o atalho padrão "CTRL+C" ou apertar o botão "stop" do Visual Studio.


```
Enter with args: --my-property value
value
Enter with args: --my-property otherValue
otherValue
```

## Especificando os tipos de comandos

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

## Utilizando o recurso de MultiAction

Esse recurso permite que você consiga disparar mais de uma `action` em um mesmo input. Por padrão ele vem habilitado e caso você ache desnecessário para o seu contexto então é só desliga-lo. É importante ressaltar que o recurso `Gerenciamento de históricos de argumentos` deixará de funcionar caso isso ocorra.

Outro ponto importante é a necessidade de "escapar" seu input caso o valor que você deseje inserir conflite com um nome de uma `action`. Isso vale para valores de `arguments` provenientes de propriedades ou de `arguments` provenientes de paramentros.

**Exemplo:**

```csharp
public class Program
{
    public static void Main(string[] args)
    {
        new App().Run(args);

        // EnableMultiAction = false
        /*
        new App(null, false).Run(args);
        */
    }

    public class MyCommand : Command
    {
        public string Action1(string value = null)
        {
            return "Action1";
        }

        public string Action2(string value = null)
        {
            return "Action2";
        }
    }
}
```

```
MyApp.exe action1
Action1 (value = default)

MyApp.exe action2
Action2 (value = default)

MyApp.exe action1 action2
Action1 (value = default)
Action2 (value = default)

MyApp.exe action1 action2 action1 action1 action2
Action1 (value = default)
Action2 (value = default)
Action1 (value = default)
Action1 (value = default)
Action2 (value = default)

MyApp.exe action1 --value \\action2
Action1 (value = action2)
```

O último exemplo demostra como usar o scape em seus valores que conflitam com nomes de `actions`. Um fato importante é que no exemplo foi usado duas barras invertidas para fazer o scape, mas isso pode variar de console para console, no `bash` o uso de apenas uma barra invertida não tem nenhum efeito, provavelmente ele deve usar para outros scapes antes de chegar na aplicação.

## Controle de eventos

Os eventos são importantes para interceptar cada passo da execução e modificar ou extender o comportamento padrão. Os eventos existentes são os seguintes:

* App.OnBeforeMemberInvoke(ApplicationResult, IMemberResult): Chamado antes do invoke de cada membro (propriedade ou metodo) que foi parseado.
* App.OnAfterMemberInvoke(ApplicationResult, IMemberResult): Chamado depois do invoke de cada membro (propriedade ou metodo) que foi parseado.
* App.OnMethodReturn(ApplicationResult, IMemberResult): : Chamado sempre que um metodo retorna valor
* App.OnComplete(ApplicationResult): Chamado ao fim da execução
* App.OnException(ApplicationResult, Exception): Chamado em caso de exception.

**Exemplo:**

```csharp
public class Program
{
    public static void Main(string[] args)
    {
        var app = new App();
        
        app.OnBeforeMemberInvoke += (appResult, memberResult) =>
        {
            app.Console.Write("Before: " + memberResult.Name);
        };

        app.OnAfterMemberInvoke += (appResult, memberResult) =>
        {
            app.Console.Write("After: " + memberResult.Name);
        };

        app.OnMethodReturn += (appResult, memberResult) =>
        {
            app.Console.Write("After MethodReturn: " + memberResult.Name);
        };

        app.OnComplete += (appResult) =>
        {
            app.Console.Write("Count: " + appResult.ExecutionResult.Results.Count());
            throw new Exception("Some error!!!");
        };

        app.OnException += (appResult, exception) =>
        {
            app.Console.Write(exception.Message);
        };

        app.Run(args);
    }

    public class FirstCommand : Command
    {
        public string MyProperty { get; set; }

        public string MyAction()
        {
            return "Return MyAction";
        }
    }
}
```

```
MyApp.exe --my-property value my-action
Before: MyProperty
After: MyProperty
Before: MyAction
After: MyAction
Return MyAction
After MethodReturn: MyAction
Count: 2
Some error!!!
```

No exemplo acima o controle passou para quem implementou os eventos e cada um dos eventos foram executados em sua respectiva ordem. 

Por padrão nos inserimos um handler chamado `SysCommand.ConsoleApp.Handlers.DefaultApplicationHandler` que é responsável pelo mecanismo padrão de `outputs` e controles de `erros`. Esse handler foi o responsável imprimir a linha "Return MyAction" do output acima. Para desliga-lo e ter o controle total dos eventos, basta desabilitar a flag `addDefaultAppHandler = false` no construtor.

```csharp
new App(addDefaultAppHandler: false).Run(args);
```

Outro modo de adicionar eventos é usando a interface `SysCommand.ConsoleApp.Handlers.IApplicationHandler`. Dessa maneira sua regra fica isolada, mas tendo o contraponto de ser obrigado a implementar todos os métodos da interface. Para adicionar um novo handler siga o exemplo abaixo:

```csharp
new App(addDefaultAppHandler: false)
        .AddApplicationHandler(new CustomApplicationHandler())
        .Run(args);
```


# Tipos suportados

string
bool
decimal
double
int
uint
DateTime
byte
short
ushort
long
ulong
float
char
Enum
Enum with Flags
Generic collections (IEnumerable, IList, ICollection)
Arrays

Syntax

[action-name ][-|--|/][argument-name][=|:| ][value]

Boolean syntax

MyApp.exe -a  // true
MyApp.exe -a- // false
MyApp.exe -a+ // true
MyApp.exe -a - // false
MyApp.exe -a + // true
MyApp.exe -a true // true
MyApp.exe -a false // false
MyApp.exe -a 0 // true
MyApp.exe -a 1 // false

Multiple assignments syntax

MyApp.exe -abc  // true for a, b and c
MyApp.exe -abc- // false for a, b and c
MyApp.exe -abc+ // true for a, b and c

Enum syntax

[Flags]
public enum Verbose
{
    None = 0,
    All = 1,
    Info = 2,
    Success = 4,
    Critical = 8,
    Warning = 16,
    Error = 32,
    Quiet = 64
}

MyApp.exe --verbose Error Info Success
MyApp.exe --verbose 32 2 Success

Generic collections or Array sintax

public void MyAction(IEnumerable<decimal> myLst, string[] myArray = null);

MyApp.exe --my-lst 1.0 1.99
MyApp.exe 1.0 1.99 // positional
MyApp.exe --my-lst 1.0 1.99 --my-array str1 str2
MyApp.exe 1.0 1.99 str1 str2 // positional

Importante!

Todos as conversões levam em consideração a cultura configurada na propriedade estática "CultureInfo.CurrentCulture".

# Help automatico

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

## Comportamento padrão

O formato padrão do help leva em consideração todos os elementos que compõem o sistema, ou seja, `Commands`, `Arguments`  e `Actions`. O formato de saída que será exibido será o seguinte:

```
usage:    [--argument=<phrase>] [--argument-number=<number>]
          [-v, --verbose=<None|All|Info|Success|Critical|Warning|Error|Quiet>]
          --required-argument=<phase>
          <actions[args]> (A)

Command help (B)
    LongName (C1), ShortName (C2)      Help text for arguments of command (properties) (C3). Complement (C4)
    Action (D)
      LongName (E1), ShortName (E2)    Help text for arguments of actions (parameters) (E3). Complement (E4)

Use 'help --action=<name>' to view the details of
any action. Every action with the symbol "*" can
have his name omitted. (F)
```

A fonte de cada texto esta em cada elemento `Commands`, `Arguments`  e `Actions` e os textos complementares estão na classe estática `SysCommand.ConsoleApp.Strings`. Segue o mapeamento de cada texto conforme o formato exibido acima:

* A: O texto e `usage` é gerado internamente pela classe `DefaultDescriptor` e sempre será exibido.
* B: O texto do `Command` sempre será exibido e a sua fonte vem da propriedade `Command.HelpText` que deve ser definida no construtor do seu comando. Caso você não atribua nenhum valor para essa propriedade, o padrão será exibir o nome do comando.
* C: Será exibido todas os argumentos (propriedades) do comando, um em baixo do outro.
  * C1: A fonte desse texto vem do atributo `ArgumentAtrribute(LongName="")`.
  * C2: A fonte desse texto vem do atributo `ArgumentAtrribute(ShortName="")`.
  * C3: A fonte desse texto vem do atributo `ArgumentAtrribute(Help="")`.
  * C4: Esse texto só vai aparecer se a flag `ArgumentAtrribute(ShowHelpComplement=true)` estiver ligada. O texto que será exibido vai depender da configuração do membro:
    * `Strings.HelpArgDescRequired`: Quando o membro é obrigatório
    * `Strings.HelpArgDescOptionalWithDefaultValue`: Quando o membro é opcional e tem default value.
    * `Strings.HelpArgDescOptionalWithoutDefaultValue`: Quando o membro é opcional e não tem default value.
* D: A fonte desse texto vem do atributo `ActionAtrribute(Name="")`.
* E: São as mesmas fontes dos argumentos de comando (propriedades), pois ambos os membros utilizam o mesmo atributo.
* F: Texto complementar para explicar como o help funciona. A fonte desse texto vem da classe `Strings.HelpFooterDesc`.

**Exemplo:**

```csharp
public class HelpCommand : Command
{
    // With complement
    [Argument(Help = "My property1 help")]
    public string MyProperty1 { get; set; }

    // Without complement
    [Argument(Help = "My property2 help", ShowHelpComplement = false, IsRequired = true)]
    public int MyProperty2 { get; set; }

    public HelpCommand()
    {
        this.HelpText = "Help for this command";
    }

    [Action(Help = "Action help")]
    public void MyActionHelp
    (
        [Argument(Help = "Argument help")]
        string arg0, // With complement

        [Argument(Help = "Argument help", ShowHelpComplement = false)]
        string arg1  // Without complement
    )
    {

    }
}
```

```
usage:    --my-property2=<number> [--my-property1=<phrase>] [-v,
          --verbose=<None|All|Info|Success|Critical|Warning|Error|Quiet>]
          <actions[args]>

Help for this command

   --my-property1    My property1 help. Is optional.
   --my-property2    My property2 help

   my-action-help    Action help
      --arg0         Argument help. Is required.
      --arg1         Argument help
```

* Para mais informações sobre customizações do help em propriedades veja o tópido de `Trabalhando com propriedades`.
* Para mais informações sobre customizações do help em ações veja o tópido de `Trabalhando com métodos`.

# Tipos de comandos

Atualmente existem tres tipos de comandos:

**Comandos de usuário**

São os comandos comuns e que herdam apenas da class `Command`. 

**Comandos de help**

São os comandos que herdam da classe `Command` e implementam a interface `IHelpCommand`. Contudo, apenas um será utilizado. 

**Comandos de debug**

Os comandos de debug são comandos que são carregados apenas durante o debugging do Visual Studio. Um bom exemplo seria o comando interno "ClearCommand", ele disponibiliza a action "clear" para limpar o prompt de comando que o Visual Studio abre durante o processo de debug. Para criar um comando de debug basta habilitar a flag `Command.OnlyInDebug`.

```csharp
public class ClearCommand : Command
{
    public ClearCommand()
    {
        this.HelpText = "Clear window. Only in debug";
        this.OnlyInDebug = true;
    }

    public void Clear()
    {
        Console.Clear();
    }
}
```

# Verbose

O controle de exibição por verbo esta contido em um comando interno chamado `VerboseCommand`. A sua função é alterar o valor da propriedade `App.Console.Verbose` caso o usuário envie um input de verbose. Atualmente, os verbos suportados são:

* All: Exibe todos os verbos
* Info: É o verbo padrão, sempre será exibido, ao menos que o usuário envie o verbo `Quiet`.
* Success: Verbo para mensagens de sucesso.
* Critical: Verbo para mensagens criticas.
* Warning: Verbo para mensagens de warning.
* Error: Verbo para mensagens de erro. O sistema força o envio desse verbo em caso de erros de parse.
* Quiet: Verbo para não exibir nenhuma mensagem, porém se a mensagem estiver sendo forçada, esse verbo é ignorado para essa mensagem.

Para que a funcionalidade funcione corretamente é obrigatorio o uso das funções de output contidas dentro da classe `ConsoleWrapper` e que tem uma instancia disponível na propriedade `App.Console`. 

**Exemplo:**

```csharp
public class TestVerbose : Command
{
    public void Test()
    {
        this.App.Console.Write("output of info"); 
        this.App.Console.Error("output of error");
        this.App.Console.Error("output of error forced", forceWrite: true);
        this.App.Console.Critical("output of critical");
    }
}
```

Forma curta: ```MyApp.exe test -v Critical```

Forma longa: ```MyApp.exe test --verbose Critical```

Outputs:

```
output of info
output of error forced
output of critical
```

* Para desativar o comando `VerboseCommand` veja o tópico de `Inicialização`.

# Output

O mecanismo de output foi extendido para aumentar a produtividade.

Primeiro, foi criado um pequeno wrapper da classe `System.Console` chamado `SysCommand.ConsoleApp.ConsoleWrapper` que esta disponivel dentro do contexto da aplicação na propriedade `App.Console`. Esse wrapper pode ser herdado e ter seus recursos modificados ou potencializados, mas por padrão temos as seguintes funcionalidades:

* Métodos de write para cada tipo de verbo
* Possibilidade de customização da cor do texto de cada verbo
  * App.Console.ColorInfo
  * App.Console.ColorCritical
  * App.Console.ColorError
  * App.Console.ColorSuccess
  * App.Console.ColorWarning
  * App.Console.ColorRead
* Variavel de controle de tipo de saída `App.Console.ExitCode` onde você pode usa-la como retorno do seu método `int Main(string[] args)`:
  * "0" : Sucesso
  * "1" : Erro
* Quebra de linha inteligente durante o uso dós métodos de write e read. A variável `App.Console.BreakLineInNextWrite` controla as quebras e te ajuda a não deixar linhas vazias sem necessidade.

Outro recurso seria a utilização dos `returns` das actions e que serão, caso existam, utilizados como output. Esse recurso se assemelha muito ao "AspNet MVC".

**Exemplo:**

```csharp
public class TestOutput : Command
{
    public decimal Test()
    {
       var result = this.App.Console.Read("My question: ");

        if (result != "S")
        { 
            // option1: use write method in wrapper class
            this.App.Console.Write(1.1m);

            // option2: use .NET class directly
            Console.WriteLine(2.2m);
        }

        // option3: or use return, its the same the option1
        return 3.3m;
    }
}
```

Input: ```MyApp.exe test```

Outputs:

```
My question: N
1.1
2.2
3.3
```

Por último, vale lembrar que nada disso impede você de usar os mecanismos comuns do .NET, como a classe "System.Console".


# Output usando template Razor

Outra opção para exibir outputs é a utilização de templates `Razor`. Esse mecanismo foi projetado para coisas simples, é muito importante dizer que ele não dispõe de diversos recursos como: debug, intellisense, highlight e analise de erros.

Para utilizar `Razor` deve-se seguir alguns simples passos:

* Por organização, criar uma pasta chamada "Views". 
* Caso ainda queira mais organização, crie uma sub-pasta dentro da pasta "Views" com o nome do `Command`.
* Criar um arquivo de template com a extensão ".razor" dentro da pasta "Views". Esse arquivo deve ter o mesmo nome da action (método)
* Implementar o seu template podendo ou não usar a variável "@Model"
* Exibir as propriedades do arquivo ".razor" e configura-lo com a **Build Action = Embedded Resource** ou com a propriedade **Copy to Output = Copy aways**. Isso é necessário para o gerenciador de template encontre o arquivo na basta "bin/" em caso do uso do **Copy to Output** ou dentro do Assembly do domínio de aplicativo padrão com o uso do **Build Action**.

**Exemplo:**

######Commands/ExampleRazorCommand.cs

```csharp
public class ExampleRazorCommand : Command
{
    public string MyAction()
    {
        return View<MyModel>();
    }

    public string MyAction2()
    {
        var model = new MyModel
        {
            Name = "MyName"
        };

        return View(model, "MyAction.razor");
    }

    public class MyModel
    {
        public string Name { get; set; }
    }
}
```

######Views/ExampleRazor/MyAction.razor

```
@if (Model == null)
{
    <text>#### HelloWorld {NONE} ####</text>
}
else {
    <text>#### HelloWorld (@Model.Name) ####</text>
}
```

######Tests

Input1: ```MyApp.exe my-action```

Input2: ```MyApp.exe my-action2```

Outputs:

```
    #### HelloWorld {NONE} ####
    #### HelloWorld {MyName} ####
```

######Observação

* A pesquisa do template via `Arquivo físico` ou via `Embedded Resource` segue a mesma lógica. Ele busca pelo caminho mais especifico usando o nome do "command.action.extensão" e caso ele não encontre ele tentará encontrar pelo nome mais generico, sem o nome do command.
  * Busca primeiro por: "ExampleRazorCommand.MyAction.razor"
  * Caso não encontre na primeira tentativa, então busca por: "MyAction.razor"
* É possível passar o nome da view diretamente, sem a necessidade de usar a pesquisa automatica. como no exemplo da action "MyAction2()".
* Por questões técnicas, o método View<>() obriga o uso de uma inferencia ou um model. Infira um `object` se você não necessitar de um model `View<object>()`.
* Devido ao uso do recurso de `Razor`, o seu projeto terá uma dependencia da dll `System.Web.Razor`.

# Output usando template T4

Outra opção para exibir outputs é a utilização de templates `T4`. Esse mecanismo, ao contrário dos templates `Razor` é mais completo, ele não perdeu nenhum dos beneficios que o Visual Studio nos fornece. Basta seguir apenas alguns passos para usa-lo:

* Por organização, criar uma pasta "Views"
* Criar um arquivo T4 no formato "Runtime Text Template"
* Se for utilizar model é preciso configurar um parametro, que por obrigatoriedade, deve-se chamar "Model" e ter o seu respectivo tipo em forma de string. Caso não utilize nenhum "Model" então ignore esse passo.
* Implementar o seu template

**Exemplo:**

######Commands/ExampleT4Command.cs

```csharp
public class ExampleT4Command : Command
{
    public string T4MyAction()
    {
        return ViewT4<MyActionView>();
    }

    public string T4MyAction2()
    {
        var model = new MyModel
        {
            Name = "MyName"
        };

        return ViewT4<MyActionView, MyModel>(model);
    }

    public class MyModel
    {
        public string Name { get; set; }
    }
}
```
######Views/ExampleT4/MyActionView.tt

```csharp
<#@ parameter type="Example.T4Command.MyModel" name="Model" #>
<# if(Model == null) { #>
#### HelloWorld {NONE} ####
<# } #>
<# else { #>
#### HelloWorld (<#= Model.Name #>) ####
<# } #>
```

######Tests

Input1: ```MyApp.exe t4-my-action```

Input2: ```MyApp.exe t4-my-action2```

Outputs:

```
    #### HelloWorld {NONE} ####
    #### HelloWorld {MyName} ####
```

# Output tabelado

A classe `SysCommand.ConsoleApp.View.TableView` tras o recurso de `output tabelado` que pode ser muito útil para apresentar informações de forma rápida e visualmente mais organizada. É claro que tudo depende da quantidade de informação que você quer exibir, quanto maior, pior a visualização.

**Exemplo:**

######Commands/TableCommand.cs

```csharp
public class TableCommand : Command
{
    public string MyTable()
    {
        var list = new List<MyModel>
        {
            new MyModel() {Id = "1", Column2 = "Line 1 Line 1"},
            new MyModel() {Id = "2 " , Column2 = "Line 2 Line 2"},
            new MyModel() {Id = "3", Column2 = "Line 3 Line 3"}
        };

        return TableView.ToTableView(list)
                        .Build()
                        .ToString();
    }

    public class MyModel
    {
        public string Id { get; set; }
        public string Column2 { get; set; }
    }
}
```
######Tests

Input1: ```MyApp.exe my-table```

Outputs:

```
Id   | Column2
--------------------
1    | Line 1 Line 1
--------------------
2    | Line 2 Line 2
--------------------
3    | Line 3 Line 3
--------------------
```

# Gerenciamento de históricos de argumentos

Esse recurso permite que você salve aqueles inputs que são utilizados com muita frequencia e podem ser persistidos indeterminadamente. O seu funcionamento é bem simples, uma `Command` interno chamado `SysCommand.ConsoleApp.Commands.ArgsHistoryCommand` é responsável por indentificar as `actions` de gerenciamento e persisti-lo em um arquivo `Json` no caminho padrão `.app/history.json`. As `actions` de gerenciamento são as seguintes:

* `history-save   [name]`
* `history-load   [name]`
* `history-delete [name]` 
* `history-list`

**Exemplo:**

```csharp
public class TestArgsHistories : Command
{
    public void TestHistoryAction()
    {
        this.App.Console.Write("Testing"); 
    }
}
```

```
C:\MyApp.exe test-history-action history-save "CommonCommand1"
Testing

C:\MyApp.exe history-load "CommonCommand1"
Testing

C:\MyApp.exe history-list
[CommonCommand1] test-history-action

C:\MyApp.exe history-remove "CommonCommand1"
{No output}

C:\MyApp.exe history-list
{No output}
```

* Para desativar o comando `ArgsHistoryCommand` veja o tópico de `Inicialização`.
* A action `history-load` retorna um objeto do tipo `RedirectResult` que força o redirecionamento para um novo comando. Qualquer input depois dessa action será desprezado. Veja o tópico `Redirecionamento de comandos`.
* Esse recurso só vai funcionar se a flag `App.EnableMultiAction` estiver ligada.

# Redirecionamento de comandos

Para redirecionar a sua aplicação com uma nova sequencia de comandos é muito simples, basta a sua action retornar uma instancia da classe `RedirectResult` passando em seu construtor uma string contendo a nova sequencia de comandos. Vale ressaltar que as instancias dos comandos serão as mesmas, ou seja, o estado de cada comando não voltará ao inicio, apenas o fluxo de execução. Outro ponto importante é que qualquer input depois dessa action não será chamado, ou seja, a execução reinicia com o novo comando no momento em que existe um retorno do tipo `RedirectResult`.

**Exemplo:**

```csharp
public class RedirectCommand : Command
{
    private int _count;

    public RedirectResult RedirectNow(string arg)
    {
        _count++;
        App.Console.Write($"Redirecting now!!. Count: {_count}");
        return new RedirectResult("redirected", "--arg", arg);
    }

    public string Something()
    {
        return "Something";
    }
    
    public string Redirected(string arg)
    {
        _count++;
        return $"Redirected: {arg}. Count: {_count}";
    }
}
```

No exemplo abaixo a action `Something` será executada, pois esta antes do redirect.

```
C:\MyApp.exe something redirect-now my-value
Something
Redirecting now!!. Count: 1
Redirected: my-value. Count: 2
```

No exemplo abaixo a action `Something` não será executada, pois esta depois do redirect.

```
C:\MyApp.exe redirect-now my-value something
Redirecting now!!. Count: 1
Redirected: my-value. Count: 2
```

* Para desabilitar o recurso de multi-action, desative a propriedade `App.EnableMultiAction` antes do método `App.Run()`.

# Cancelamento da continuidade da execução

Quando existem muitas actions com o mesmo nome e assinatura, todas elas serão executadas juntas quando solicitada pelo usuário. Porém, você pode impedir isso usando o comando `ExecutionScope.StopPropagation()` dentro da sua action que você deseje que seja a última na pilha de execução.

**Exemplo:**

```csharp
public class StopPropagationCommand1 : Command
{
    public string StopPropagationAction1(bool cancel = false)
    {
        return "StopPropagationCommand1.StopPropagationAction1";
    }

    public string StopPropagationAction2()
    {
        return "StopPropagationCommand1.StopPropagationAction2";
    }
}

public class StopPropagationCommand2 : Command
{
    public string StopPropagationAction1(bool cancel = false)
    {
        if (cancel)
        {
            ExecutionScope.StopPropagation();
        }

        return "StopPropagationCommand2.StopPropagationAction1";
    }

    public string StopPropagationAction2()
    {
        return "StopPropagationCommand2.StopPropagationAction2";
    }
}

public class StopPropagationCommand3 : Command
{
    public string StopPropagationAction1(bool cancel = false)
    {
        return "StopPropagationCommand3.StopPropagationAction1";
    }

    public string StopPropagationAction2()
    {
        return "StopPropagationCommand3.StopPropagationAction2";
    }
}
```

```
C:\MyApp.exe stop-propagation-action1
StopPropagationCommand1.StopPropagationAction1
StopPropagationCommand2.StopPropagationAction1
StopPropagationCommand3.StopPropagationAction1

C:\MyApp.exe stop-propagation-action1 --cancel
StopPropagationCommand1.StopPropagationAction1
StopPropagationCommand2.StopPropagationAction1
```

Perceba que ao utilizar o argumento "--cancel" a action "StopPropagationCommand3.StopPropagationAction1" não foi executada. Isso por que ela estava na última posição da pilha de execução e como a action "StopPropagationCommand2.StopPropagationAction1" cancelou a continuidade da execução, qualquer outra action da sequencia sera ignorada.

Outra possibilidade de uso do `StopPropagation` é quando existem multiplas actions no mesmo input. A lógica é a mesma, será cancelado todas as actions da pilha que estão depois da action que disparou o stop.

```
C:\MyApp.exe stop-propagation-action1 stop-propagation-action2
StopPropagationCommand1.StopPropagationAction1
StopPropagationCommand2.StopPropagationAction1
StopPropagationCommand3.StopPropagationAction1
StopPropagationCommand1.StopPropagationAction2
StopPropagationCommand2.StopPropagationAction2
StopPropagationCommand3.StopPropagationAction2

C:\MyApp.exe stop-propagation-action1 --cancel stop-propagation-action2
StopPropagationCommand1.StopPropagationAction1
StopPropagationCommand2.StopPropagationAction1
```
Perceba que a execução parou no mesmo ponto.

* Para desabilitar o recurso de multi-action, desative a propriedade `App.EnableMultiAction` antes do método `App.Run()`.

# Tratamento de erros

O tratamento de erro é gerado de forma automatica pelo sistem e são categorizados da seguinte forma:

* Erros no processo de parse: São erros que ocorrem no processo de parse e são sub-categorizados da seguinte forma:
  * `ArgumentParsedState.ArgumentAlreadyBeenSet`: Indica que um argumento esta duplicado no mesmo input.
  * `ArgumentParsedState.ArgumentNotExistsByName`: Indica que um argumento nomeado não existe.
  * `ArgumentParsedState.ArgumentNotExistsByValue`: Indica que um argumento posicional não existe
  * `ArgumentParsedState.ArgumentIsRequired`: Indica que um argumento é obrigatório
  * `ArgumentParsedState.ArgumentHasInvalidInput`: Indica que um argumento esta inválido
  * `ArgumentParsedState.ArgumentHasUnsupportedType`: Indica que o esta tudo certo com o input, porém o tipo do argumento não tem suporte. Veja a lista de tipos suportados.
* Not Found: Nenhuma rota encontrada para o input solicitado.
* Exception génerica: Não existe nenhum tipo de tratamento padrão, mas é possível interceptar qualquer exception dentro do evento `App.OnException`.

**Exemplo:**

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
                app.OnException += (appResult, exception) =>
                {
                    app.Console.ExitCode = ExitCodeConstants.Error;
                    app.Console.Write(exception.Message);
                };
                return app;
            }
        );
    }

    public class CustomDescriptor : DefaultDescriptor
    { 
        public override void ShowErrors(ApplicationResult appResult)
        {
            foreach (ExecutionError error in appResult.ExecutionResult.Errors)
            {
                foreach (ArgumentParsed prop in error.PropertiesInvalid)
                {
                    if (prop.ParsingStates.HasFlag(ArgumentParsedState.ArgumentAlreadyBeenSet))
                        appResult.App.Console.Error(string.Format("The argument '{0}' has already been set", prop.GetArgumentNameInputted()));
                    if (prop.ParsingStates.HasFlag(ArgumentParsedState.ArgumentNotExistsByName))
                        appResult.App.Console.Error(string.Format("The argument '{0}' does not exist", prop.GetArgumentNameInputted()));
                    if (prop.ParsingStates.HasFlag(ArgumentParsedState.ArgumentNotExistsByValue))
                        appResult.App.Console.Error(string.Format("Could not find an argument to the specified value: {0}", prop.Raw));
                    if (prop.ParsingStates.HasFlag(ArgumentParsedState.ArgumentIsRequired))
                        appResult.App.Console.Error(string.Format("The argument '{0}' is required", prop.GetArgumentNameInputted()));
                    if (prop.ParsingStates.HasFlag(ArgumentParsedState.ArgumentHasInvalidInput))
                        appResult.App.Console.Error(string.Format("The argument '{0}' is invalid", prop.GetArgumentNameInputted()));
                    if (prop.ParsingStates.HasFlag(ArgumentParsedState.ArgumentHasUnsupportedType))
                        appResult.App.Console.Error(string.Format("The argument '{0}' is unsupported", prop.GetArgumentNameInputted()));
                }

                foreach (ActionParsed method in error.MethodsInvalid)
                {
                    foreach (ArgumentParsed parameter in method.Arguments)
                    {
                        ...
                    }
                }
            }
        }

        public override void ShowNotFound(ApplicationResult appResult)
        {
            appResult.App.Console.Error("Could not find any action.", forceWrite: true);
        }
    }
}
```

# Trabalhando com propriedades

O trabalho com propriedades é muito simples e objetivo, basta criar suas propriedades como publicas e escolher um dos dois meios abaixo para saber se uma propriedade foi inputada pelo usuário, você que escolhe qual utilizar:

######Modo de uso 1

Primeiro, você pode utilizar o método `Main()` sem parametro e que, por convensão de nome, será o responsável por ser invocado caso alguma de suas propriedade tenha sido utilizadas no input do usuário. O nome "Main" foi escolhido para manter o padrão de nomenclatura que o .NET utiliza em aplicações de console. 

Por segurança, utilize todos os tipos primitivos como `Nullable` para garantir que o usuário fez o input. Ou utilize o método `GetArgument(string name)` para verificar se uma propriedade foi parseada. Vale ressaltar que uma propriedade com `Default value` sempre terá resultado de parse e caso necessário, utilize mais uma verificação para saber se o resultado partiu de um input do usuário.

**Exemplo:**

```csharp
public string Main()
{
    if (this.MyProperty != null)
        App.Console.Write("Has MyProperty");

    if (this.MyPropertyInt != null)
        App.Console.Write("Safe mode: MyPropertyInt");

    if (this.MyPropertyUnsafeMode == 0)
        App.Console.Write("Unsafe mode: Preferably, use nullable in MyPropertyUnsafeMode");

    if (this.GetArgument("MyPropertyUnsafeMode") != null)
        App.Console.Write("Safe mode, but use string: MyPropertyUnsafeMode");

    if (this.GetArgument(nameof(MyPropertyUnsafeMode)) != null)
        App.Console.Write("Safe mode, but only in c# 6: MyPropertyUnsafeMode");

    if (this.GetArgument(nameof(MyPropertyDefaultValue)) != null)
        App.Console.Write("MyPropertyDefaultValue aways has value");

    // if necessary, add this verification to know if property had input.
    if (this.GetArgument(nameof(MyPropertyDefaultValue)).IsMapped)
        App.Console.Write("MyPropertyDefaultValue has input");

    return "Main() methods can also return values ;)";
}
```

```
C:\MyApp.exe --my-property value
Has MyProperty
Unsafe mode: Preferably, use nullable in MyPropertyUnsafeMode
MyPropertyDefaultValue aways has value
Main() methods can also return values ;)

C:\MyApp.exe --my-property-int 0
Safe mode: MyPropertyInt
Unsafe mode: Preferably, use nullable in MyPropertyUnsafeMode
MyPropertyDefaultValue aways has value
Main() methods can also return values ;)

C:\MyApp.exe --my-property-unsafe-mode 0
Unsafe mode: Preferably, use nullable in MyPropertyUnsafeMode
Safe mode, but use string: MyPropertyUnsafeMode
Safe mode, but only in c# 6: MyPropertyUnsafeMode
MyPropertyDefaultValue aways has value
Main() methods can also return values ;)

C:\MyApp.exe --my-property-default-value 0
Unsafe mode: Preferably, use nullable in MyPropertyUnsafeMode
MyPropertyDefaultValue aways has value
MyPropertyDefaultValue has input
Main() methods can also return values ;)
```
Tenha muito cuidado com propriedades com `Default values`, o fato dela ter valor por padrão faz com que o método `Main()` sempre seja chamado mesmo quando não exista nenhum input.

```
C:\MyApp.exe
Unsafe mode: Preferably, use nullable in MyPropertyUnsafeMode
MyPropertyDefaultValue aways has value
Main() methods can also return values ;)
```
######Modo de uso 2

Por fim, você ainda pode utilizar o `set { .. }` da sua propriedade para tomar alguma ação. Esse recurso não é recomendado, pois o método `GetArgument(string name)` ainda não esta pronto para ser usado nesse momento, mas caso queira algo pontual e rápido, nada te impede de usar esse meio.

```csharp
public class TestProperty2Command : Command
{
    public bool MyCustomVerbose
    {
        set
        {
            if (value)
                Console.WriteLine("MyCustomVerbose=true");
            else
                App.Console.Write("MyCustomVerbose=false");
        }
    }
}
```

```
C:\MyApp.exe --my-custom-verbose
MyCustomVerbose=true

C:\MyApp.exe --my-custom-verbose false
MyCustomVerbose=false
```

## Customizando os nomes dos argumentos

A regra a seguir descreve como é o comportamento padrão de nomenclatura para que uma propriedade vire um `argument`:

Primeiro se converte o nome da propriedade em minusculo, depois adiciona um traço "-" antes de cada letra maiuscula que estiver no meio ou no final do nome. No caso de propriedades com apenas uma letra, o padrão será deixar a letra minuscula e o input será aceito apenas na forma curta.

Essa é a regra padrão de nomenclarutura e você pode escolher usa-la ou customizada-la, para isso utilize o atributo `ArgumentAttribute`. O uso do atributo `ArgumentAttribute` é exclusivo, ao utiliza-lo você esta eliminando o padrão de nomenclatura por completo, ou seja, se você customizar a `forma curta` você será obrigado a customizar a `forma longa` também, e vice-versa. Do contrário só o formato customizado será habilitado.

**Exemplo:**

```csharp
public class CustomPropertiesNamesCommand : Command
{
    // customized with long and short option
    [Argument(LongName = "prop", ShortName = 'A')]
    public int? MyCustomPropertyName { get; set; }

    // only with long option
    public string NormalLong { get; set; }

    // customized only with short option
    [Argument(ShortName = 'B')]
    public string ForcedShort { get; set; }

    // only with short option
    public int? C { get; set; }

    public CustomPropertiesNamesCommand()
    {
    }

    public void Main()
    {
        if (MyCustomPropertyName != null)
            App.Console.Write("MyCustomPropertyName=" + MyCustomPropertyName);

        if (NormalLong != null)
            App.Console.Write("NormalLong=" + NormalLong);

        if (ForcedShort != null)
            App.Console.Write("ForcedShort=" + ForcedShort);

        if (C != null)
            App.Console.Write("C=" + C);
    }
}
```

```
C:\MyApp.exe --prop 111 --normal-long strvalue -B strvalue2 -c 9999
MyCustomPropertyName=111
NormalLong=strvalue
ForcedShort=strvalue2
C=9999

C:\MyApp.exe -A 111 --normal-long strvalue -B strvalue2 -c 9999
MyCustomPropertyName=111
NormalLong=strvalue
ForcedShort=strvalue2
C=9999
```

## Customizando as informações de help

Para configurar o texto de help utilize o atributo `ArgumentAttribute(Help="")`. Por padrão, para cada argumento será exibido um complemento após o texto do help. A informação que esse complemento nos tras é se o parametro é obrigatório ou opcional (com ou sem default value). Caso você deseje desativar esse complemento utilize o atributo `ArgumentAttribute(ShowHelpComplement=false)`.

```csharp
public class CustomPropertiesHelpCommand : Command
{
    // customized with long and short option
    [Argument(Help = "This is my property")]
    public int? MyPropertyHelp { get; set; }

    [Argument(Help = "This is my property 2", ShowHelpComplement = false)]
    public int? MyPropertyHelp2 { get; set; }

    public CustomPropertiesHelpCommand()
    {
        this.HelpText = "Custom help for CustomPropertiesHelpCommand";
    }
}
```

```
C:\MyApp.exe help
Custom help for CustomPropertiesHelpCommand

   --my-property-help              This is my property. Is optional.
   --my-property-help2             This is my property 2
```

Esse tópico apenas apresentará os atributos que configuram o help. Para mais informações sobre o help veja no tópico `Help Automatico`.

## Propriedades obrigatórias

Para argumentos que são obrigatórios, é necessário que você use o `ArgumentAtrribute` ligando a flag `IsRequired`.

**Exemplo:**

```csharp
public class TestProperty5Command : Command
{
    [Argument(IsRequired = true)]
    public string MyPropertyRequired { get; set; }

    public void Main()
    {
        if (MyPropertyRequired != null)
            App.Console.Write("MyPropertyRequired=" + MyPropertyRequired);
    }
}
```

```
C:\MyApp.exe
There are errors in command: TestProperty5Command
The argument '--my-property-required' is required

C:\MyApp.exe --my-property-required 123
MyPropertyRequired=123
```

## Habilitando o input posicional

Para habilitar o input posicional basta ligar a flag `EnablePositionalArgs` em seu `Command`, contudo é importante validar o quanto isso necessário, pois muitos inputs posicionais podem complicar muito o uso da sua aplicação. Apesar do `SysCommand` estar bem preparado para esse tipo de input, não queremos que você polua o seu input.

```csharp
public class TestProperty3Command : Command
{
    public int? MyPosicionalProperty1 { get; set; }
    public int? MyPosicionalProperty2 { get; set; }

    public TestProperty3Command()
    {
        this.EnablePositionalArgs = true;
    }

    public void Main()
    {
        if (MyPosicionalProperty1 != null)
            App.Console.Write("MyPosicionalProperty1=" + MyPosicionalProperty1);
        if (MyPosicionalProperty2 != null)
            App.Console.Write("MyPosicionalProperty2=" + MyPosicionalProperty2);
    }
}
```
```
C:\MyApp.exe --my-posicional-property1 1 --my-posicional-property2 2
MyPosicionalProperty1=1
MyPosicionalProperty2=2

C:\MyApp.exe 1 2
MyPosicionalProperty1=1
MyPosicionalProperty2=2

C:\MyApp.exe 1 --my-posicional-property2 2
MyPosicionalProperty1=1
MyPosicionalProperty2=2
```

Você pode também controlar a posição de cada propriedade dentro do input usando a configuração `Position`.

**Exemplo:**

```csharp
public class TestProperty3Command : Command
{
    [Argument(Position = 2)]
    public int? MyPosicionalProperty1 { get; set; }

    [Argument(Position = 1)]
    public int? MyPosicionalProperty2 { get; set; }

    public TestProperty3Command()
    {
        this.EnablePositionalArgs = true;
    }

    public void Main()
    {
        if (MyPosicionalProperty1 != null)
            App.Console.Write("MyPosicionalProperty1=" + MyPosicionalProperty1);
        if (MyPosicionalProperty2 != null)
            App.Console.Write("MyPosicionalProperty2=" + MyPosicionalProperty2);
    }
}
```

```
C:\MyApp.exe --my-posicional-property1 1 --my-posicional-property2 2
MyPosicionalProperty1=1
MyPosicionalProperty2=2

C:\MyApp.exe 1 2
MyPosicionalProperty1=2
MyPosicionalProperty2=1
```

## Ignorar propriedades publicas por uma escolha manual usando atributo

Para mudar o comportamente padrão de propriedades publicas, você precisa apenas desligar a flag `OnlyPropertiesWithAttribute` do `Command`. Com ela desligada o parseador deixará de olhar para as propriedades publicas e usará apenas as propriedades publicas e que tiverem o atributo `ArgumentAtrribute`.

**Exemplo:**

```csharp
public class TestProperty4Command : Command
{
    public int? MyPropertyWithoutAttribute { get; set; }

    [Argument]
    public int? MyPropertyWithAttribute { get; set; }

    public TestProperty4Command()
    {
        this.OnlyPropertiesWithAttribute = true;
    }

    public void Main()
    {
        if (MyPropertyWithAttribute != null)
            App.Console.Write("MyPropertyWithAttribute=" + MyPropertyWithAttribute);
    }
}
```

```
C:\MyApp.exe --my-property-with-attribute 1
MyPropertyWithAttribute=1

C:\MyApp.exe --my-property-without-attribute 1
There are errors in command: DoSomethingCommand
The argument '--my-property-without-attribute' does not exist
```

# Trabalhando com métodos

O trabalho com métodos também é muito bem simples, todos os métodos definidos como `public`, por padrão, serão habilitados para virarem `input actions` e estarem disponíveis para uso. O fato interessante é que você pode utilizar os recursos nativos do .NET deixando seu código mais limpo, como:

* Métodos sem parametros
* Métodos com parametros opcionais com `Default value`
* Métodos com sobrecargas
* Métodos com `return` onde o retorno do método, por padrão, será utilizado como output no console usando

## Métodos sem parametros

**Exemplo:**

```csharp
public class Method1Command : Command
{
    public string MyAction()
    {
        return "MyAction";
    }
}
```

```
C:\MyApp.exe my-action
MyAction
```

## Parametros optionais

Os parametros opcionais são uteis para evitar a criação de sobrecargas e no caso de uma aplicação console ajuda a criar `actions` com diversas opções, mas não obrigando o usuário a preencher todas. 

Por segurança, ao usar parametros opcionais, obte por utilizar todos os tipos primitivos como `Nullable` para _garantir que o usuário fez o input_. Ou utilize o método `GetAction()` para verificar se o parametro foi mapeado, ou seja, se teve algum tipo de input.

**Exemplo:**

```csharp
public class Method1Command : Command
{
    public string MyAction2(int? arg0 = null, int arg1 = 0)
    {
        // unsafe, because the user can enter with value "--arg1 0" and you never know.
        if (arg1 != 0)
            App.Console.Write("arg1 wrong way to do it!");

        // safe, but bureaucratic
        if (this.GetAction().Arguments.Any(f => f.Name == "arg1" && f.IsMapped))
            App.Console.Write("arg1 has input");

        // recommended. the best way! 
        if (arg0 != null)
            App.Console.Write("arg0 has input");

        return "MyAction2";
    }
}
```
```
C:\MyApp.exe my-action2
MyAction2

C:\MyApp.exe my-action2 --arg0 99
arg0 has input
MyAction2

C:\MyApp.exe my-action2 --arg1 0
arg1 has input
MyAction2
```

Observação: Não utilize o método `GetAction()` em métodos que não são `actions`, você terá uma exception.

## Sobrecargas

O recurso de sobrecarga de métodos é suportado da mesma forma que você faria para qualquer outra finalidade. Muitas vezes esse recurso pode ser mais interessante que usar parametros opcionais, o código fica mais limpo. Outras vezes isso não será possível, pois com parametros opcionais o usuário tem a opção de escolher qualquer parametro independentemente de sua posição no método, coisa que a sobrecarga não pode.

**Exemplo:**

```csharp
public class Method1Command : Command
{
    public string MyAction3()
    {
        return "MyAction3";
    }

    public string MyAction3(int arg0)
    {
        return "arg0 has input";
    }

    public void MyAction3(int arg0, int arg1)
    {
        App.Console.Write("arg0 has input");
        App.Console.Write("arg1 has input");
    }
}
```
```
C:\MyApp.exe my-action3
MyAction3

C:\MyApp.exe my-action3 --arg0 9
arg0 has input

C:\MyApp.exe my-action3 --arg0 9 --arg1 99
arg0 has input
arg1 has input

C:\MyApp.exe my-action3 --arg1 99]
There are errors in command: Method1Command
The argument '--arg1' does not exist
```

O último comando mostrou a limitação da sobrecarga com relação aos parametros opcionais. O parseador entendeu que os dois métodos com parametros `MyAction3` estão inválidos, veja:

* MyAction3(int arg0): Não tem o input "--arg1" que foi solicitado, portanto esta inválido.
* MyAction3(int arg0, int arg1): Tem o input "--arg1", mas não tem o input "--arg0", portanto esta inválido.

Nesse caso o parseador escolhera o unico método valido, ou seja, o método `MyAction3` _sem parametros_ e usará o argumento extra "--arg1" para tentar encontra-lo como propriedade em algum `Command`, porém essa propriedade não existe em nenhum lugar, gerando o erro.

## Usando inputs posicionais

Outro modo de chamar sua action no console é usando `input posicional`. Por padrão, todas as `action` aceitam argumentos posicionais, mais isso pode ser desabilitado usando o atributo `ActionAttribute(EnablePositionalArgs = false)`.

**Exemplo:**

```csharp
public string MyActionWithPosicional(int arg0, int arg1)
{
    return "MyActionWithPosicional";
}

[Action(EnablePositionalArgs = false)]
public string MyActionWithoutPosicional(int arg0, int arg1)
{
    return "MyActionWithoutPosicional";
}
```
```
C:\MyApp.exe my-action-with-posicional --arg0 1 --arg1 2
MyActionWithPosicional

C:\MyApp.exe my-action-with-posicional 1 2
MyActionWithPosicional

C:\MyApp.exe my-action-without-posicional --arg0 1 --arg1 2
MyActionWithoutPosicional

C:\MyApp.exe my-action-without-posicional 1 2
There are errors in command: Method1Command
Error in method: my-action-without-posicional(Int32, Int32)
The argument '--arg0' is required
The argument '--arg1' is required
```

## Ignorar métodos publicos por uma escolha manual usando atributo

Para mudar o comportamente padrão de métodos publicos, você precisa apenas desligar a flag `OnlyMethodsWithAttribute` do `Command`. Com ela desligada o parseador deixará de olhar para as métodos publicos e usará apenas os métodos publicos e que tiverem o atributo `ActionAtrribute`.

**Exemplo:**

```csharp
public class Method2Command : Command
{
    public Method2Command()
    {
        this.OnlyMethodsWithAttribute = true;
    }

    [Action]
    public string MyActionWithAttribute()
    {
        return "MyActionWithAttribute";
    }

    public string MyActionWithoutAttribute()
    {
        return "MyActionWithAttribute";
    }
}
```

```
C:\MyApp.exe my-action-with-attribute
MyActionWithAttribute

C:\MyApp.exe my-action-without-attribute
Could not find any action.
```

Outra forma de ignorar métodos publicos e sem alterar o comportamento padrão da propriedade `OnlyMethodsWithAttribute` do `Command` é utilizando o atributo `ActionAttribute(Ignore = true)`.

**Exemplo:**

```csharp
public class Method3Command : Command
{
    public string MyActionNotIgnored()
    {
        return "MyActionNotIgnored";
    }

    [Action(Ignore = true)]
    public string MyActionIgnored()
    {
        return "MyActionIgnored";
    }
}
```

```
C:\MyApp.exe my-action-not-ignored
MyActionNotIgnored

C:\MyApp.exe my-action-ignored
Could not find any action.
```

## Customizando nomes de actions e arguments

A regra a seguir descreve como é o comportamento padrão de nomenclatura para que os métodos vire uma `action` e um parametro vire um `argument`:

Primeiro se converte o nome do membro (métodos ou parametros) em minusculo, depois adiciona um traço "-" antes de cada letra maiuscula que estiver no meio ou no final do nome. No caso de paramentros com apenas uma letra, o padrão será deixar a letra minuscula e o input será aceito apenas na forma curta.

Essa é a regra padrão de nomenclarutura e você pode escolher usa-la ou customizada-la de modo total ou parcial. Para isso utilize os atributos `ActionAttribute` para métodos e `ArgumentAttribute` os parametros. O uso do atributo `ArgumentAttribute` é exclusivo, ao utiliza-lo você esta eliminando o padrão de nomenclatura por completo, ou seja, se você customizar a `forma curta` você será obrigado a customizar a `forma longa` também, e vice-versa. Do contrário só o formato customizado será habilitado.

**Exemplo:**

```csharp
public class Method3Command : Command
{
    [Action(Name = "ActionNewName")]
    public string MyAction(
        [Argument(LongName = "longName1", ShortName = 'a')]
        string arg0, // customized with long and short option

        string arg1, // only with long option

        [Argument(ShortName = 'z')]
        string arg2, // only with short option

        string z     // only with short option
    )
    {
        return "ActionNewName";
    }
}
```

```
C:\MyApp.exe ActionNewName --longName1 1 --arg1 2 -z 3 -w 4
ActionNewName

C:\MyApp.exe ActionNewName -a 1 --arg1 2 -z 3 -w 4
ActionNewName
```

Outra opção de customização é a inclusão de prefixo antes do nome de cada `action`. Isso pode ser feito de duas formas, a primeira você só precisa ligar a flag de comando `Command.UsePrefixInAllMethods`. Com essa flag ligada, todas as `actions` passarão a ter o seguinte padrão de nome "CommandName-ActionName", ou seja, elas vão conter o nome do `Command` adicionado de um "-" seguido do nome da action. Caso o nome do comando tenha o sufixo "Command" então esse sufixo será removido.

Você ainda pode querer que essa flag não afete todas as suas `actions`, para isso utilize a flag `ActionAttribute(UsePrefix=false)` para que uma determinada action não tenha seu nome alterado com o prefixo.

```csharp
public class PrefixedCommand : Command
{
    public PrefixedCommand()
    {
        this.UsePrefixInAllMethods = true;
    }

    public string MyAction()
    {
        return "prefixed-my-action";
    }

    [Action(Name = "my-action2-custom")]
    public string MyAction2()
    {
        return "prefixed-my-action2-custom";
    }

    [Action(UsePrefix = false)]
    public string MyActionWithoutPrefix()
    {
        return "my-action-without-prefix";
    }
}
```

```
C:\MyApp.exe prefixed-my-action
prefixed-my-action

C:\MyApp.exe prefixed-my-action2-custom
prefixed-my-action2-custom

C:\MyApp.exe my-action-without-prefix
my-action-without-prefix
```

A segunda forma é você especificar qual será o prefixo de cada `action` usando a propriedade de commando `Command.PrefixMethods`. Assim o prefixo não será processado usando o nome do comando e sim especificado por você. Vale ressaltar que a flag `Command.UsePrefixInAllMethods` ainda precisa estar ligada.

**Exemplo:**

```csharp
public class Prefixed2Command : Command
{
    public Prefixed2Command()
    {
        this.PrefixMethods = "custom-prefix";
        this.UsePrefixInAllMethods = true;
    }

    public string MyAction()
    {
        return "custom-prefix-my-action";
    }
}
```

```
C:\MyApp.exe custom-prefix-my-action
custom-prefix-my-action
```

## Customizando as informações de help de actions e seus parametros

Para as `actions` você precisa customizar o atributo `ActionAttribute(Help="")` e para os paramentros utilizasse o atributo `ArgumentAttribute(Help="")`. Por padrão, para cada argumento será exibido um complemento após o texto do help. A informação que esse complemento nos tras é se o parametro é obrigatório ou opcional (com ou sem default value). Caso você deseje desativar esse complemento utilize o atributo `ArgumentAttribute(ShowHelpComplement=false)`.

**Exemplo:**

```csharp
public class MethodHelpCommand : Command
{
    public MethodHelpCommand() 
    {
        this.HelpText = "Help for this command";
    }

    [Action(Help = "Action help")]
    public string MyActionHelp(
        [Argument(Help = "Argument help")]
        string arg0, // With complement

        [Argument(Help = "Argument help", ShowHelpComplement = false)]
        string arg1  // Without complement
    )
    {
        return "Action help";
    }
}
```

```
C:\MyApp.exe help
Help for this command

   my-action-help                  Action help
      --arg0                       Argument help. Is required.
      --arg1                       Argument help
```

Esse tópico apenas apresentará os atributos que configuram o help. Para mais informações sobre o help veja no tópico `Help Automatico`.

## Trocando a posição de parametros posicionais

A propriedade `ArgumentAttribute(Position=X)` também funciona para parametros da mesma forma que funciona para propriedades. Não é um recurso que faça muito sentido, mas é importante documenta-lo.

**Exemplo:**

```csharp
public class Method5Command : Command
{
    public string MyActionWithArgsInverted(
        [Argument(Position = 2)]
        string arg0,
        [Argument(Position = 1)]
        string arg1
    )
    {
        return "MyActionWithArgsInverted";
    }
}
```

```
C:\MyApp.exe my-action-with-args-inverted 1 2
arg0 = '2'; arg1 = '1'
```

## Propriedades do atributos ArgumentAttribute que não são utilizados

As seguintes propriedades não fazem sentido no cenário de parametros de métodos e só existem por que o atributo `ArgumentAtrribute` é compartilhado no uso de propriedades.

* IsRequired: Em C#, todo parametro que não tem default value é obrigatório, essa configuração é ignorada se for utilizada.
* DefaultValue: Como o proprio C# já nos dá a opção de default value para parametros, essa configuração é redundante, sendo assim ela é ignorada por que o padrão do .NET já é suficiente e mais limpo.

## Métodos padrão

O uso de métodos padrão (ou métodos implicitos) fazem com que o recurso seja muito similar ao uso de propriedades, ou seja, você não é obrigado a especificar o nome da `action` e os seus parâmetros podem ser inseridos diretamente no input como se fossem argumentos provenientes de propriedades. 

Por convensão, se você chamar sua `action` de "Main" e ela tiver parametros, ela será considerada como padrão. Para mudar esse comportamento você deve desligar a flag `Action(IsDefault = false)`, assim o comportamento padrão será alterado e sua action "Main" (com parametros) não será mais acessada de forma implicita e obrigará a especificação de seu nome no input. O contrário também é verdadeiro, se sua action tem outro nome e você gostaria de torna-la um método padrão então basta ligar a flag `Action(IsDefault = true)`.

**Exemplo:**

```csharp
public class MethodDefaultCommand : Command
{
    public string Main(string arg0)
    {
        return "Main(string arg0)";
    }

    public string Main(string arg0, string arg1)
    {
        return "Main(string arg0, string arg1)";
    }

    [Action(IsDefault = false)]
    public string Main(int argument)
    {
        return "Main(int argument)";
    }

    [Action(IsDefault = true)]
    public string AnyName(string argument)
    {
        return "AnyName(string argument)";
    }

    [Action(IsDefault = true)]
    public string ActionWhenNotExistsInput()
    {
        return "ActionWhenNotExistsInput()";
    }
}
```

```
C:\MyApp.exe --arg0 value
Main(string arg0)

C:\MyApp.exe --arg0 value --arg1 value1
Main(string arg0, string arg1)

C:\MyApp.exe main --argument 9999
Main(int argument)

C:\MyApp.exe --argument value
AnyName(string argument)

C:\MyApp.exe
ActionWhenNotExistsInput()
```

**Observações:**

* É importante ressaltar que o todos os métodos padrão ainda podem ser chamados de forma explicita, ou seja, com o seu nome sendo especifico.
* O uso de método padrão sem argumentos só funciona se não existir nenhum argumento required, do contrário esse método nunca será chamado, pois haverá um erro obrigando o uso do argumento.'

# Tipos de inputs

Os argumentos, sejam eles paramentros de métodos ou propriedades, podem ter duas formas: a `longa` e a `curta`. Na forma `longa` o argumento deve-se iniciar com "--" ou "/" seguido do seu nome. Na forma `curta` ele deve sempre iniciar com apenas um traço "-" e seguido de apenas um caracter. Esse tipo de input (longo ou curto) é chamado de `input nomeado`.

Existe também a possibilidade de aceitar inputs posicionais, ou seja, sem a necessidade de utilizar os nomes dos argumentos. Esse tipo de input é chamado de `input posicional`.

**Exemplo:**

```csharp
public string MyProperty { get;set; }
public void MyAction(string A, string B);
```

**Input nomeado**:

```MyApp.exe my-action -a valueA -b valueB --my-property valueMyProperty```

**Input posicional**:

```MyApp.exe my-action valueA valueB valueMyProperty```

* Para as propriedades, o `input posicional` é desabilitado por padrão, para habilita-lo utilize a propriedade de comando `Command.EnablePositionalArgs`. 
* Para os métodos esse tipo de input é habilitado por padrão, para desabilita-lo veja no tópico de `Customizações`. 

# Licença


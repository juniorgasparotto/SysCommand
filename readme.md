#SysCommand

Framework for development console application using the MVC pattern. A good option of command line parser.

#Install

* NuGet: 
* NuGet Core CRL: 

#Simple example

######Code

```csharp
namespace Example
{
    using SysCommand.ConsoleApp;

    public class Program
    {
        public static int Main()
        {
            return App.RunApplication();
        }
    }

    // example with method
    public class Command1 : Command
    {
        // overload1
        public string HelloWorld(string myArg0, int? myArg1 = null)
        {
            return string.Format("My HelloWorld1 (Arg0: {0}; Arg1: {1})", myArg0, myArg1);
        }

        // overload2
        public string HelloWorld(string myArg0, DateTime myArg1)
        {
            return string.Format("My HelloWorld2 (Arg0: {0}; Arg1: {1})", myArg0, myArg1);
        }
    }
    
    // example with properties
    public class Command2 : Command
    {
        public string MyArg0 { get; set; }
        public string MyArg1 { get; set; }

        // the method main() without params is used (by convention) when one or more args is parsed
        public string Main()
        {
            return string.Format("My HelloWorld3 (Arg0: {0}; Arg1: {1})", MyArg0, MyArg1);
        }
    }
}
```
######Tests and Results

```
C:\MyApp.exe hello-world --my-arg0 ABC
My HelloWorld1 (Arg0: ABC; Arg1: )

C:\MyApp.exe hello-world ABC
My HelloWorld1 (Arg0: ABC; Arg1: ) // positional

C:\MyApp.exe hello-world --my-arg0 ABC --my-arg1 10000
My HelloWorld1 (Arg0: ABC; Arg1: 10000)

C:\MyApp.exe hello-world ABC 10000
My HelloWorld1 (Arg0: ABC; Arg1: 10000) // positional

C:\MyApp.exe hello-world --my-arg0 ABC --my-arg1 "2017-01-01 10:10:22"
My HelloWorld2 (Arg0: ABC; Arg1: 01/01/2017 10:10:22)

C:\MyApp.exe hello-world ABC "2017-01-01 10:10:22"
My HelloWorld2 (Arg0: ABC; Arg1: 01/01/2017 10:10:22) // positional

C:\MyApp.exe --my-arg0 ABC
My HelloWorld3 (Arg0: ABC; Arg1: )

C:\MyApp.exe --my-arg0 ABC --my-arg1 DEF
My HelloWorld3 (Arg0: ABC; Arg1: DEF)
```

##Comportamento padrão

Ao criar-se uma classe que herda de `Command`, em qualquer lugar do seu projeto console application, todos os seus métodos e propriedades `publicas` serão habilitados para serem chamados via prompt de comando automaticamente.

Os métodos serão convertidos em ações e as propriedades em argumentos de acordo com a seguinte regra: Converte o nome do membro (métodos, parametros e propriedades) em minusculo e adiciona um traço "-" antes de cada letra maiuscula que estiver no meio ou no final do nome. 

**Exemplo:**

```csharp
public string MyProperty { get;set; }
public void MyAction(string MyArgument);
```

```MyApp.exe my-action --my-argument value1 --my-property value2```

Em caso de propriedades ou paramentros de métodos com apenas uma letra, o padrão será deixar a letra minuscula e o input será aceito apenas na forma curta.

**Exemplo:**

```csharp
public string S { get;set; }
public void MyAction(string A);
```

```MyApp.exe -s value1 my-action -a value2```

* Para ter mais controle de quais comandos podem ser utilizados, veja o tópico de `Inicialização`.
* Para desabilitar a disponibilidade automatica de membros `publicos` ou customizar os nomes dos membros, veja o tópico de `Customizações`.

##Help automatico

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
        public CustomHelp()
        {
            HelpText = "My custom help";
        }

        public string MyCustomHelp(string action = null)
        {
            foreach(var map in this.App.Maps)
            { 
                ... 
            }
        }
    }
}
```

```MyApp.exe custom-help```

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

* Para customizar os textos de cada ação ou propriedade, veja o tópido de `Customizações`.
* O comando de help é o único que não pode ser ignorado pela inicialização, caso ele não exista na lista de tipos, ele será adicionado internamente.

##Tipos de commands

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
        HelpText = "Clear window. Only in debug";
        OnlyInDebug = true;
    }

    public void Clear()
    {
        Console.Clear();
    }
}
```

##Verbose

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

##Output

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

##Output usando template Razor

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

##Output usando template T4

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

##Output tabelado

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

##Gerenciamento de históricos de argumentos

Esse recurso permite que você salve aqueles inputs que são utilizados com muita frequencia e podem ser persistidos indeterminadamente. O seu funcionamento é bem simples, uma `Command` interno chamado `SysCommand.ConsoleApp.Commands.ArgsHistoryCommand` é responsável por indentificar as `actions` de gerenciamento e persisti-lo em um arquivo `Json` no caminho padrão `.app/history.json`. As `actions` de gerenciamento são as seguintes:

* `history-save   [name]`   -> 
* `history-load   [name]`   -> 
* `history-delete [name]`   -> 
* `history-list`            -> 



Observação: Caso você opte por escolher os comandos que farão parte da sua aplicação, todos os comandos internos, com exceção do comando de `help`, NÃO serão carregados. O comando de `help` é o único que sempre será carregado.

##Controle de erro automatico

##Trabalhando com propriedades



* Main()

##RestartResult or ActionResult


##Controle de execução das actions

StopPropagation

##Eventos

##Métodos explicitos e implicitos

* Main()

##Tipos de inputs

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

##Customizações

SysCommand.Mapping.ActionAttribute

  * string Name: Define um nome customizado para a action. Ignora o padrão acima.
  * bool Ignore: Permite que um método publico possa continuar sendo publico, mas deixar de ser uma action.
  * bool EnablePositionalArgs: Habilita ou desabilita o parse positional de argumentos. Default é `true`.
  * string Help: Texto usado para compor o help.
  * bool UsePrefix: Define se a action terá ou não o prefixo da classe pai. Default é `false`.
  * bool IsDefault: Define se um método pode ser acesso de forma implicita, ou seja, sem a obrigatoriedade de ter seu nome especificado no input.

[Action(IsDefault=true)]
public void MyAction(string MyArgument);

Explicit input

MyApp.exe my-action my-argument value

Or Implicit input

MyApp.exe my-argument value

SysCommand.Mapping.ArgumentAttribute

  * char ShortName: Indica o caracter para o argumento ser acessado de forma simples, usando apenas um traço. Por exemplo, para habilitar o verbose podemos usar a forma reduzida '-v' ao inves da forma longa '--verbose'.
  * string LongName: Indica a string da forma longa. 
  * bool IsRequired: 
  * string Help: 
  * bool HasDefaultValue: 
  * object DefaultValue: 
  * int Position: 
  * HasPosition: 
  * bool ShowHelpComplement: 


* SysCommand.ConsoleApp.App
* SysCommand.ConsoleApp.Command


É interessante manter todos os seus Command's em uma pasta chamada "Commands", deixando semelhante a estrutura do Asp.NET MVC.

####Support types

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

[action-name ][-|--|/][name][=|:| ][value]

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

####Inicialização

##Features

  * Main context:  `App`
  * Console Application with MVC
    * Parser
    * Supported types
    * Razor templates: Just use the return "Command.View()" in your actions, like MVC Web application. (using System.Web.Razor dependency)
    * T4 templates: Just use the return "Command.ViewT4()" in your actions.
    * Indented text using the class "TableView".
    * Functionality Multi Action to be possible invoke several actions in the same input. By default is enable 'App.EnableMultiAction'.
  * Automatic configuration. Just the class inherit from "Command".
  * Automatic help functionality including usage mode. Just use the input actions 'help'
  * Functionality for saving command histories. Just use the input actions 'history-save [name]', 'history-load [name]', 'history-remove [name]' and 'history-list'
  * Simple mechanism of object persistence in JSON text files (using NewtonSoft dependency)
  * Mechanism to speed development in debug mode. Just use the "App.RunInfiniteIfDebug()" method.
    * Include the command 'clear' to clear the console window when in debug mode.
  * Mechanism to help write and read informations: Just use the console wrapper "App.Console":
    * Write: Print texts using the following verbs: "Info", "Success", "Warning", "Critical", "Error", "None", "All".
    * Read: If you use the 'Writes' methods is recommended use the reads methods.
    * Verbose: Choose which are verbs can be printed in console. Just use the input argument '-v' or '--verbose'
  * Functionality to persists anything in App scope (in memory). Just use 'App.Items".
  * Events controllers "OnComplete", "OnException" e etc...
  * Extras: Simple command line parser using "OptionSet" class.

######Main context

O contexto da execução é baseado na instancia da classe `App`. Os passos são simples, criação, configuração e execução.

App:

* constructor
  *  IEnumerable<Type> commandsTypes (default null): Indica os tipos de comandos que participaração da analise e execução. Caso `null` então será feito uma pesquisa automatica de todos as classes do assembly que herdam de `Command`. A pesquisa é feita usando a classe `AppDomainCommandLoader`.
  *  bool enableMultiAction = true: Determina se a analise irá considerar a execução de mais de uma ação por linha de comando. Ver mais na sequencia.
  *  IExecutor executor = null: Alterar o executor padrão por um customizado.
  *  bool addDefaultAppHandler = true: Desabilita o handler default. O handler default nada mais é que a implementação dos eventos da class `App`.
* bool ReadArgsWhenIsDebug: 
* IEnumerable<CommandMap> Maps: 
* IEnumerable<Command> Commands: 
* ConsoleWrapper Console: 
* IDescriptor Descriptor: 
* ItemCollection Items: 

* App AddApplicationHandler(IApplicationHandler handler): 
* ApplicationResult Run(): 
* ApplicationResult Run(string arg): 
* ApplicationResult Run(string[] args): 
* int RunApplication(Func<App> appFactory = null)
* event OnComplete: 
* event OnException: 
* event OnBeforeMemberInvoke: 
* event OnAfterMemberInvoke: 
* event OnMethodReturn: 

######Parser

Ao criar-se uma classe que herda de `Command`, em qualquer lugar do seu projeto console application, todos os seus métodos e propriedades `publicas` serão habilitados para serem chamados via prompt de comando automaticamente. As customizações podem ser feitas usando atributos ou via construtor da classe de comando.

Customizações do Command:

  * App App: Referencia do contexto da aplicação.
  * string HelpText
  * bool OnlyMethodsWithAttribute
  * bool OnlyPropertiesWithAttribute 
  * bool EnablePositionalArgs
  * bool UsePrefixInAllMethods
  * string PrefixMethods
  * bool OnlyInDebug 
  * ExecutionScope ExecutionScope

```csharp
public class Command1 : Command
{
    public Command1()
    {
        this.HelpText = "My custom help";
        this.EnablePositionalArgs = true;
    }
}
```

ActionAttribute

  * Name: Define um nome customizado para a action. Por padrão, o nome do método 
  * Ignore: 
  * EnablePositionalArgs: 
  * Help: 
  * UsePrefix: 
  * IsDefault: 

ArgumentAttribute

  * char ShortName: 
  * string LongName: 
  * bool IsRequired: 
  * string Help: 
  * bool HasDefaultValue: 
  * object DefaultValue: 
  * int Position: 
  * HasPosition: 
  * bool ShowHelpComplement: 





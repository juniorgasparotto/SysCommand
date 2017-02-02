## Como funciona? !heading

Ele funciona como um analisar de linhas de comando automático onde todas as tarefas de parse ficam por conta do framework, deixando o programador focado nas regras de negócios de sua aplicação.

Veja abaixo um exemplo básico:

```csharp
namespace Example.Initialization.GettingStart
{
    using SysCommand.ConsoleApp;
    using SysCommand.Mapping;

    public class Program
    {
        public static int Main(string[] args)
        {
            return App.RunApplication();
        }
    }

    // Classes inheriting from `Command` will be automatically found by the system
    // and its public properties and methods will be available for use.
    public class MyCommand : Command
    {
        public string MyProperty { get; set; }

        // This signature "Main()" is reserved to process arguments/properties.
        public void Main()
        {
            // verify if property was inputed by user.
            if (MyProperty != null)
            {
                this.App.Console.Write(string.Format("Main MyProperty='{0}'", MyProperty));
            }
        }

        public void MyAction(bool v)
        {
            this.App.Console.Write(string.Format("MyAction v='{0}'", v));
        }
    }
}
```

outputs

```
cmd> MyApp.exe help
... the automatic help text will be shown ...

cmd> MyApp.exe --my-property value
Main MyProperty='value'

cmd> MyApp.exe my-action -v
MyAction v='True'
```

**_Note que não existe nenhum código de parse, seu código está limpo e pronto para receber comandos._**

### Entenda melhor...

Tecnicamente, existem quatro entidades de domínio que são a base do framework:

**`App`**

É o contexto da aplicação, onde uma `App` contém diversos `Commands`. É representada pela classe `SysCommand.ConsoleApp.App` e deve ser a primeira entidade a ser configurada em seu método `Main(string[] args)`. 

A inicialização do contexto da aplicação pode ser feita de duas formas, por uma instância da class `App` ou atravez do método estático `App.RunApplication` que disponibiliza um recurso muito interressante de `simulação de console` ajudando você a testar seus inputs dentro do próprio Visual Studio, sem a necessidade de executar seu ".exe" em um console externo. Veja [Classe App](#classe-app) e [Inicializando por método estático com simulador de console](#inicializando-por-método-estático-com-simulador-de-console).

**`Command`**

 Os comandos representam um agrupamento de funcionalidades do mesmo contexto de negócio, similar aos _Controllers do MVC_. Programaticamente eles são representadas por classes que herdam de `SysCommand.ConsoleApp.Command`. Cada instância de `Command` terá acesso ao contexto corrente pela propriedade `this.App`.
 
 Por padrão, o sistema buscará automaticamente qualquer classe que extenda de `Command`, sendo assim não é necessário especifica-los na inicializaçao. Veja [Tipos de comandos](#tipos-de-comandos) e [Especificando os tipos de comandos](#especificando-os-tipos-de-comandos).

**`Argument`**

Os argumentos representam o meio mais básico de uma aplicação console, são os conhecidos `--argument-name value`, `-v` e etc. Programaticamente eles são representados pelas propriedades do `Command` e devem ser acompanhados de um método chamado `Main()` (sem parâmetros) para poder interceptar se uma propriedade teve ou não input. O nome "Main" foi escolhido pela similaridade de conceito com o método `Main(string[] args)` do .NET.

Do lado do usuário, nenhuma sintaxe especial foi criada, todo o padrão já conhecido foi respeitado, ou seja, os argumentos longos são acessados com o prefixo `--` acompanhado do nome do argumento e os curtos com um traço `-` ou uma barra `/` acompanhado de apenas um caracter. Os valores dos argumentos devem estar na frente do nome do argumento separados por um espaço ` ` ou pelos caracteres `:` ou `=`.  Inputs posicionais também são suportados, possibilitando a omissão do nome do argumento.

Por padrão, todas as propriedades publicas de seu `Command` serão habilitadas para serem `arguments`. Veja [Trabalhando com propriedades](#trabalhando-com-propriedades), [Ignorar propriedades publicas por uma escolha manual usando atributo](#ignorar-propriedades-publicas-por-uma-escolha-manual-usando-atributo), [Tipos de inputs](#tipos-de-inputs) e [Tipos suportados](#tipos-suportados).

**`Action`**

Representam ações iguais as _Actions dos Controllers do MVC_. Programaticamente representam os métodos do `Command` e seus parâmetros (se existir) serão convertidos em `arguments` que só serão acessados quando acompanhados do nome da `actions`.

Seu uso é similar ao modo como usamos os recursos do `git` como: `git add -A`; `git commit -m "comments"`, onde `add` e `commit` seriam o nome das `actions` e `-A` e `-m` seus respectivos `arguments`.

Ainda é possível usar uma `action` omitindo seu nome no input, esse recurso nós chamamos de `Métodos Padrão` e se assemelha muito com o uso de propriedades.

Por padrão, todos os métodos publicos de seu `Command` serão habilitadas para serem `actions`. Veja [Trabalhando com métodos](#trabalhando-com-métodos), [Ignorar métodos publicos por uma escolha manual usando atributo](#ignorar-métodos-publicos-por-uma-escolha-manual-usando-atributo) e [Métodos padrão](#métodos-padrão).

**Exemplificando:**

```csharp
namespace Example.Initialization.GettingStart
{
    using SysCommand.ConsoleApp;
    using SysCommand.Mapping;

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
        // usage:
        // MyApp.exe add --all
        public void Add(bool all)
        {
            this.App.Console.Write("Add");
        }

        // usage:
        // MyApp.exe commit -m "comments"
        public void Commit(string m)
        {
            this.App.Console.Write("Commit");
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
        [Argument(LongName = "custom-property", ShortName = 'p', Help = "My custom argument ")]
        public decimal? MyPropertyDecimal { get; set; }

        // Method to process arguments/properties, if any exist.
        // This signature "Main()" is reserved for this use only.
        public string Main()
        {
            if (MyProperty != null)
                this.App.Console.Write(string.Format("Main MyProperty='{0}'", MyProperty));

            if (MyPropertyDecimal != null)
                this.App.Console.Write(string.Format("Main MyPropertyDecimal='{0}'", MyPropertyDecimal));

            return "Return methods can also be used as output";
        }

        // "Action without customization"
        // usage:
        // MyApp.exe my-action -p value
        public string MyAction(string p)
        {
            // Example showing that properties are executed before methods
            if (MyPropertyDecimal != null)
                this.App.Console.Write("Use property here if you want!");

            return string.Format("MyAction p='{0}'", p);
        }

        // "Action customized"
        // usage:
        // MyApp.exe custom-action
        // MyApp.exe custom-action -o
        [Action(Name = "custom-action", Help = "My custom action")]
        public string MyAction
        (
            [Argument(ShortName = 'o')]
            bool? optionalParameter = null
        )
        {
            return string.Format("MyCustomAction optionalParameter='{0}'", optionalParameter);
        }
    }
}
```

outputs

```
cmd> MyApp.exe help
... show help here ...

cmd> MyApp.exe add --all
Add

cmd> MyApp.exe commit -m "comments"
Commit

cmd> MyApp.exe --my-property value
Main MyProperty='value'
Return methods can also be used as output

cmd> MyApp.exe --my-property=value
Main MyProperty='value'
Return methods can also be used as output

cmd> MyApp.exe --custom-property 123
Main MyPropertyDecimal='123'
Return methods can also be used as output

cmd> MyApp.exe --custom-property:123
Main MyPropertyDecimal='123'
Return methods can also be used as output

cmd> MyApp.exe -p 123
Main MyPropertyDecimal='123'
Return methods can also be used as output

cmd> MyApp.exe my-action -p value
MyAction p='value'

cmd> MyApp.exe my-action /p value
MyAction p='value'

cmd> MyApp.exe custom-action
MyCustomAction optionalParameter=''

cmd> MyApp.exe custom-action -o
MyCustomAction optionalParameter='True'

cmd> MyApp.exe commit -m "my commit" --my-property=value --custom-property:123
Main MyProperty='value'
Main MyPropertyDecimal='123'
Return methods can also be used as output
Commit
```

**Saiba mais...**

* Note que os tipos primitivos de cada propriedade estão como `Nullable`, isso é importante para ter condições de identificar que o usuário fez o input de uma determinada propriedade. Veja [Trabalhando com propriedades](#trabalhando-com-propriedades).
* Todos os tipos primitivos do .NET, Enums, Enums Flags e Collections são suportados. Veja o tópico de [Tipos suportados](#tipos-suportados).
* Use `App.Console.Write()`, `App.Console.Error()` (entre outros) para imprimir seus outputs e usufruir de recursos como o `verbose`. Veja [Verbose](#verbose).
* Você pode utilizar o retorno dos métodos como `output`, inclusive o método reservado `Main()`. Ou use `void` se não quiser usar esse recurso. Veja [Output](#output).
* Se desejar, customize seus `arguments` ou `actions` usando os atributos `ArgumentAttribute` e `ActionAttribute`. Você pode customizar diversos atributos como nomes, texto de ajuda, obrigatóriedade e dentro outros. Veja [Customizando os nomes dos argumentos](#customizando-os-nomes-dos-argumentos) e [Customizando nomes de actions e arguments](#customizando-nomes-de-actions-e-arguments).
* Você pode usar métodos com o mesmo nome (sobrecargas) para definir diferentes `actions`. Elas podem ser chamadas no prompt de comando com o mesmo nome, mas os argumentos definirão qual o método a ser chamado, igual ocorre em C#. Veja [Sobrecargas](#sobrecargas)
* Opte por usar o método `int Program.Main(string[] args)` com retorno, assim você pode retornar o status code para o console. (ERROR=1 ou SUCCESS=0).
* Existe também o suporte nativo para gerar o texto de ajuda. Veja [Help automático](#help-automatico).

Esse foi apenas um resumo, para conhecer mais sobre esse projeto veja a nossa [Documentação completa](#documentação).
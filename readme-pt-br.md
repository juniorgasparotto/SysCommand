﻿[
![Inglês](https://github.com/juniorgasparotto/SysCommand/blob/master/documentation/img/en-us.png)
](https://github.com/juniorgasparotto/SysCommand)
[
![Português](https://github.com/juniorgasparotto/SysCommand/blob/master/documentation/img/pt-br.png)
](https://github.com/juniorgasparotto/SysCommand/blob/master/readme-pt-br.md)

# <a name="presentation" />SysCommand 

O `SysCommand` é um poderoso framework, multiplataforma, para desenvolver `Console Applications` em .NET. É simples, fortemente tipado e com grandes influências do padrão MVC.

## Build Status

<table>
    <tr><th>netstandard2.0+</th><th>net461+</th></tr>
    <tr>
        <td>

[![Build status](https://ci.appveyor.com/api/projects/status/6hb2sox6y6g5pwmt/branch/master?svg=true)](https://ci.appveyor.com/project/ThiagoSanches/syscommand-bg4ki/branch/master)
</td>
<td>

[![Build status](https://ci.appveyor.com/api/projects/status/36vajwj2n93f4u21/branch/master?svg=true)](https://ci.appveyor.com/project/ThiagoSanches/syscommand/branch/master)
</td>
</tr>

</table>

A partir da versão 2.0.0, apenas os novos frameworks serão suportados, veja abaixo a tabela de suporte:

<table>
<tr>
<th>Frameworks</th>
<th>Versão compatível</th>
<th>Notas da versão</th>
</tr>
<tr>
<td>netstandard2.0, net461</td>
<td>

[2.0.0](https://www.nuget.org/packages/SysCommand/2.0.0)

</td>
<td>

[notes](https://github.com/juniorgasparotto/SysCommand/releases/tag/2.0.0)

</td>
</tr>  
<tr>
<td>netstandard1.6, net452</td>
<td>

[1.0.9](https://www.nuget.org/packages/SysCommand/1.0.9)

</td>
<td>

[notes](https://github.com/juniorgasparotto/SysCommand/releases/tag/1.0.9)

</td>
</tr>
</table>

## Canais

* [Reportar um erro](https://github.com/juniorgasparotto/SysCommand/issues/new)
* [Mandar uma mensagem](https://syscommand.slack.com/)
* [Doações](https://github.com/juniorgasparotto/SysCommand/blob/master/readme-pt-br.md#donate)

# <a name="install" />Instalação

Via [NuGet](https://www.nuget.org/packages/SysCommand/):

```
Install-Package SysCommand
```

## <a name="presentation-how-it-works" />Como funciona?

Ele funciona como um analisador automatizado de linha de comando, permitindo que o programador se concentre nas regras de negócios de sua aplicação.

Para isso, você pode escolher 3 maneiras de trabalho:

* Método **Main tipado**: Equivale ao modelo tradicional `Main(string[] args)`, mas de forma tipada.
* **Propriedades**: Cada propriedade será transformada em argumentos.
* **Métodos**: Cada método será transformado em um sub-comando: **Ação**

Além disso, ele dispõe de um recurso para simular um prompt de comando dentro do proprio Visual Studio, eliminando a necessidade de testar sua aplicação fora do ambiente de desenvolvimento.

Outros recursos essênciais como `help`, `verbose`, `error handling` e outros também são suportados.

**Exemplo de `Main-typed`:**

```csharp
namespace Example.Initialization.Simple
{
    using SysCommand.ConsoleApp;

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
        // This signature "Main(...)" is reserved to process arguments fastly.
        public void Main(string myArgument, int? myArgument2 = null)
        {
            // this arg is obrigatory
            this.App.Console.Write(string.Format("myArgument='{0}'", myArgument));

            // verify if property was inputed by user.
            if (myArgument2 != null)
                this.App.Console.Write(string.Format("myArgument2='{0}'", myArgument2));
        }
    }
}
```

_Testes em um prompt externo:_

```
C:\Users\MyUser> MyApp.exe help
... the automatic help text will be shown ...

C:\Users\MyUser> MyApp.exe --my-argument "value"
myArgument='value'

C:\Users\MyUser> MyApp.exe --my-argument "value" --my-argument2 1000
myArgument='value'
myArgument2='1000'
```

_Testes no Visual Studio usando o simulador de console:_

```
cmd> help
... the automatic help text will be shown ...

cmd> --my-argument "value"
myArgument='value'

cmd> --my-argument "value" --my-argument2 1000
myArgument='value'
myArgument2='1000'
```

**Exemplo de uso com propriedades:**

```csharp
namespace Example.Initialization.Simple
{
    using SysCommand.ConsoleApp;

    public class Program
    {
        public static int Main(string[] args)
        {
            return App.RunApplication();
        }
    }

    public class MyCommand : Command
    {
        public string MyArgument { get; set; }

        // This signature "Main()" is reserved to process properties.
        public void Main()
        {
            if (MyArgument != null)
                this.App.Console.Write(string.Format("Main MyArgument='{0}'", MyArgument));
        }
    }
}
```

```
cmd> --my-argument value
Main MyArgument='value'
```

**Exemplo de ações:**

```csharp
namespace Example.Initialization.Simple
{
    using SysCommand.ConsoleApp;

    public class Program
    {
        public static int Main(string[] args)
        {
            return App.RunApplication();
        }
    }

    public class MyCommand : Command
    {
        public void MyAction(bool a)
        {
            this.App.Console.Write(string.Format("MyAction a='{0}'", a));
        }
    }
}
```

```
cmd> my-action -a
MyAction a='True'
```

**_Note que não existe nenhum código de analise em nenhum exemplo, seu código está limpo e pronto para receber comandos._**

### Entenda melhor...

Tecnicamente, existem quatro entidades de domínio que são a base do framework:

**`App`**

É o contexto da aplicação, onde uma `App` contém diversos `Commands`. É representada pela classe `SysCommand.ConsoleApp.App` e deve ser a primeira entidade a ser configurada em seu método `Main(string[] args)`.

A inicialização do contexto da aplicação pode ser feita de duas formas, por uma instância da class `App` ou através do método estático `App.RunApplication` que fornece um recurso de simulação de console que ajuda você a testar seus inputs dentro do próprio Visual Studio, sem a necessidade de executar seu ".exe" em um console externo, basta apertar o _Play_. Saiba mais: [Iniciando](https://github.com/juniorgasparotto/SysCommand/blob/master/documentation/pt-br.md#class-app) , [Inicializando com o simulador de console](https://github.com/juniorgasparotto/SysCommand/blob/master/documentation/pt-br.md#initializing-by-static-method).

**`Command`**

Os comandos representam um agrupamento de funcionalidades do mesmo contexto de negócio, similar aos _Controllers do MVC_. Programaticamente eles são representadas por classes que herdam de `SysCommand.ConsoleApp.Command`. Cada instância de `Command` terá acesso ao contexto corrente pela propriedade `this.App`.

Por padrão, o sistema tenta encontrar, de forma automática, qualquer classe que extenda de `Command`, sendo assim não é necessário especifica-los na inicializaçao, embora isso seja possível. Saiba mais: [Tipos de comandos](https://github.com/juniorgasparotto/SysCommand/blob/master/documentation/pt-br.md#kind-of-commands) , [Especificando os tipos de comandos](https://github.com/juniorgasparotto/SysCommand/blob/master/documentation/pt-br.md#specifying-commands).

**`Argument`**

Os argumentos representam o meio mais básico de uma aplicação console, são os conhecidos `--argument-name value`, `-v` e etc. Programaticamente eles são representados pelas _propriedades_ do `Command` e devem ser acompanhados de um método chamado `Main()` (sem parâmetros) para poder interceptar se uma propriedade foi ou não utilizada. O nome `Main` foi escolhido pela similaridade de conceito com o método `Main(string[] args)` do .NET.

Do lado do usuário, nenhuma sintaxe especial foi criada, os padrões mais conhecidos foram implementados. Os argumentos longos são acessados com o prefixo `--` e são acompanhados do nome do argumento. Os argumentos curtos são acessados com um traço `-` ou uma barra `/` e são acompanhados de apenas um caracter. Os valores dos argumentos devem estar na frente do nome do argumento separados por um espaço ` ` ou `:` ou `=`. Inputs posicionais também são suportados, possibilitando a omissão do nome do argumento.

Por padrão, todas as propriedades publicas de seu `Command` serão habilitadas para serem `arguments`. Saiba mais: [Trabalhando com propriedades](https://github.com/juniorgasparotto/SysCommand/blob/master/documentation/pt-br.md#properties), [Escolha manual de propriedades via atributo](https://github.com/juniorgasparotto/SysCommand/blob/master/documentation/pt-br.md#properties-ignore-public), [Input](https://github.com/juniorgasparotto/SysCommand/blob/master/documentation/pt-br.md#input), [Tipos suportados](https://github.com/juniorgasparotto/SysCommand/blob/master/documentation/pt-br.md#support-types).

**`Action`**

Representam ações iguais as _Actions dos Controllers do MVC_. Programaticamente representam os _métodos_ do `Command` e seus parâmetros (se existir) serão convertidos em `arguments` e que só serão acessados quando acompanhados do nome da ação.

Seu uso é similar ao modo como usamos os recursos do `git` como: `git add -A`; `git commit -m "comments"`, onde `add` e `commit` seriam o nome das ações e `-A`, `-m` seus respectivos argumentos.

Ainda é possível omitir o nome da ação no input do usuário. Esse recurso é chamado de **Método Padrão** e se assemelha muito com o uso de propriedades.

Por padrão, todos os métodos publicos de seu `Command` serão habilitadas para serem `actions`. Saiba mais: [Trabalhando com métodos](https://github.com/juniorgasparotto/SysCommand/blob/master/documentation/pt-br.md#methods), [Ignorar métodos publicos por uma escolha manual usando atributo](https://github.com/juniorgasparotto/SysCommand/blob/master/documentation/pt-br.md#methods-ignore-public), [Métodos padrão](https://github.com/juniorgasparotto/SysCommand/blob/master/documentation/pt-br.md#methods-default).

**Exemplo avançado:**

```csharp
namespace Example.Initialization.Advanced
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
            this.App.Console.Error("Add error");
            this.App.Console.Write("Add");
        }

        // usage:
        // MyApp.exe commit -m "comments"
        public void Commit(string m)
        {
            this.App.Console.Error("Commit error");
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

        // "Action without customization and is a overload"
        // usage:
        // MyApp.exe my-action -p value --p2
        public string MyAction(string p, bool p2)
        {
            return string.Format("MyAction p='{0}'; p2='{1}'", p, p2);
        }

        // "Action customized"
        // usage:
        // MyApp.exe custom-action
        // MyApp.exe custom-action -o
        [Action(Name = "custom-action", Help = "My custom action")]
        public string CustomAction
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

_Input para exibir o help automático:_

```
cmd> help
... show help here ...
```

_Inputs similares ao uso do git:_

```
cmd> add --all
Add

cmd> commit -m "comments"
Commit
```

_Inputs com os 3 tipos de separadores de valor:_

```
cmd> --my-property value
Main MyProperty='value'
Return methods can also be used as output

cmd> --my-property=value
Main MyProperty='value'
Return methods can also be used as output

cmd> --custom-property:123
Main MyPropertyDecimal='123'
Return methods can also be used as output
```

_Inputs com os 2 tipos de delimitador de argumentos na forma curta:_

```
cmd> -p 123
Main MyPropertyDecimal='123'
Return methods can also be used as output

cmd> /p 123
Main MyPropertyDecimal='123'
Return methods can also be used as output
```

_Inputs com as 2 sobrecargas do método MyAction:_

```
cmd> my-action -p value
MyAction p='value'

cmd> my-action -p value --p2
MyAction p='value'; p2='True'
```

_Inputs posicionais:_

```
cmd> my-action positional-value
MyAction p='positional-value'

cmd> my-action positional-value false
MyAction p='positional-value'; p2='False'
```

_Inputs com parâmetros opcionais:_

```
cmd> custom-action
MyCustomAction optionalParameter=''

cmd> custom-action -o
MyCustomAction optionalParameter='True'
```

_Input com argumentos de diferentes comandos e com o argumento de `--verbose` para permitir mostrar Erros:_

```
cmd> commit -m "my commit" --my-property=value --custom-property:123 --verbose Error
Main MyProperty='value'
Main MyPropertyDecimal='123'
Return methods can also be used as output
Commit error
Commit
```

**Saiba mais...**

* Note que os tipos primitivos de cada propriedade estão configurados como `Nullable`. Isso é importante para ter condições de identificar que o usuário inseriu uma determinada propriedade. Saiba mais: [Trabalhando com propriedades](https://github.com/juniorgasparotto/SysCommand/blob/master/documentation/pt-br.md#properties).
* Todos os tipos primitivos do .NET, Enums, Enums Flags e Collections são suportados. Veja o tópico de [Tipos suportados](https://github.com/juniorgasparotto/SysCommand/blob/master/documentation/pt-br.md#support-types).
* Use `App.Console.Write()`, `App.Console.Error()` (entre outros) para imprimir seus outputs e usufruir de recursos como o `verbose`. Saiba mais: [Verbose](https://github.com/juniorgasparotto/SysCommand/blob/master/documentation/pt-br.md#verbose).
* Você pode utilizar o retorno dos métodos como `output`, inclusive o método reservado `Main()`. Ou use `void` se não quiser usar esse recurso. Saiba mais: [Output](https://github.com/juniorgasparotto/SysCommand/blob/master/documentation/pt-br.md#output).
* Se desejar, customize seus `arguments` ou `actions` usando os atributos `ArgumentAttribute` e `ActionAttribute`. Você pode customizar diversos atributos como nomes, texto de ajuda e dentro outros. Saiba mais: [Customizando os nomes dos argumentos](https://github.com/juniorgasparotto/SysCommand/blob/master/documentation/pt-br.md#properties-customizing-name) e [Customizando nomes de ações e argumentos](https://github.com/juniorgasparotto/SysCommand/blob/master/documentation/pt-br.md#methods-customizing-names).
* Você pode usar métodos com o mesmo nome (sobrecargas) para definir diferentes `actions`. Elas podem ser chamadas no prompt de comando com o mesmo nome, mas os argumentos definirão qual o método a ser chamado, igual ocorre em `c#`. Saiba mais: [Sobrecargas](https://github.com/juniorgasparotto/SysCommand/blob/master/documentation/pt-br.md#methods-overloads)
* Opte por usar o método `int Program.Main(string[] args)` com retorno, assim você pode retornar o status code para o console. (ERROR=1 ou SUCCESS=0).
* Existe também o suporte nativo para gerar o texto de ajuda. Saiba mais: [Help](https://github.com/juniorgasparotto/SysCommand/blob/master/documentation/pt-br.md#help).

Esse foi apenas um resumo, para conhecer mais sobre esse projeto veja a nossa [Documentação](https://github.com/juniorgasparotto/SysCommand/blob/master/documentation/pt-br.md#documentation).

## <a name="what-is-the-purpose" />Qual o objetivo deste projeto?

O objetivo é ajudar programadores de qualquer linguagem de programação que sofrem para criar uma aplicação console devido a burocracia do parse e pela dificuldade de manutenção. Se você é como eu que adora criar mini-aplicações para resolver problemas do dia a dia usando consoles, então junte-se a nós!

Se você nunca trabalhou com .NET, talvez essa seja uma excelente oportunidade de conhece-lo. Com o novo .NET (Core Clr) você pode criar aplicativos em qualquer plataforma e isso pode ser simplificado com o `SysCommand`.

# <a name="install-dlls" />DLLs do pacote

* `SysCommand.dll`: Contém toda a lógica de parse e execução de linhas de comandos. Tudo foi pensado para tornar o padrão MVC tão natural quanto possível.
* `NewtonSoft.Json`: Necessário para os recursos que fazem uso de JSON.
* São dependências necessárias para o uso da sintaxe "Razor":
  * `Microsoft.CSharp`:
  * `Microsoft.AspNetCore.Mvc.Razor.Extensions`:
  * `Microsoft.AspNetCore.Razor.Runtime`:
  * `Microsoft.Extensions.DependencyModel`:

## <a name="install-step-a-step" />Passo a passo de como usar

* Criar seu projeto do tipo `Console Application`
* Instale o `SysCommand` no seu projeto `Console Application`
* Na primeira linha do método `public int Program.Main(string[] args)`, adicione o código `return App.RunApplication()`.
* Criar uma classe, em qualquer lugar, que herde de `SysCommand.ConsoleApp.Command`.
* Criar suas propriedades com seus tipos `Nullable` e deixe-as como publicas. Elas se tornarão `arguments` no prompt de comando.
* Crie um método `Main()` sem parâmetros em sua classe para poder interceptar os inputs de suas propriedades. Utilize `Property != null` para identificar que a propriedade foi inserida.
* Crie métodos publicos, com ou sem parâmetros, para que eles se tornem `actions`. Caso tenha parâmetros opcionais, deixe-os configurados como `Nullable` pela mesma razão acima.
* Digite `help` no prompt de comando que será aberto para poder visualizar suas propriedades e métodos transformados em `arguments` e `actions`.
* Agora é só usar!

# Documentação

Veja a documentação completa clicando [aqui](https://github.com/juniorgasparotto/SysCommand/blob/master/documentation/pt-br.md#documentation)

# <a name="donate" />Doações

SysCommand é um projeto de código aberto. Iniciado em 2017, muitas horas foram investidos na criação e evolução deste projeto.

E tudo com apenas um objetivo: Torná-lo uma boa ferramenta para a criação de aplicativos do tipo console.. Se o SysCommand foi útil pra você, ou se você deseja ve-lo evoluir cada vez mais, considere fazer uma pequena doação (qualquer valor). Ajude-nos também com sujestões e possíveis problemas.

De qualquer forma, agradeçemos você por ter chego até aqui ;)

**BitCoin:**

_19DmxWBNcaUGjm2PQAuMBD4Y8ZbrGyMLzK_

![bitcoinkey](https://github.com/juniorgasparotto/SysCommand/blob/master/documentation/img/bitcoinkey.png)

# <a name="license" />Licença

The MIT License (MIT)

Copyright (c) 2017 Glauber Donizeti Gasparotto Junior

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
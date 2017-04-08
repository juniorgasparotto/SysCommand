## Parser <header-set anchor-name="input-parser" />

O parser é dividido em 4 etapas fundamentais e o namespace `SysCommand.DefaultExecutor` é o responsável por conter as lógicas de cada etapa. A interface `SysCommand.DefaultExecutor.IExecutor` contém 4 métodos que representam cada uma dessas etapas e a classe `SysCommand.DefaultExecutor.Executor` implementa essa interface com as regras padrão do `SysCommand`.

As etapas são:

1. Mapeamento: Representado pelo método `GetMaps`.
2. Parser simples: Representado pelo método `ParseRaw`
3. Parser complexo: Representado pelo método `Parse`
4. Execução: Representado pelo método `Execute`

```csharp
public interface IExecutor
{
    IEnumerable<CommandMap> GetMaps(IEnumerable<CommandBase> commands);
    IEnumerable<ArgumentRaw> ParseRaw(string[] args, IEnumerable<CommandMap> commandsMap);
    ParseResult Parse(string[] args, IEnumerable<ArgumentRaw> argumentsRaw, IEnumerable<CommandMap> commandsMap, bool enableMultiAction);
    ExecutionResult Execute(ParseResult parseResult, Action<IMemberResult, ExecutionScope> onInvoke);
}
```

### Mapeamento <header-set anchor-name="input-parser-mapping" />

No mapeamento o foco é popular uma lista do modelo `SysCommand.Mapping.CommandMap` onde cada item de `CommandMap` representa um `Command`, ou seja, o mapa do comando com todas as suas `Properties` e `Methods`.

Para cada `Property` temos a classe `SysCommand.Mapping.ArgumentMap` que contém todas as informações de uma propriedade para que ela se torne um `argument` na linha de comando. Basicamente, essas informações refletem o atributo `ArgumentAttribute` somado de outras informações internas.

Para cada `Action` temos a classe `SysCommand.Mapping.ActionMap` que contém todas as informações de uma ação para que ela se torne um `action` na linha de comando. Basicamente, essas informações refletem o atributo `ActionAttribute` somado de outras informações internas. Essa classe contém uma lista com a assinatura `IEnumerable<ArgumentMap> ArgumentsMaps` que representa os seus parâmetros.

#### Retorno <header-set anchor-name="input-parser-mapping-return" />

Por fim, uma lista do tipo `IEnumerable<CommandMap>` é retornada contendo o mapa de cada `Command`.

### Parser simples <header-set anchor-name="input-parser-simple" />

É o momento onde ocorre a transformação de um `input` em objeto da forma mais simples possível, a única informação adicional que essa etapa precisa é de uma lista de `ActionMap`, assim é possível saber quando uma `action` foi inputada. Cada item é representado pela classe `SysCommand.Parsing.ArgumentRaw` que contém todas as informações do argumento como por exemplo `Name`, `Value` e `ArgumentFormat` que determina o formato do input, veja suas possibilidades:

* `Unnamed`: Argumento posicional
* `ShortNameAndNoValue`: Argumento na forma curta e sem valor (booleanos)
* `ShortNameAndHasValue`: Argumento na forma curta com valor
* `ShortNameAndHasValueInName`: Argumento na forma curta e com valor unificado com o nome do argumento usando `=` ou `:`.
* `LongNameAndNoValue`: Argumento na forma longa sem valor (booleanos)
* `LongNameAndHasValue`: Argumento na forma longa com valor
* `LongNameAndHasValueInName`: Argumento na forma longa e com valor unificado com o nome do argumento usando `=` ou `:`.

Essa etapa precisa conhecer as `actions`, pelo único motivo de escapar valores que conflitem com nomes de `actions`.

Considere que `action1` é uma ação com 1 argumento opcional chamado `--value` e que aceita valores posicionais:

```csharp
public void Action1(string value = null);`
```

_Dispara a `action1` duas vezes:_

```
MyApp.exe action1 action1
```

_Dispara a `action1` apenas 1 vez e com o valor "action1" no argumento `--value`. Sem essa escape a "action1" seria chamada duas vezes:_

```
MyApp.exe action1 \action1
```

Assim o parser sabe que o input `\action1` significa `action1`, ou seja, sem a barra de escape `\`.

#### Retorno <header-set anchor-name="input-parser-simple-return" />

Por fim, uma lista do tipo `IEnumerable<ArgumentRaw>`.

### Parser complexo <header-set anchor-name="input-parser-complex" />

É a etapa mais longa, onde combina o resultado do mapeamento com o resultado do parser simples. O objetivo é obter as melhores rotas para um mesmo input.

1. A primeira etapa consiste em encontrar os métodos de acordo com o input de entrada. Para isso, será usado como referencia todos os `ArgumentRaw` no formato `Unnamed`, ou seja, argumentos sem nomes. A busca será dentro do mapa retornado pelo método `GetMaps`. Quando um método é encontrado, uma instância do tipo `SysCommand.Parsing.ActionParsed` é criada e cada parâmetro do método será representado pela classe `SysCommand.Parsing.ArgumentParsed`.
2. A primeira `action` pode ter seu nome omitido, mas para isso ela precisa ser do tipo `Default`. Veja <header-get name="methods-default" />. Caso existam, elas só serão utilizadas quando o primeiro `ArgumentRaw` do input não é uma `action`. Nesse cenário todos os métodos `Default` serão escolhidos para a próxima etapa. Daí para frente o processo será o mesmo.
3. Após encontrar todos os métodos de cada `action` do input, será feito a divisão em níveis. Cada nível será criado da seguinte forma:
  * Se o input iniciar com argumentos então formaram o primeiro nível. Isso se não existir nenhum método `Default`.
  * Caso exista mais de uma `action` no input, incluindo `Defaults`, cada uma representará um novo nível.
  * Os argumentos que não fazem parte do mapa da `action` (sobras) formaram outro nível. Esse nível será criado na sequência do nível da `action`.
  * Caso não encontre nenhuma `action` no input e apenas argumentos, então haverá apenas um nível.
  * Caso não exista nenhum input, mas exista métodos `Default` sem parâmetros, então eles serão escolhidos para a execução.
4. Todos os níveis que não são de `action` (apenas de argumentos) serão usados para encontrar as proprieades. Quando isso acontece, cada propriedade será representada pela classe `SysCommand.Parsing.ArgumentParsed` assim como os parâmetros dos métodos.

Nota importante: Quando a flag `bool enableMultiAction` estiver desligada o parser aceitará apenas uma `action`.

**Exemplo:**

```csharp
namespace Example.Input.Parser
{
    using SysCommand.ConsoleApp;
    using System;

    public class Program
    {
        public static int Main(string[] args)
        {
            return App.RunApplication();
        }
    }

    public class Command1 : Command
    {
        public string Property1 { get; set; }

        public void Main(string a, string b, string c)
        {

        }

        public void Action1(string value = null)
        {

        }

        public void Action2(string value = null)
        {

        }
    }

    public class Command2 : Command
    {
        public string Property2 { get; set; }

        public void Action1(string value = null)
        {

        }

        public void Action2(string value = null)
        {

        }
    }
}
```

_A) 2 níveis com o primeiro pertencendo ao método default 'Main(...)':_

```
MyApp.exe --a 1 --b 2 --c 3 action2
          |      L1        |  L2   |
```

_B) 2 níveis com duas actions:_

```
MyApp.exe action1 action2
          |  L1  |   L2 |
```

_C) 3 níveis, iniciando com 1 argumentos:_

```
MyApp.exe --property1 value action1 action2
          |        L1      |   L2  |  L3  |
```

_D) 3 níveis, iniciando com 2 argumentos:_

```
MyApp.exe --property1 value --property2 value2 action1 action2
          |                L1                 |   L2  |   L3 |
```

_E) 4 níveis, com sobras de argumentos na 'action2':_

```
MyApp.exe --property1 value action1 action2 --property2 value2
          |       L1       |   L2  |  L3   |        L4       |

```

No exemplo E o argumento `--property2` foi derivado dos argumentos extras da ação `action2`. Observe que essa ação não teve seu argumento `--value` especificado no input e o argumento `--property2` não faz parte de seu mapa, sendo assim esse argumento entra como extra e insumo para o próximo nível de argumentos. Esses extras podem estar em qualquer lugar depois do nome da `action`, após seu nome, no meio ou no final.

#### Escolhendo os melhores métodos <header-set anchor-name="input-parser-complex-methods" />

Com a divisão de níveis por `action` concluída, é feito a escolha dos melhores métodos dentro de cada nível.

1. Essa escolha trabalha da seguinte forma:
  * Seleciona os métodos que tem todos os parâmetros válidos
  * Entre os métodos válidos, seleciona o primeiro método que tenha, respectivamente:
    * A maior quantidade de parâmetros combinados com o input que foi enviado
    * A menor quantidade de parâmetros em seu mapa
    * A menor quantidade de argumentos extras
2. Com o melhor método em mãos para cada nível, a próxima etapa é remover todos os métodos do mesmo nível que não combinam com o melhor método. Isso não significa que tenham que ter a mesma assinatura, ou seja, não é preciso ter o mesmo nome, nem a mesma quantidade de parâmetros e nem os mesmos tipos, nada disso importa, o que vale é a relação do input com o método.

A combinação desejada é que todos os outros métodos tenham as mesmas quantidades de parâmetros parseados (`ArgumentParsed`) e que os inputs de seus parâmetros (`IEnumerable<ArgumentRaw> AllRaw`) combinem com os inputs do melhor método, inclusive, com a mesma sequência. Isso significa que a estratégia de parse do input foi a mesma para os métodos que combinaram, assim garante que não haverá o uso do mesmo input para finalidades diferentes.

**Exemplos:**

```csharp
namespace Example.Input.Parser
{
    using SysCommand.ConsoleApp;
    using System;
    using System.Collections.Generic;

    public class Program
    {
        public static int Main(string[] args)
        {
            return App.RunApplication();
        }
    }

    public class Command1 : Command
    {
        public void Action3(string value = null)
        {
            App.Console.Write("Action3(string value = null)");
        }
    }

    public class Command2 : Command
    {
        public void Action3(int? value = null, string value2 = null)
        {
            App.Console.Write("Action3(int? value = null, string value2 = null)");
        }
    }

    public class Command3 : Command
    {
        public void Action3(List<string> value = null)
        {
            App.Console.Write("Action3(List<string> value)");
        }
    }
}
```

**Nota**: Os valores dos argumentos de todos os cenários estão no formato posicional

```
MyApp.exe action3 123 456 action3 123 456 678 action3 999
Output Level1:
    Action3(int? value = null, string value2 = null)
Output Level2:
    Action3(List<string> value)
Output Level3:
    Action3(string value = null)
    Action3(int? value = null, string value2 = null)
    Action3(List<string> value)
```

_Explicação:_

* Inputs (`ArgumentRaw`) "action3", "123", "456", "action3", "123", "456", "678", "action3", "999"
* Esse input tem 3 nível:
  * Nível 1: `action3 123 456`
    * `Action3(int? value = null, string value2 = null)`: Melhor método, todos devem ter esse modelo
      * ArgumentParsed 1: AllRaw { "123" }
      * ArgumentParsed 2: AllRaw { "456" }
    * `Action3(string value = null)`: Não foi escolhido
      * ArgumentParsed 1: AllRaw { "123" }
    * `Action3(List<string> value)`: Não foi escolhido
      * ArgumentParsed 1: AllRaw { "123", "456" }
  * Nível 2: `action3 123 456 678`
    * `Action3(List<string> value)`: Melhor método, todos devem ter esse modelo
      * ArgumentParsed 1: AllRaw { "123", "456", "678" }
    * `Action3(string value = null)`: : Não foi escolhido
      * ArgumentParsed 1: AllRaw { "123" }
    * `Action3(int? value = null, string value2 = null)`: Não foi escolhido
      * ArgumentParsed 1: AllRaw { "123" }
      * ArgumentParsed 2: AllRaw { "456" }
  * Nível 3: `action3 999`
    * `Action3(string value = null)`: Melhor método
      * ArgumentParsed 1: AllRaw { "999" }
    * `Action3(int? value = null, string value2 = null)`: A sequencia combinou
      * ArgumentParsed 1: AllRaw { "999" }
    * `Action3(List<string> value)`: A sequencia combinou
      * ArgumentParsed 1: AllRaw { "999" }

Todos os métodos "não escolhidos" foram descartados do processo. Essa regra é primordial para que mais de uma `action` seja chamada no mesmo nível.

#### Escolhendo as melhores propriedades <header-set anchor-name="input-parser-complex-properties" />

Essa escolha trabalha da seguinte forma:

1. Encontra a propriedade de referência para cada input (`ArgumentRaw`) do mesmo nível. Para isso, seleciona a primeira propriedade válida que tem o primeiro input, depois a segunda propriedade válida que tem o segundo input e assim sucessivamente até que todos os inputs sejam completamente combinados. É possível que apenas uma propriedade de referência tenha mais de um input, é o caso de listas ou `Enums Flags`. Esses tipos terão preferência para serem referências, pois combinam mais de um input. Essa regra não existe para os métodos por que os parâmetros dos melhores métodos já são referências para os demais.
2. Depois de localizar as referências, a segunda etapa é excluir as outras propriedades válidas que não combinam com as referências. Aqui é a mesma regra dos parâmetros dos métodos, ou seja, para combinar as propriedades devem ter os mesmos inputs (`ArgumentRaw`) e com as mesmas sequências. Assim garante que não haverá o uso do mesmo input para finalidades diferentes.
3. Se algum `ArgumentRaw` não for combinado, então todos os argumentos válidos serão eliminados.

**Exemplos:**

```csharp
namespace Example.Input.Parser
{
    using SysCommand.ConsoleApp;
    using System;
    using System.Collections.Generic;

    public class Program
    {
        public static int Main(string[] args)
        {
            return App.RunApplication();
        }
    }

    public class Command4 : Command
    {
        public string Prop1 { set { App.Console.Write("Command4.Prop1"); } }
        public string Prop2 { set { App.Console.Write("Command4.Prop2"); } }
        public string Prop3 { set { App.Console.Write("Command4.Prop3"); } }
        public string Prop4 { set { App.Console.Write("Command4.Prop4"); } }

        public Command4()
        {
            this.EnablePositionalArgs = true;
        }
    }

    public class Command5 : Command
    {
        [Flags]
        public enum MyEnum
        {
            A = 1, B = 2, C = 4 , D = 8
        }

        public MyEnum Prop1
        {
            set
            {
                App.Console.Write("Command5.Prop1");
            }
        }

        public Command5()
        {
            this.EnablePositionalArgs = true;
        }
    }

    public class Command6 : Command
    {
        [Flags]
        public enum MyEnum
        {
            A = 1, B = 2, C = 4
        }

        public MyEnum Prop1
        {
            set
            {
                App.Console.Write("Command6.Prop1");
            }
        }

        public Command6()
        {
            this.EnablePositionalArgs = true;
        }
    }
}
```

**Nota**: Os valores dos argumentos de todos os cenários estão no formato posicional

_Cenário 1: Propriedade com prioridade de maioria que é descartada por estar inválida_

```
MyApp.exe W Z Y T
Output Level1:
    Command4.Prop1
    Command4.Prop2
    Command4.Prop3
    Command4.Prop4
```

_Explicação:_

* Inputs (`ArgumentRaw`): "W", "Z", "Y", "T"
* Esse input tem 1 nível:
  * Command4.Prop1: Propriedade de referência para o input "W"
    * AllRaw { "W" }
  * Command4.Prop2: Propriedade de referência para o input "Z"
    * AllRaw { "Z" }
  * Command4.Prop3: Propriedade de referência para o input "Y"
    * AllRaw { "Y" }
  * Command4.Prop4: Propriedade de referência para o input "T"
    * AllRaw { "T" }
  * Command5.Prop1: Propriedade que tem prioridade, mas está inválida, o input "W" não faz parte do Enum.
    * AllRaw { "W" }
  * Command6.Prop1: Mesmo caso do `Command5.Prop1`
    * AllRaw { "W" }

_Cenário 2: Propriedade com mais combinações será a referência_

```
MyApp.exe A B C D
Output Level1:
    Command5.Prop1
```

_Explicação:_

* Inputs (`ArgumentRaw`): "A", "B", "C", "D"
* Esse input tem 1 nível:
  * Command5.Prop1: Propriedade de referência de todos os inputs
    * AllRaw { "A", "B", "C", "D" }
  * Command6.Prop1: Propriedade tem os 3 primeiros inputs, mas ela precisa ser 100% compátivel com a referência.
    * AllRaw { "A", "B", "C" }
  * Command4.Prop1: Propriedade está válida, mas o input "A" já tem a referência `Command5.Prop1` que tem prioridade por maioria.
    * AllRaw { "A" }
  * Command4.Prop2: Mesmo caso do `Command4.Prop1`
    * AllRaw { "B" }
  * Command4.Prop3: Mesmo caso do `Command4.Prop1`
    * AllRaw { "C" }
  * Command4.Prop4: Mesmo caso do `Command4.Prop1`
    * AllRaw { "D" }

_Cenário 3: Propriedade com mais combinações será a referência_

```
MyApp.exe A B C W
Output Level1:
    Command4.Prop4
    Command5.Prop1
    Command6.Prop1
```

_Explicação:_

* Inputs (`ArgumentRaw`): "A", "B", "C", "W"
* Esse input tem 1 nível:
  * Command5.Prop1: Propriedade de referência dos 3 primeiros inputs
    * AllRaw { "A", "B", "C" }
  * Command6.Prop1: Compatível com a referência
    * AllRaw { "A", "B", "C" }
  * Command4.Prop1: Propriedade está válida, mas o input "A" já tem a referência `Command5.Prop1` que tem prioridade por maioria.
    * AllRaw { "A" }
  * Command4.Prop2: Mesmo caso do `Command4.Prop1`
    * AllRaw { "B" }
  * Command4.Prop3: Mesmo caso do `Command4.Prop1`
    * AllRaw { "C" }
  * Command4.Prop4: Propriedade de referência do input "W" que é a 4 posição, posição que essa propriedade aceita.
    * AllRaw { "W" }

Todos as propriedades "não escolhidas" foram descartados do processo. Essa regra é primordial para que mais de uma propriedade seja chamada no mesmo nível.

#### Retorno <header-set anchor-name="input-parser-complex-return" />

Por fim, uma instância do tipo `SysCommand.Parsing.ParseResult` é retornada contendo:

* `Levels`: Contém todos os níveis, onde cada nível tem uma lista de `CommandParse`. Os `CommandParse` contém a lista dos membros (métodos ou propriedades) que estão válidos ou inválidos.
* `Args`: A lista de inputs que deram inicio ao parse.
* `Maps`: A lista de mapas que deram inicio ao parse.
* `EnableMultiAction`: O mesmo parâmetro de entrada que deu inicio ao parse.

### Execução <header-set anchor-name="input-parser-execution" />

A execução só ocorre se todos os níveis tiverem ao menos um `Command` válido.

Um `Command` é considerado válido quando ele tem ao menos um membro válido (método ou propriedade) e nenhum membro inválido.

Se essa regra falhar, o retorno do método `Execute()` vai indicar na propriedade `ExecutionResult.State` o tipo do erro e todos os erros serão indicados na propriedade `ExecutionResult.Errors`:

* `ExecutionState.NotFound`: Quando não encontra nenhum membro válido ou inválido em nenhum nível. Ou quando só existem propriedades e todas estão com no estado `NotMapped`.
* `ExecutionState.HasError`: Indica que existe um ou mais membros inválidos em algum dos níveis.

Se tudo estiver certo, a ordem da execução será a seguinte:

* Define o `ExecutionScope` em todos os `Command` que estão válidos. Isso é importante para o comando ter acesso ao escopo da execução corrente.
* Executa todas as propriedades, indiferentemente de qual nível ela esteja.
* Para cada `Command` válido: Caso o comando tenha propriedades, então executa o método `Main()` se estiver implementado.
* Executa todos os métodos de cada nível na ordem do menor para o maior (ou da esquerda para a direita do input).

#### Retorno <header-set anchor-name="input-parser-execution-return" />

Por fim, uma instância do tipo `SysCommand.Execution` é retornada contendo:

* `Results`: Os resultados de cada membro (métodos ou propriedades)
* `Errors`: Lista com os erros, caso existam.
* `State`: Success, HasError, NotFound
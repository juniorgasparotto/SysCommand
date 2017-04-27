## Customizando nomes de ações e argumentos <header-set anchor-name="methods-customizing-names" />

A regra a seguir descreve como é o comportamento padrão de nomenclatura para que os métodos vire uma `action` e um parâmetro vire um `argument`:

Primeiro se converte o nome do membro (métodos ou parâmetros) em minusculo, depois adiciona um traço "-" antes de cada letra maiuscula que estiver no meio ou no final do nome. No caso de paramentros com apenas uma letra, o padrão será deixar a letra minuscula e o input será aceito apenas na forma curta.

Essa é a regra padrão de nomenclarutura e você pode escolher usa-la ou customizada-la de modo total ou parcial. Para isso utilize os atributos `ActionAttribute` para métodos e `ArgumentAttribute` os parâmetros. O uso do atributo `ArgumentAttribute` é exclusivo, ao utiliza-lo você esta eliminando o padrão de nomenclatura por completo, ou seja, se você customizar a **forma curta** você será obrigado a customizar a **forma longa** também, e vice-versa. Do contrário só o formato customizado será habilitado.

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

A segunda forma é você especificar qual será o prefixo de cada `action` usando a propriedade de comando `Command.PrefixMethods`. Assim o prefixo não será processado usando o nome do comando e sim especificado por você. Vale ressaltar que a flag `Command.UsePrefixInAllMethods` ainda precisa estar ligada.

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
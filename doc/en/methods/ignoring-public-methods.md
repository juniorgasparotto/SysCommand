## Ignorar métodos publicos por uma escolha manual usando atributo !heading

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